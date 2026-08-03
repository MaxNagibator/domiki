import type { ProfilerOnRenderCallback } from 'react';

const SLOW_FRAME_MS = 60;
const MIN_FRAMES_TO_JUDGE = 10;

export const perfWatch = import.meta.env.DEV;

export const perfDetailed = perfWatch
    && (new URLSearchParams(window.location.search).has('perf') || localStorage.getItem('perf') === '1');

interface Bucket {
    count: number;
    total: number;
    max: number;
}

interface Session {
    durationMs: number;
    moves: number;
    frames: number;
    hoverChanges: number;
    hitTest: Bucket;
    frameGap: Bucket;
    longTasks: Bucket;
    render: Bucket;
    styleAndLayout: Bucket;
    scripts: Record<string, Bucket>;
    zones: Record<string, Bucket>;
}

interface LoafScript {
    duration: number;
    invoker?: string;
    invokerType?: string;
    sourceURL?: string;
    sourceFunctionName?: string;
}

interface LoafEntry extends PerformanceEntry {
    renderStart?: number;
    styleAndLayoutStart?: number;
    scripts?: LoafScript[];
}

interface PerfStore {
    sessions: Session[];
    report: () => string;
    enable: () => string;
    disable: () => string;
}

declare global {
    interface Window {
        __perf?: PerfStore;
    }
}

function bucket(): Bucket {
    return { count: 0, total: 0, max: 0 };
}

function add(target: Bucket, value: number): void {
    target.count += 1;
    target.total += value;
    target.max = Math.max(target.max, value);
}

function line(name: string, item: Bucket): string {
    return item.count === 0
        ? `  ${name}: –`
        : `  ${name}: ${String(item.count)} шт, сумма ${item.total.toFixed(0)} мс, сред ${(item.total / item.count).toFixed(1)} мс, макс ${item.max.toFixed(1)} мс`;
}

function fps(session: Session): number {
    return session.frameGap.count === 0 ? 0 : Math.round(1000 / (session.frameGap.total / session.frameGap.count));
}

function describe(session: Session, index: number): string {
    const head = `Жест ${String(index + 1)}: ${(session.durationMs / 1000).toFixed(1)} с, ${String(session.frames)} кадров (${String(fps(session))} к/с), ${String(session.moves)} move, ${String(session.hoverChanges)} смен цели`;
    if (!perfDetailed) {
        return `${head}\n${line('промежуток кадров', session.frameGap)}\n  подробности: включи window.__perf.enable() и перезагрузи`;
    }

    const sorted = (map: Record<string, Bucket>, limit: number) => Object.entries(map)
        .sort((a, b) => b[1].total - a[1].total)
        .slice(0, limit)
        .map(([id, item]) => line(id, item));
    return [
        head,
        line('промежуток кадров', session.frameGap),
        line('поиск цели', session.hitTest),
        line('длинные задачи', session.longTasks),
        line('отрисовка кадра', session.render),
        line('стили и раскладка', session.styleAndLayout),
        ...sorted(session.scripts, 5),
        ...sorted(session.zones, 12),
    ].join('\n');
}

let current: Session | null = null;
let startedAt = 0;
let lastFrameAt = 0;
let lastCommitAt = 0;
let observer: PerformanceObserver | null = null;

const store: PerfStore = {
    sessions: [],
    report: () => store.sessions.length === 0
        ? 'Замеров нет – потаскай трудягу по постройкам.'
        : store.sessions.map(describe).join('\n\n'),
    enable: () => {
        localStorage.setItem('perf', '1');
        return 'подробные замеры включены, перезагрузи страницу';
    },
    disable: () => {
        localStorage.removeItem('perf');
        return 'подробные замеры выключены, перезагрузи страницу';
    },
};

export function perfGestureStart(): void {
    if (!perfWatch) {
        return;
    }

    current = {
        durationMs: 0,
        moves: 0,
        frames: 0,
        hoverChanges: 0,
        hitTest: bucket(),
        frameGap: bucket(),
        longTasks: bucket(),
        render: bucket(),
        styleAndLayout: bucket(),
        scripts: {},
        zones: {},
    };
    startedAt = performance.now();
    lastFrameAt = 0;
    window.__perf = store;
    if (!perfDetailed) {
        return;
    }

    observer = new PerformanceObserver(list => {
        for (const entry of list.getEntries() as LoafEntry[]) {
            const session = current;
            if (session == null) {
                continue;
            }

            add(session.longTasks, entry.duration);
            const end = entry.startTime + entry.duration;
            if (entry.renderStart != null && entry.renderStart > 0) {
                add(session.render, end - entry.renderStart);
            }

            if (entry.styleAndLayoutStart != null && entry.styleAndLayoutStart > 0) {
                add(session.styleAndLayout, end - entry.styleAndLayoutStart);
            }

            for (const script of entry.scripts ?? []) {
                const name = `скрипт ${script.invoker ?? script.sourceFunctionName ?? script.sourceURL ?? '?'}`.slice(0, 80);
                add(session.scripts[name] ??= bucket(), script.duration);
            }
        }
    });
    observer.observe({ type: 'long-animation-frame', buffered: false });
}

export function perfGestureMove(): void {
    if (current != null) {
        current.moves += 1;
    }
}

export function perfGestureFrame(hitTestMs: number, hoverChanged: boolean): void {
    if (current == null) {
        return;
    }

    const at = performance.now();
    current.frames += 1;
    add(current.hitTest, hitTestMs);
    if (lastFrameAt !== 0) {
        add(current.frameGap, at - lastFrameAt);
    }

    lastFrameAt = at;
    if (hoverChanged) {
        current.hoverChanges += 1;
    }
}

export function perfGestureEnd(): void {
    if (current == null) {
        return;
    }

    observer?.disconnect();
    observer = null;
    current.durationMs = performance.now() - startedAt;
    const finished = current;
    current = null;
    if (finished.moves === 0) {
        return;
    }

    store.sessions.push(finished);
    window.__perf = store;
    const rate = fps(finished);
    const slow = finished.frameGap.count >= MIN_FRAMES_TO_JUDGE
        && finished.frameGap.total / finished.frameGap.count > SLOW_FRAME_MS;
    if (slow) {
        console.warn(`Перетаскивание идёт с ${String(rate)} к/с – похоже на просадку.\n${describe(finished, store.sessions.length - 1)}`);
        return;
    }

    if (perfDetailed) {
        console.info(describe(finished, store.sessions.length - 1));
    }
}

export const perfOnRender: ProfilerOnRenderCallback = (id, _phase, actualDuration, _base, _start, commitTime) => {
    if (current == null) {
        return;
    }

    lastCommitAt = Math.max(lastCommitAt, commitTime);
    add(current.zones[id] ??= bucket(), actualDuration);
};

export function perfCommitProbe(): void {
    if (current != null && perfDetailed && lastCommitAt !== 0) {
        add(current.zones['коммит → эффекты'] ??= bucket(), performance.now() - lastCommitAt);
    }
}
