import TraitOrdinarySprite from '../../assets/traits/ordinary.svg?react';
import TraitNimbleSprite from '../../assets/traits/nimble.svg?react';
import TraitDiligentSprite from '../../assets/traits/diligent.svg?react';
import TraitSonyaSprite from '../../assets/traits/sonya.svg?react';
import TraitLuckySprite from '../../assets/traits/lucky.svg?react';
import TraitThriftySprite from '../../assets/traits/thrifty.svg?react';
import { renderIconSprite } from './core';
import type { IconSpriteProps, SpriteComponent } from './core';

const traitSprites: Record<string, SpriteComponent> = {
    ordinary: TraitOrdinarySprite,
    nimble: TraitNimbleSprite,
    diligent: TraitDiligentSprite,
    sonya: TraitSonyaSprite,
    lucky: TraitLuckySprite,
    thrifty: TraitThriftySprite,
};

export const TraitSprite = (props: IconSpriteProps) => <>{renderIconSprite('trait', traitSprites, TraitOrdinarySprite, props)}</>;
