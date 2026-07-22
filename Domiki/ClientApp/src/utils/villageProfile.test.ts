import { describe, expect, it } from 'vitest';
import type { DomikTypeDto, NeighborReputationDto, VillageProfileDto } from '../types/api';
import { getVillageProfileState, VILLAGE_PROFILE_LEVEL_REQUIREMENT, VILLAGE_PROFILE_REPUTATION_REQUIREMENT, type ProfileState } from './villageProfile';

const NEIGHBOR_ID = 1;
const NOW = 1_000_000;

const domikTypes: DomikTypeDto[] = [
    { id: 10, name: 'Кузница', logicName: 'forge', maxCount: 1, availableCount: 0, maxLevel: 5, unlockLevel: 0, blueprintId: null, nextCountGateLevel: null, levels: [] },
    { id: 11, name: 'Каменоломня', logicName: 'stone_mine', maxCount: 1, availableCount: 0, maxLevel: 5, unlockLevel: 0, blueprintId: null, nextCountGateLevel: null, levels: [] },
];

const profiles: VillageProfileDto[] = [
    { neighborId: NEIGHBOR_ID, domikTypeId: 10, durationPercent: 85 },
    { neighborId: NEIGHBOR_ID, domikTypeId: 11, durationPercent: 85 },
];

const baseReputation: NeighborReputationDto = {
    neighborId: NEIGHBOR_ID,
    neighborName: 'Заречье',
    neighborLogicName: 'zarechye',
    points: VILLAGE_PROFILE_REPUTATION_REQUIREMENT,
    nextThreshold: null,
    nextRewardName: null,
    isFriend: false,
    isOpen: true,
};

const future = new Date(NOW + 1000).toISOString();
const past = new Date(NOW - 1000).toISOString();

describe('getVillageProfileState', () => {
    it.each<[string, Partial<NeighborReputationDto>, VillageProfileDto[], number, number | null, string | null, ProfileState]>([
        ['road not open, everything else fine', { isOpen: false }, profiles, VILLAGE_PROFILE_LEVEL_REQUIREMENT, null, null, 'closed'],
        ['neighbor has no profile rows at all', {}, [], VILLAGE_PROFILE_LEVEL_REQUIREMENT, null, null, 'closed'],
        ['already the adopted profile', {}, profiles, VILLAGE_PROFILE_LEVEL_REQUIREMENT, NEIGHBOR_ID, future, 'adopted'],
        ['village level below requirement', {}, profiles, VILLAGE_PROFILE_LEVEL_REQUIREMENT - 1, null, null, 'need-level'],
        ['reputation below requirement', { points: VILLAGE_PROFILE_REPUTATION_REQUIREMENT - 1 }, profiles, VILLAGE_PROFILE_LEVEL_REQUIREMENT, null, null, 'need-reputation'],
        ['profile change cooldown still running', {}, profiles, VILLAGE_PROFILE_LEVEL_REQUIREMENT, null, future, 'cooldown'],
        ['cooldown already passed', {}, profiles, VILLAGE_PROFILE_LEVEL_REQUIREMENT, null, past, 'available'],
        ['never adopted any profile before', {}, profiles, VILLAGE_PROFILE_LEVEL_REQUIREMENT, null, null, 'available'],
        ['adopted wins over below-level and cooldown', { points: 0 }, profiles, 0, NEIGHBOR_ID, future, 'adopted'],
        ['closed wins over adopted when the road closes afterwards', { isOpen: false }, profiles, VILLAGE_PROFILE_LEVEL_REQUIREMENT, NEIGHBOR_ID, future, 'closed'],
    ])('%s -> %s', (_name, reputationOverride, effectiveProfiles, villageLevel, profileNeighborId, profileChangeAvailableDate, expected) => {
        const reputation = { ...baseReputation, ...reputationOverride };
        const result = getVillageProfileState(effectiveProfiles, domikTypes, reputation, villageLevel, profileNeighborId, profileChangeAvailableDate, NOW);
        expect(result.state).toBe(expected);
    });

    it('collects the two specialization buildings and the shared duration percent', () => {
        const result = getVillageProfileState(profiles, domikTypes, baseReputation, VILLAGE_PROFILE_LEVEL_REQUIREMENT, null, null, NOW);
        expect(result.buildings.map(type => type.logicName)).toEqual(['forge', 'stone_mine']);
        expect(result.durationPercent).toBe(85);
    });

    it('reports no buildings and a neutral duration percent for a neighbor without profile rows', () => {
        const result = getVillageProfileState([], domikTypes, baseReputation, VILLAGE_PROFILE_LEVEL_REQUIREMENT, null, null, NOW);
        expect(result.buildings).toEqual([]);
        expect(result.durationPercent).toBe(100);
    });
});
