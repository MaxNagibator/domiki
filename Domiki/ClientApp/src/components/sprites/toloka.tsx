import BridgeSprite from '../../assets/tolokaTypes/bridge.svg?react';
import CaravanSprite from '../../assets/tolokaTypes/caravan.svg?react';
import GranarySprite from '../../assets/tolokaTypes/granary.svg?react';
import KilnSprite from '../../assets/tolokaTypes/kiln.svg?react';
import { cleanSpriteProps, clampLevel, warnUnknownSprite } from './core';
import type { SpriteComponent, SpriteProps } from './core';

const tolokaSprites: Record<string, SpriteComponent> = {
    bridge: BridgeSprite,
    granary: GranarySprite,
    kiln: KilnSprite,
    caravan: CaravanSprite,
};

export const TolokaSprite = ({ logicName, level = 1, ...props }: SpriteProps) => {
    const Sprite = tolokaSprites[logicName];
    if (Sprite == null) {
        warnUnknownSprite('toloka', logicName);
    }
    return Sprite == null ? null : <Sprite data-level={clampLevel(level)} {...cleanSpriteProps(props)} />;
};
