import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import type { PointerEvent as ReactPointerEvent } from 'react';
import { perfGestureEnd, perfGestureFrame, perfGestureMove, perfGestureStart } from '../utils/perf';

const MOVE_THRESHOLD = 6;

export interface AssignPoint {
    x: number;
    y: number;
}

export interface GhostTracker {
    point: () => AssignPoint;
    subscribe: (paint: (point: AssignPoint) => void) => () => void;
}

export interface WorkerAssign {
    workerId: number | null;
    dragging: boolean;
    hoverDomikId: number | null;
    ghost: GhostTracker;
    grab: (workerId: number, event: ReactPointerEvent) => void;
    drop: (domikId: number, point: AssignPoint) => void;
    cancel: () => void;
}

function domikAtPoint(x: number, y: number): number | null {
    const holder = document.elementFromPoint(x, y)?.closest('[data-assign-domik]');
    const raw = holder?.getAttribute('data-assign-domik');
    return raw == null || raw === '' ? null : Number(raw);
}

export function useWorkerAssign(onAssign: (workerId: number, domikId: number, point: AssignPoint) => void): WorkerAssign {
    const [workerId, setWorkerId] = useState<number | null>(null);
    const [dragging, setDragging] = useState(false);
    const [hoverDomikId, setHoverDomikId] = useState<number | null>(null);
    const origin = useRef<AssignPoint | null>(null);
    const assignRef = useRef(onAssign);
    const ghostPoint = useRef<AssignPoint>({ x: 0, y: 0 });
    const painters = useRef(new Set<(point: AssignPoint) => void>());
    const frame = useRef(0);
    const draggingRef = useRef(false);
    const hover = useRef<number | null>(null);

    useEffect(() => { assignRef.current = onAssign; });

    const paintGhost = useCallback(() => {
        painters.current.forEach(paint => { paint(ghostPoint.current); });
    }, []);

    const ghost = useMemo<GhostTracker>(() => ({
        point: () => ghostPoint.current,
        subscribe: paint => {
            const set = painters.current;
            set.add(paint);
            return () => set.delete(paint);
        },
    }), []);

    const finish = useCallback(() => {
        perfGestureEnd();
        if (frame.current !== 0) {
            cancelAnimationFrame(frame.current);
            frame.current = 0;
        }

        origin.current = null;
        draggingRef.current = false;
        hover.current = null;
        setWorkerId(null);
        setDragging(false);
        setHoverDomikId(null);
    }, []);

    const drop = useCallback((domikId: number, point: AssignPoint) => {
        const held = workerId;
        finish();
        if (held != null) {
            assignRef.current(held, domikId, point);
        }
    }, [workerId, finish]);

    const grab = useCallback((id: number, event: ReactPointerEvent) => {
        if (event.button !== 0 && event.pointerType === 'mouse') {
            return;
        }

        perfGestureStart();
        origin.current = { x: event.clientX, y: event.clientY };
        ghostPoint.current = { x: event.clientX, y: event.clientY };
        draggingRef.current = false;
        setWorkerId(id);
        setDragging(false);
        setHoverDomikId(null);
    }, []);

    useEffect(() => {
        if (workerId == null) {
            return;
        }

        const onMove = (event: PointerEvent) => {
            const start = origin.current;
            if (start == null) {
                return;
            }

            const moved = Math.abs(event.clientX - start.x) > MOVE_THRESHOLD || Math.abs(event.clientY - start.y) > MOVE_THRESHOLD;
            if (!moved) {
                return;
            }

            event.preventDefault();
            perfGestureMove();
            ghostPoint.current = { x: event.clientX, y: event.clientY };
            if (!draggingRef.current) {
                draggingRef.current = true;
                setDragging(true);
            }

            if (frame.current !== 0) {
                return;
            }

            frame.current = requestAnimationFrame(() => {
                frame.current = 0;
                paintGhost();
                const at = performance.now();
                const target = domikAtPoint(ghostPoint.current.x, ghostPoint.current.y);
                perfGestureFrame(performance.now() - at, hover.current !== target);
                hover.current = target;
                setHoverDomikId(target);
            });
        };

        const onUp = (event: PointerEvent) => {
            const start = origin.current;
            const moved = start != null
                && (Math.abs(event.clientX - start.x) > MOVE_THRESHOLD || Math.abs(event.clientY - start.y) > MOVE_THRESHOLD);
            if (!moved) {
                origin.current = null;
                return;
            }

            const target = domikAtPoint(event.clientX, event.clientY);
            const point = { x: event.clientX, y: event.clientY };
            const held = workerId;
            finish();
            if (target != null) {
                assignRef.current(held, target, point);
            }
        };

        const onKey = (event: KeyboardEvent) => {
            if (event.key === 'Escape') {
                finish();
            }
        };

        const onOutside = (event: PointerEvent) => {
            if (origin.current != null) {
                return;
            }

            const element = event.target instanceof Element ? event.target : null;
            if (element?.closest('[data-assign-domik], [data-assign-worker], .assign-menu') == null) {
                finish();
            }
        };

        const onSelectStart = (event: Event) => { event.preventDefault(); };

        document.addEventListener('selectstart', onSelectStart);
        window.addEventListener('pointermove', onMove, { passive: false });
        window.addEventListener('blur', finish);
        window.addEventListener('pointerup', onUp);
        window.addEventListener('pointercancel', finish);
        window.addEventListener('keydown', onKey);
        document.addEventListener('pointerdown', onOutside);
        document.body.classList.add('assign-active');
        return () => {
            document.removeEventListener('selectstart', onSelectStart);
            window.removeEventListener('pointermove', onMove);
            window.removeEventListener('blur', finish);
            window.removeEventListener('pointerup', onUp);
            window.removeEventListener('pointercancel', finish);
            window.removeEventListener('keydown', onKey);
            document.removeEventListener('pointerdown', onOutside);
            document.body.classList.remove('assign-active');
        };
    }, [workerId, finish, paintGhost]);

    return { workerId, dragging, hoverDomikId, ghost, grab, drop, cancel: finish };
}
