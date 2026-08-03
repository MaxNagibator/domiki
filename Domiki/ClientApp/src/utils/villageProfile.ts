import type { DomikTypeDto, NeighborReputationDto, VillageProfileDto } from '../types/api';

export type ProfileState = 'adopted' | 'available' | 'need-reputation' | 'need-level' | 'cooldown' | 'closed';

export const VILLAGE_PROFILE_LEVEL_REQUIREMENT = 20;
export const VILLAGE_PROFILE_REPUTATION_REQUIREMENT = 10;

export interface VillageProfileState {
    state: ProfileState;
    buildings: DomikTypeDto[];
    durationPercent: number;
}

export function getVillageProfileState(
    profiles: VillageProfileDto[],
    domikTypes: DomikTypeDto[],
    reputation: NeighborReputationDto,
    villageLevel: number,
    profileNeighborId: number | null,
    profileChangeAvailableDate: string | null,
    now: number,
): VillageProfileState {
    const effects = profiles.filter(effect => effect.neighborId === reputation.neighborId);
    const buildings = effects
        .map(effect => domikTypes.find(type => type.id === effect.domikTypeId))
        .filter((type): type is DomikTypeDto => type != null);
    const durationPercent = effects[0]?.durationPercent ?? 100;

    if (!reputation.isOpen || effects.length === 0) {
        return { state: 'closed', buildings, durationPercent };
    }

    if (profileNeighborId === reputation.neighborId) {
        return { state: 'adopted', buildings, durationPercent };
    }

    if (villageLevel < VILLAGE_PROFILE_LEVEL_REQUIREMENT) {
        return { state: 'need-level', buildings, durationPercent };
    }

    if (reputation.points < VILLAGE_PROFILE_REPUTATION_REQUIREMENT) {
        return { state: 'need-reputation', buildings, durationPercent };
    }

    if (profileChangeAvailableDate != null && Date.parse(profileChangeAvailableDate) > now) {
        return { state: 'cooldown', buildings, durationPercent };
    }

    return { state: 'available', buildings, durationPercent };
}
