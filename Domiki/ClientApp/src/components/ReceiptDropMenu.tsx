import { useEffect, useLayoutEffect, useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import type { ResourceTypeDto, WorkerDto } from '../types/api';
import type { AssignTarget } from '../utils/assign';
import { workerSkillPercent } from '../utils/assign';
import { formatDuration } from '../utils/time';
import { ResourceChip } from './ResourceChip';

const MENU_WIDTH = 288;
const EDGE_GAP = 12;

interface ReceiptDropMenuProps {
    target: AssignTarget;
    domikName: string;
    domikTypeId: number;
    worker: WorkerDto;
    point: { x: number; y: number };
    resourceTypes: ResourceTypeDto[];
    onPick: (receiptId: number, workerIds: number[]) => void;
    onClose: () => void;
}

export const ReceiptDropMenu = ({ target, domikName, domikTypeId, worker, point, resourceTypes, onPick, onClose }: ReceiptDropMenuProps) => {
    const menuRef = useRef<HTMLDivElement>(null);
    const [position, setPosition] = useState({ left: point.x + 8, top: point.y + 8 });
    const bonus = workerSkillPercent(worker, domikTypeId);

    useLayoutEffect(() => {
        const menu = menuRef.current;
        if (menu == null) {
            return;
        }

        const height = menu.offsetHeight;
        setPosition({
            left: Math.max(EDGE_GAP, Math.min(point.x + 8, window.innerWidth - MENU_WIDTH - EDGE_GAP)),
            top: Math.max(EDGE_GAP, Math.min(point.y + 8, window.innerHeight - height - EDGE_GAP)),
        });
        menu.querySelector<HTMLButtonElement>('.assign-menu-option:not(:disabled)')?.focus();
    }, [point.x, point.y]);

    useEffect(() => {
        const onKey = (event: KeyboardEvent) => {
            if (event.key === 'Escape') {
                onClose();
            }
        };

        const onOutside = (event: PointerEvent) => {
            const element = event.target instanceof Element ? event.target : null;
            if (element?.closest('.assign-menu') == null) {
                onClose();
            }
        };

        window.addEventListener('keydown', onKey);
        document.addEventListener('pointerdown', onOutside);
        window.addEventListener('scroll', onClose, true);
        return () => {
            window.removeEventListener('keydown', onKey);
            document.removeEventListener('pointerdown', onOutside);
            window.removeEventListener('scroll', onClose, true);
        };
    }, [onClose]);

    return createPortal(
        <div className="assign-menu" ref={menuRef} role="dialog" aria-label={`Приставить к постройке «${domikName}»`}
            style={{ left: position.left, top: position.top, width: MENU_WIDTH }}>
            <div className="assign-menu-head">
                <span className="assign-menu-domik">{domikName}</span>
                <span className="assign-menu-worker">{worker.name} {bonus > 0 ? `+${bonus}` : bonus}&nbsp;%</span>
            </div>
            {target.options.map(option => {
                const shortfallText = option.shortfall
                    .map(missing => `${resourceTypes.find(type => type.id === missing.typeId)?.name ?? '?'} ${missing.value}`)
                    .join(', ');
                return (
                    <button key={option.receipt.id} type="button" disabled={!option.canRun}
                        className={'assign-menu-option' + (option.canRun ? '' : ' assign-menu-option--blocked')}
                        onClick={() => { onPick(option.receipt.id, option.crew.map(item => item.id)); }}>
                        <span className="assign-menu-row">
                            <span className="assign-menu-name">{option.receipt.name}</span>
                            <span className="assign-menu-time">{formatDuration(option.receipt.durationSeconds)}</span>
                        </span>
                        <span className="assign-menu-row assign-menu-flow">
                            {option.receipt.inputResources.map(input => {
                                const resourceType = resourceTypes.find(type => type.id === input.typeId);
                                return resourceType == null
                                    ? null
                                    : <ResourceChip key={input.typeId} resourceType={resourceType} value={input.value} />;
                            })}
                            {option.receipt.inputResources.length > 0 && <span className="assign-menu-arrow">–</span>}
                            {option.receipt.outputResources.map(output => {
                                const resourceType = resourceTypes.find(type => type.id === output.typeId);
                                return resourceType == null
                                    ? null
                                    : <ResourceChip key={output.typeId} resourceType={resourceType} value={output.value} />;
                            })}
                        </span>
                        {option.receipt.plodderCount > 1 &&
                            <span className="assign-menu-crew">
                                нужно трудяг: {option.receipt.plodderCount} – {option.crew.map(item => item.name).join(', ')}
                                {option.autoCrew.length > 0 && ' (добор)'}
                            </span>
                        }
                        {!option.canRun &&
                            <span className="assign-menu-block">{shortfallText === '' ? option.reason : `не хватает: ${shortfallText}`}</span>
                        }
                    </button>
                );
            })}
        </div>,
        document.body,
    );
};
