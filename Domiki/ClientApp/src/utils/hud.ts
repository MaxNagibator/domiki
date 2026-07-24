import type { DomikDto, DomikTypeDto, ExpeditionStateDto, OrderDto, ResourceTypeDto, WorkerDto } from '../types/api';
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

export interface HudDigest {
    idleDomiks: number;
    soonestOrder: { neighborName: string; neighborLogicName: string; hours: number } | null;
    expeditionsBack: number;
    workersResting: number;
    workersSick: number;
}

function isProductionCapable(domik: DomikDto, domikTypes: DomikTypeDto[]): boolean {
    const type = domikTypes.find(t => t.id === domik.typeId);
    const level = type?.levels.find(l => l.value === domik.level);
    return level != null && level.maxManufactureCount > 0 && level.receiptIds.length > 0;
}

export function computeHudDigest(
    domiks: DomikDto[],
    domikTypes: DomikTypeDto[],
    orders: OrderDto[],
    expeditions: ExpeditionStateDto | null,
    workers: WorkerDto[],
    now: number,
): HudDigest {
    const idleDomiks = domiks.filter(domik =>
        domik.finishDate == null
        && (domik.manufactures?.length ?? 0) === 0
        && isProductionCapable(domik, domikTypes)).length;

    const soonest = orders
        .map(order => ({ order, remaining: remainingSeconds(order.expireDate, now) }))
        .filter(entry => entry.remaining > 0)
        .sort((a, b) => a.remaining - b.remaining)[0];
    const soonestOrder = soonest != null && soonest.remaining <= ORDER_SOON_HOURS * 3600
        ? { neighborName: soonest.order.neighborName, neighborLogicName: soonest.order.neighborLogicName, hours: Math.max(1, Math.ceil(soonest.remaining / 3600)) }
        : null;

    const expeditionsBack = (expeditions?.active ?? [])
        .filter(expedition => remainingSeconds(expedition.finishDate, now) <= 0).length;

    const workersSick = workers.filter(worker =>
        worker.sickUntil != null && remainingSeconds(worker.sickUntil, now) > 0).length;
    const workersResting = workers.filter(worker =>
        (worker.sickUntil == null || remainingSeconds(worker.sickUntil, now) <= 0)
        && worker.restUntil != null && remainingSeconds(worker.restUntil, now) > 0).length;

    return { idleDomiks, soonestOrder, expeditionsBack, workersResting, workersSick };
}
