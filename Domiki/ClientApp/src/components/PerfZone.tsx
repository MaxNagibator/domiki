import { Profiler } from 'react';
import type { ReactNode } from 'react';
import { perfDetailed, perfOnRender } from '../utils/perf';

interface PerfZoneProps {
    id: string;
    children: ReactNode;
}

export const PerfZone = ({ id, children }: PerfZoneProps) =>
    perfDetailed ? <Profiler id={id} onRender={perfOnRender}>{children}</Profiler> : children;
