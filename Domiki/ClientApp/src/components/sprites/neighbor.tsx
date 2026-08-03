import NeighborGenericSprite from '../../assets/neighbors/generic.svg?react';
import NeighborZarechyeSprite from '../../assets/neighbors/zarechye.svg?react';
import NeighborBorovoeSprite from '../../assets/neighbors/borovoe.svg?react';
import NeighborKamenkaSprite from '../../assets/neighbors/kamenka.svg?react';
import NeighborGlinischiSprite from '../../assets/neighbors/glinischi.svg?react';
import NeighborDubravaSprite from '../../assets/neighbors/dubrava.svg?react';
import { renderIconSprite } from './core';
import type { IconSpriteProps, SpriteComponent } from './core';

const neighborSprites: Record<string, SpriteComponent> = {
    generic: NeighborGenericSprite,
    zarechye: NeighborZarechyeSprite,
    borovoe: NeighborBorovoeSprite,
    kamenka: NeighborKamenkaSprite,
    glinischi: NeighborGlinischiSprite,
    dubrava: NeighborDubravaSprite,
};

export const NeighborSprite = (props: IconSpriteProps) => <>{renderIconSprite('neighbor', neighborSprites, NeighborGenericSprite, props)}</>;
