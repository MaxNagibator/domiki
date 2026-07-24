import { describe, expect, it } from 'vitest';
import type { DomikDto, DomikTypeDto, ExpeditionStateDto, OrderDto, ResourceTypeDto, WorkerDto } from '../types/api';
import { computeHudDigest, groupStockByDen, STOCK_DENS, type StockEntry } from './hud';

const NOW = Date.parse('2026-07-23T00:00:00.000Z');
const iso = (hoursFromNow: number) => new Date(NOW + hoursFromNow * 3600 * 1000).toISOString();

const forge: DomikTypeDto = {
    id: 10, name: 'Кузница', logicName: 'forge', maxCount: 3, availableCount: 0, maxLevel: 3, unlockLevel: 0,
    blueprintId: null, nextCountGateLevel: null,
    levels: [{ value: 1, resources: [], modificators: [], receiptIds: [1], maxManufactureCount: 1 }],
};
const well: DomikTypeDto = {
    id: 20, name: 'Колодец', logicName: 'well', maxCount: 1, availableCount: 0, maxLevel: 1, unlockLevel: 0,
    blueprintId: null, nextCountGateLevel: null,
    levels: [{ value: 1, resources: [], modificators: [], receiptIds: [], maxManufactureCount: 0 }],
};
const domikTypes = [forge, well];

const domik = (id: number, typeId: number, over: Partial<DomikDto> = {}): DomikDto => ({
    id, typeId, level: 1, finishDate: null, upgradeSeconds: null, manufactures: null, ...over,
});
const manufacture = { id: 1, finishDate: iso(2), durationSeconds: 10, plodderCount: 1, receiptId: 1, autoRepeat: false };

const order = (over: Partial<OrderDto>): OrderDto => ({
    id: 1, neighborId: 1, neighborName: 'Заречье', neighborLogicName: 'zarechye', expireDate: iso(1),
    required: [], rewardCoins: 0, rewardGold: 0, rewardReputation: 0, ...over,
});

const worker = (id: number, over: Partial<WorkerDto>): WorkerDto => ({
    id, name: `w${id}`, gender: 0, traitId: 0, traitName: '', traitLogicName: '', traitDurationPercent: 0,
    noFatigue: false, noSick: false, manufactureId: null, expeditionId: null, errandId: null, incidentId: null,
    workedSeconds: 0, restUntil: null, sickUntil: null, sickTypeId: null, skills: [], ...over,
});

const expeditionState = (active: ExpeditionStateDto['active']): ExpeditionStateDto => ({
    active, types: [], expeditionsSincePity: 0, pityThreshold: 5, maxActive: 2,
});
const expedition = (id: number, finishDate: string) => ({ id, expeditionTypeId: 1, expeditionName: 'Лес', startDate: iso(-4), finishDate });

const resType = (logicName: string, marketValue: number): ResourceTypeDto =>
    ({ id: 0, name: logicName, logicName, isFood: false, marketValue });
const entry = (logicName: string, value: number, marketValue = 10): StockEntry =>
    ({ type: resType(logicName, marketValue), value });

describe('STOCK_DENS', () => {
    it('assigns every reference resource to exactly one den', () => {
        const allStockLogicNames = [
            'stone', 'wood', 'clay', 'brick', 'board', 'tool', 'furniture', 'block', 'millstone', 'dishes',
            'grain', 'flour', 'bread', 'ore', 'iron', 'wool', 'cloth', 'cloak', 'cheese',
        ];
        const placed = STOCK_DENS.flatMap(den => den.logicNames);

        expect(new Set(placed).size).toBe(placed.length);
        expect([...allStockLogicNames].sort()).toEqual([...placed].sort());
    });
});

describe('groupStockByDen', () => {
    it('keeps only non-empty dens in reference order and sorts each by market value', () => {
        const dens = groupStockByDen([entry('bread', 3, 20), entry('grain', 8, 10), entry('board', 2, 35), entry('wood', 5, 10)]);

        expect(dens.map(den => den.key)).toEqual(['forest', 'food']);
        expect(dens[0]?.items.map(item => item.type.logicName)).toEqual(['wood', 'board']);
        expect(dens[1]?.items.map(item => item.type.logicName)).toEqual(['grain', 'bread']);
    });

    it('drops unknown resources into a trailing Прочее den so nothing vanishes', () => {
        const dens = groupStockByDen([entry('amber', 1), entry('wood', 2)]);

        expect(dens.map(den => den.label)).toEqual(['Лесное', 'Прочее']);
        expect(dens[1]?.items.map(item => item.type.logicName)).toEqual(['amber']);
    });
});

describe('computeHudDigest idle domiks', () => {
    it('counts production-capable domiks that stand with no active manufacture', () => {
        const domiks = [domik(1, 10), domik(2, 10, { manufactures: [manufacture] }), domik(3, 10, { finishDate: iso(2) }), domik(4, 20)];

        expect(computeHudDigest(domiks, domikTypes, [], null, [], NOW).idleDomiks).toBe(1);
    });
});

describe('computeHudDigest soonest order', () => {
    it('surfaces the nearest order only within the alert window and rounds hours up', () => {
        const orders = [order({ id: 1, expireDate: iso(2.2) }), order({ id: 2, neighborName: 'Боровое', neighborLogicName: 'borovoe', expireDate: iso(0.4) })];

        expect(computeHudDigest([], domikTypes, orders, null, [], NOW).soonestOrder).toEqual({ neighborName: 'Боровое', neighborLogicName: 'borovoe', hours: 1 });
    });

    it('stays silent when the nearest order is beyond the alert window or already expired', () => {
        const orders = [order({ id: 1, expireDate: iso(9) }), order({ id: 2, expireDate: iso(-1) })];

        expect(computeHudDigest([], domikTypes, orders, null, [], NOW).soonestOrder).toBeNull();
    });
});

describe('computeHudDigest expeditions and workers', () => {
    it('counts returned expeditions, not those still afield', () => {
        const state = expeditionState([expedition(1, iso(-0.1)), expedition(2, iso(3))]);

        expect(computeHudDigest([], domikTypes, [], state, [], NOW).expeditionsBack).toBe(1);
    });

    it('splits sick from resting and ignores expired timers', () => {
        const workers = [
            worker(1, { sickUntil: iso(4) }),
            worker(2, { restUntil: iso(2), sickUntil: iso(4) }),
            worker(3, { restUntil: iso(1) }),
            worker(4, { restUntil: iso(-1) }),
        ];

        const digest = computeHudDigest([], domikTypes, [], null, workers, NOW);

        expect(digest.workersSick).toBe(2);
        expect(digest.workersResting).toBe(1);
    });
});
