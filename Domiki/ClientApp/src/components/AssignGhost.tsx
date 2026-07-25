import { useEffect, useRef } from 'react';
import type { GhostTracker } from '../hooks/useWorkerAssign';
import { WorkerSprite } from './sprites';

const GHOST_OFFSET = 12;

interface AssignGhostProps {
    ghost: GhostTracker;
    name: string;
}

export const AssignGhost = ({ ghost, name }: AssignGhostProps) => {
    const node = useRef<HTMLDivElement>(null);

    useEffect(() => {
        const paint = (point: { x: number; y: number }) => {
            node.current?.style.setProperty('transform', `translate3d(${String(point.x + GHOST_OFFSET)}px, ${String(point.y + GHOST_OFFSET)}px, 0)`);
        };

        paint(ghost.point());
        return ghost.subscribe(paint);
    }, [ghost]);

    return (
        <div className="assign-ghost" ref={node} aria-hidden="true">
            <WorkerSprite name={name} state="working" />
            <span>{name}</span>
        </div>
    );
};
