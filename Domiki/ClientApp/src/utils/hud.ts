import type { DomikDto, DomikTypeDto, ExpeditionStateDto, OrderDto, ReceiptDto, ResourceDto, ResourceTypeDto, WorkerDto } from '../types/api';
import { buildDomikNamer } from './domikNames';
import { canAffordUpgrade, resourceShortfall } from './game';
import { remainingSeconds } from './time';

export const ORDER_SOON_HOURS = 3;

export interface StockDen {
    key: string;
    label: string;
    logicNames: readonly string[];
}

export const STOCK_DENS: readonly StockDen[] = [
    { key: 'forest', label: 'Лесное', logicNames: ['wood', 'board', 'furniture'] },
    { key: 'pottery', label: 'Гончарное', logicNames: ['clay', 'brick', 'dishes'] },
    { key: 'stone', label: 'Каменное', logicNames: ['stone', 'block', 'millstone'] },
    { key: 'smithy', label: 'Кузнечное', logicNames: ['ore', 'iron', 'tool'] },
    { key: 'food', label: 'Съестное', logicNames: ['grain', 'flour', 'bread', 'cheese'] },
    { key: 'cloth', label: 'Суконное', logicNames: ['wool', 'cloth', 'cloak'] },
];

const OTHER_DEN_KEY = 'other';
const OTHER_DEN_LABEL = 'Прочее';

export interface StockEntry {
    type: ResourceTypeDto;
    value: number;
}

export interface StockDenView {
    key: string;
    label: string;
    items: StockEntry[];
}

export function groupStockByDen(stock: StockEntry[]): StockDenView[] {
    const denByLogicName = new Map<string, string>();
    for (const den of STOCK_DENS) {
        for (const logicName of den.logicNames) {
            denByLogicName.set(logicName, den.key);
        }
    }

    const views = new Map<string, StockDenView>(
        [...STOCK_DENS.map(den => ({ key: den.key, label: den.label })), { key: OTHER_DEN_KEY, label: OTHER_DEN_LABEL }]
            .map(den => [den.key, { ...den, items: [] as StockEntry[] }]),
    );

    for (const entry of stock) {
        const key = denByLogicName.get(entry.type.logicName) ?? OTHER_DEN_KEY;
        views.get(key)?.items.push(entry);
    }

    return [...views.values()]
        .filter(view => view.items.length > 0)
        .map(view => ({ ...view, items: view.items.sort((a, b) => a.type.marketValue - b.type.marketValue) }));
}

export interface HudBuildingRef {
    domikId: number;
    typeId: number;
    logicName: string;
    displayName: string;
    level: number;
}

export interface HudBlockedBuilding extends HudBuildingRef {
    missing: ResourceDto[];
}

export interface HudSoonestOrder {
    neighborName: string;
    neighborLogicName: string;
    hours: number;
}

export interface HudStandingShift {
    manufactureId: number;
    domikId: number;
    domikName: string;
    receiptId: number;
    receiptName: string;
    finishDate: string;
}

export interface HudDigest {
    idleDomiks: number;
    soonestOrder: HudSoonestOrder | null;
    expeditionsBack: number;
    workersResting: number;
    workersSick: number;
    idleBuildings: HudBuildingRef[];
    blockedBuildings: HudBlockedBuilding[];
    restingEarliest: string | null;
    sickEarliest: string | null;
    upgradeableBuildings: HudBuildingRef[];
    standingShifts: HudStandingShift[];
}

function isProductionCapable(domik: DomikDto, domikTypes: DomikTypeDto[]): boolean {
    const type = domikTypes.find(t => t.id === domik.typeId);
    const level = type?.levels.find(l => l.value === domik.level);
    return level != null && level.maxManufactureCount > 0 && level.receiptIds.length > 0;
}

function isIdle(domik: DomikDto, domikTypes: DomikTypeDto[]): boolean {
    return domik.finishDate == null
        && (domik.manufactures?.length ?? 0) === 0
        && isProductionCapable(domik, domikTypes);
}

function levelReceipts(domik: DomikDto, domikTypes: DomikTypeDto[], receipts: ReceiptDto[]): ReceiptDto[] {
    const type = domikTypes.find(t => t.id === domik.typeId);
    const level = type?.levels.find(l => l.value === domik.level);
    return (level?.receiptIds ?? []).flatMap(receiptId => {
        const receipt = receipts.find(r => r.id === receiptId);
        return receipt == null ? [] : [receipt];
    });
}

function earliestIso(dates: readonly string[]): string | null {
    return dates.length === 0 ? null : dates.reduce((earliest, date) => Date.parse(date) < Date.parse(earliest) ? date : earliest);
}

function byLevelDescThenNameAsc(a: HudBuildingRef, b: HudBuildingRef): number {
    return b.level - a.level || a.displayName.localeCompare(b.displayName, 'ru');
}

