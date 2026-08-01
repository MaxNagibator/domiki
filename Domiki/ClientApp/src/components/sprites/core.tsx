import type { FC, SVGProps } from 'react';
import { prepareInlineSprite } from '../../utils/inlineSprite';

export type SpriteComponent = FC<SVGProps<SVGSVGElement>>;

export const cleanSpriteProps = (props: SVGProps<SVGSVGElement>): SVGProps<SVGSVGElement> => {
    const hidden = props['aria-hidden'] ?? (props['aria-label'] == null ? true : undefined);
    return {
        ...props,
        className: props.className == null ? 'px-sprite' : `px-sprite ${props.className}`,
        ref: prepareInlineSprite,
        role: hidden ? undefined : (props.role ?? 'img'),
        'aria-hidden': hidden,
        'aria-labelledby': undefined,
        focusable: 'false',
    };
};

export interface IconSpriteProps extends SVGProps<SVGSVGElement> {
    logicName: string;
    size?: 24 | 32 | 40 | 48 | 64;
}

const warnedUnknownSprites = new Set<string>();

export const warnUnknownSprite = (kind: string, logicName: string) => {
    if (!import.meta.env.DEV) {
        return;
    }

    const key = `${kind}:${logicName}`;
    if (!warnedUnknownSprites.has(key)) {
        warnedUnknownSprites.add(key);
        console.warn(`[sprites] Unknown ${kind} logicName: "${logicName}"`);
    }
};

export const renderIconSprite = (kind: string, sprites: Record<string, SpriteComponent>, fallback: SpriteComponent | undefined, { logicName, size = 32, ...props }: IconSpriteProps) => {
    const mappedSprite = sprites[logicName];
    if (mappedSprite == null) {
        warnUnknownSprite(kind, logicName);
    }
    const Sprite = mappedSprite ?? fallback;
    return Sprite == null ? null : <Sprite data-size={size} {...cleanSpriteProps(props)} />;
};

export interface SpriteProps extends SVGProps<SVGSVGElement> {
    logicName: string;
    level?: number;
    working?: boolean;
}

export const clampLevel = (level: number) => Math.min(5, Math.max(1, Math.floor(level)));
