import type { SVGProps } from 'react';
import SheepActorSprite from '../../assets/actors/sheep.svg?react';
import { cleanSpriteProps } from './core';

interface SheepSpriteProps extends SVGProps<SVGSVGElement> {
    state?: 'idle' | 'walking';
}

export const SheepSprite = ({ state = 'idle', ...props }: SheepSpriteProps) =>
    <SheepActorSprite data-state={state} {...cleanSpriteProps(props)} />;