function totalShortfall(building: HudBlockedBuilding): number {
    return building.missing.reduce((sum, item) => sum + item.value, 0);
}

export function computeHudDigest(
    domiks: DomikDto[],
    domikTypes: DomikTypeDto[],
    receipts: ReceiptDto[],
    resources: ResourceDto[],
    orders: OrderDto[],
    expeditions: ExpeditionStateDto | null,
    workers: WorkerDto[],
    now: number,
): HudDigest {
    const namer = buildDomikNamer(domiks);
    const buildingRef = (domik: DomikDto, domikType: DomikTypeDto): HudBuildingRef => ({
        domikId: domik.id,
        typeId: domik.typeId,
        logicName: domikType.logicName,
        displayName: namer(domik.typeId, domik.id, domikType.name, domikType.logicName),
        level: domik.level,
    });

    const idleBuildings: HudBuildingRef[] = [];
    const blockedBuildings: HudBlockedBuilding[] = [];
    for (const domik of domiks) {
        if (!isIdle(domik, domikTypes)) {
            continue;
        }
        const domikType = domikTypes.find(t => t.id === domik.typeId);
        if (domikType == null) {
            continue;
        }

        const shortfalls = levelReceipts(domik, domikTypes, receipts)
            .map(receipt => ({ receipt, missing: resourceShortfall(receipt.inputResources, resources) }));
        const affordable = shortfalls.length === 0 || shortfalls.some(entry => entry.missing.length === 0);
        if (affordable) {
            idleBuildings.push(buildingRef(domik, domikType));
            continue;
        }

        const worst = shortfalls.reduce((best, entry) =>
            entry.missing.reduce((sum, m) => sum + m.value, 0) < best.missing.reduce((sum, m) => sum + m.value, 0) ? entry : best);
        blockedBuildings.push({ ...buildingRef(domik, domikType), missing: worst.missing });
    }
    const idleDomiks = idleBuildings.length + blockedBuildings.length;
    idleBuildings.sort(byLevelDescThenNameAsc);
    blockedBuildings.sort((a, b) => totalShortfall(a) - totalShortfall(b));

    const upgradeableBuildings: HudBuildingRef[] = domiks.flatMap(domik => {
        const domikType = domikTypes.find(t => t.id === domik.typeId);
        return domikType != null && canAffordUpgrade(domik, domikType, resources) ? [buildingRef(domik, domikType)] : [];
    });
    upgradeableBuildings.sort(byLevelDescThenNameAsc);

    const standingShifts: HudStandingShift[] = domiks.flatMap(domik => {
        const domikType = domikTypes.find(t => t.id === domik.typeId);
        if (domikType == null) {
            return [];
        }
        return (domik.manufactures ?? []).flatMap(manufacture => {
            if (!manufacture.autoRepeat) {
                return [];
            }
            const receipt = receipts.find(r => r.id === manufacture.receiptId);
            if (receipt == null) {
                return [];
            }
            return [{
                manufactureId: manufacture.id,
                domikId: domik.id,
                domikName: namer(domik.typeId, domik.id, domikType.name, domikType.logicName),
                receiptId: receipt.id,
                receiptName: receipt.name,
                finishDate: manufacture.finishDate,
            }];
        });
    });

    const soonest = orders
        .map(order => ({ order, remaining: remainingSeconds(order.expireDate, now) }))
        .filter(entry => entry.remaining > 0)
        .sort((a, b) => a.remaining - b.remaining)[0];
    const soonestOrder = soonest != null && soonest.remaining <= ORDER_SOON_HOURS * 3600
        ? {
            neighborName: soonest.order.neighborName,
            neighborLogicName: soonest.order.neighborLogicName,
            hours: Math.max(1, Math.ceil(soonest.remaining / 3600)),
        }
        : null;

    const expeditionsBack = (expeditions?.active ?? [])
        .filter(expedition => remainingSeconds(expedition.finishDate, now) <= 0).length;

    const sickWorkers = workers.filter(worker =>
        worker.sickUntil != null && remainingSeconds(worker.sickUntil, now) > 0);
    const restingWorkers = workers.filter(worker =>
        (worker.sickUntil == null || remainingSeconds(worker.sickUntil, now) <= 0)
        && worker.restUntil != null && remainingSeconds(worker.restUntil, now) > 0);

    return {
        idleDomiks,
        soonestOrder,
        expeditionsBack,
        workersResting: restingWorkers.length,
        workersSick: sickWorkers.length,
        idleBuildings,
        blockedBuildings,
        restingEarliest: earliestIso(restingWorkers.flatMap(worker => worker.restUntil != null ? [worker.restUntil] : [])),
        sickEarliest: earliestIso(sickWorkers.flatMap(worker => worker.sickUntil != null ? [worker.sickUntil] : [])),
        upgradeableBuildings,
        standingShifts,
    };
}
