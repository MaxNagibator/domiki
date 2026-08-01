import DecorBenchSprite from '../../assets/decorTypes/bench.svg?react';
import DecorCarpPondSprite from '../../assets/decorTypes/carp_pond.svg?react';
import DecorCarvedGateSprite from '../../assets/decorTypes/carved_gate.svg?react';
import DecorCraneWellSprite from '../../assets/decorTypes/crane_well.svg?react';
import DecorFenceSprite from '../../assets/decorTypes/fence.svg?react';
import DecorFlagSprite from '../../assets/decorTypes/flag.svg?react';
import DecorFlowersSprite from '../../assets/decorTypes/flowers.svg?react';
import DecorFountainSprite from '../../assets/decorTypes/fountain.svg?react';
import DecorGardenSprite from '../../assets/decorTypes/garden.svg?react';
import DecorGazeboSprite from '../../assets/decorTypes/gazebo.svg?react';
import DecorTrophySprite from '../../assets/decorTypes/trophy.svg?react';
import DecorBrickArchSprite from '../../assets/decorTypes/brick_arch.svg?react';
import DecorLanternSprite from '../../assets/decorTypes/lantern.svg?react';
import { renderIconSprite } from './core';
import type { IconSpriteProps, SpriteComponent } from './core';

const decorSprites: Record<string, SpriteComponent> = {
    fence: DecorFenceSprite,
    carved_gate: DecorCarvedGateSprite,
    crane_well: DecorCraneWellSprite,
    gazebo: DecorGazeboSprite,
    carp_pond: DecorCarpPondSprite,
    flowerbed: DecorFlowersSprite,
    garden: DecorGardenSprite,
    fountain: DecorFountainSprite,
    bench: DecorBenchSprite,
    trail_idol: DecorTrophySprite,
    wanderer_banner: DecorFlagSprite,
    brick_arch: DecorBrickArchSprite,
    lantern: DecorLanternSprite,
};

export const DecorSprite = (props: IconSpriteProps) => <>{renderIconSprite('decor', decorSprites, undefined, props)}</>;
