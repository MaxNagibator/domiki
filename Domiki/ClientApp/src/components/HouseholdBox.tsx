import { useState } from 'react';
import type { ReactNode } from 'react';
import ArrowUpIcon from 'pixelarticons/svg/arrow-up.svg?react';
import ChevronDownIcon from 'pixelarticons/svg/chevron-down.svg?react';
import ChevronRightIcon from 'pixelarticons/svg/chevron-right.svg?react';
import ChevronUpIcon from 'pixelarticons/svg/chevron-up.svg?react';
import HomeIcon from 'pixelarticons/svg/home.svg?react';
import RepeatIcon from 'pixelarticons/svg/repeat.svg?react';
import type { ResourceTypeDto } from '../types/api';
import type { HudBuildingRef, HudDigest } from '../utils/hud';
import { formatTimeOfDay } from '../utils/time';
import { pluralRu } from '../utils/plural';
import { ActionButton } from './ActionButton';
import { ResourceChip } from './ResourceChip';
import { AbstractSprite, DomikSprite, MechanicSprite, NeighborSprite } from './sprites';

const CHIP_LIMIT = 5;
const BLOCKED_LIMIT = 3;

interface HouseholdBoxProps {
    digest: HudDigest;
    resourceTypes: ResourceTypeDto[];
    now: number;
    onSelectDomik: (domikId: number, logicName: string) => void;
    onOpenTab: (tabKey: string) => void;
    onToggleRepeat: (manufactureId: number, next: boolean) => void;
}

interface BuildingChipsProps {
    icon: ReactNode;
    label: string;
    buildings: HudBuildingRef[];
    onSelectDomik: (domikId: number, logicName: string) => void;
}

const BuildingChips = ({ icon, label, buildings, onSelectDomik }: BuildingChipsProps) => {
    const [expanded, setExpanded] = useState(false);
    const hiddenCount = buildings.length - CHIP_LIMIT;
    const visible = expanded || hiddenCount <= 0 ? buildings : buildings.slice(0, CHIP_LIMIT);

    return (
        <div className="household-row household-row-group" data-expanded={expanded ? 'true' : 'false'}>
            {icon}
            <span className="household-row-label">
                {label}
                <span className="household-row-count">{buildings.length}</span>
            </span>
            <div className="household-chips">
                {visible.map(building => (
                    <button key={building.domikId} type="button" className="household-building-btn"
                        onClick={() => onSelectDomik(building.domikId, building.logicName)}>
                        <DomikSprite logicName={building.logicName} className="household-building-ico" aria-hidden="true" />
                        <span className="household-building-name" title={building.displayName}>{building.displayName}</span>
                    </button>
                ))}
                {hiddenCount > 0 &&
                    <button type="button" className="household-building-btn household-more-btn"
                        aria-expanded={expanded}
                        onClick={() => { setExpanded(value => !value); }}>
                        {expanded
                            ? <>свернуть <ChevronUpIcon aria-hidden="true" /></>
                            : <>ещё {hiddenCount} <ChevronDownIcon aria-hidden="true" /></>}
                    </button>
                }
            </div>
        </div>
    );
};

export const HouseholdBox = ({ digest, resourceTypes, now, onSelectDomik, onOpenTab, onToggleRepeat }: HouseholdBoxProps) => {
    const hasNowRows = digest.idleBuildings.length > 0
        || digest.blockedBuildings.length > 0
        || digest.soonestOrder != null
        || digest.expeditionsBack > 0
        || digest.workersResting > 0
        || digest.workersSick > 0
        || digest.upgradeableBuildings.length > 0;

    const handsNeeded = digest.idleBuildings.length + digest.blockedBuildings.length + digest.upgradeableBuildings.length;
    const blockedOverflow = digest.blockedBuildings.length - BLOCKED_LIMIT;

    return (
        <section className="household-panel pixel-panel">
            <header className="household-hero">
                <span className="household-hero-emblem" aria-hidden="true"><AbstractSprite logicName="elder_order" size={40} /></span>
                <div className="household-hero-text">
                    <h3 className="panel-title household-hero-title">Хозяйство</h3>
                    <p className="household-hero-sub">Здесь видно, что в деревне просит рук – а дела правятся на дворе.</p>
                </div>
                <div className="household-hero-stat" data-calm={handsNeeded === 0 ? 'true' : 'false'} title="Построек, что просят рук">
                    <span className="household-hero-stat-num">{handsNeeded}</span>
                    <span className="household-hero-stat-label">дел на дворе</span>
                </div>
            </header>

            <div className="household-block">
                <span className="panel-label">Сейчас</span>
                {!hasNowRows &&
                    <p className="household-empty">В деревне всё при деле – староста доволен.</p>
                }
                {hasNowRows &&
                    <div className="household-rows">
                        {digest.idleBuildings.length > 0 &&
                            <BuildingChips icon={<HomeIcon className="household-row-ico" aria-hidden="true" />} label="В простое"
                                buildings={digest.idleBuildings} onSelectDomik={onSelectDomik} />
                        }

                        {digest.blockedBuildings.slice(0, BLOCKED_LIMIT).map(building => {
                            const missingNames = building.missing
                                .map(item => resourceTypes.find(type => type.id === item.typeId)?.name)
                                .filter((name): name is string => name != null)
                                .join(', ');
                            return (
                                <button key={building.domikId} type="button" className="household-row household-row-button"
                                    aria-label={`${building.displayName} стоит: не хватает ${missingNames}`}
                                    onClick={() => onSelectDomik(building.domikId, building.logicName)}>
                                    <DomikSprite logicName={building.logicName} className="household-row-ico" aria-hidden="true" />
                                    <span className="household-row-text">{building.displayName} стоит: <span className="household-row-lack">не хватает</span></span>
                                    <span className="household-row-chips">
                                        {building.missing.map(item => {
                                            const resourceType = resourceTypes.find(type => type.id === item.typeId);
                                            return resourceType == null ? null : <ResourceChip key={item.typeId} resourceType={resourceType} value={item.value} />;
                                        })}
                                    </span>
                                    <ChevronRightIcon className="household-row-go" aria-hidden="true" />
                                </button>
                            );
                        })}

                        {blockedOverflow > 0 &&
                            <div className="household-row household-more-btn">
                                ещё {blockedOverflow} построек стоят без припасов
                            </div>
                        }

                        {digest.soonestOrder != null &&
                            <button type="button" className="household-row household-row-button"
                                onClick={() => onOpenTab('orders')}>
                                <NeighborSprite logicName={digest.soonestOrder.neighborLogicName} size={24} className="household-row-ico" aria-hidden="true" />
                                <span className="household-row-text">
                                    Заказ {digest.soonestOrder.neighborName} истекает {digest.soonestOrder.hours <= 1
                                        ? <span className="household-row-urgent">через {digest.soonestOrder.hours} ч</span>
                                        : `через ${digest.soonestOrder.hours} ч`}
                                </span>
                                <ChevronRightIcon className="household-row-go" aria-hidden="true" />
                            </button>
                        }

                        {digest.expeditionsBack > 0 &&
                            <button type="button" className="household-row household-row-button"
                                onClick={() => onOpenTab('expeditions')}>
                                <MechanicSprite logicName="expeditions" size={24} className="household-row-ico" aria-hidden="true" />
                                <span className="household-row-text">Поход вернулся: добыча не разобрана</span>
                                <ChevronRightIcon className="household-row-go" aria-hidden="true" />
                            </button>
                        }

                        {digest.workersResting > 0 && digest.restingEarliest != null &&
                            <div className="household-row">
                                <AbstractSprite logicName="fatigue_rest" size={24} className="household-row-ico" aria-hidden="true" />
                                <span className="household-row-text">
                                    {pluralRu(digest.workersResting, 'Отдыхает', 'Отдыхают', 'Отдыхают')} {digest.workersResting} {pluralRu(digest.workersResting, 'трудяга', 'трудяги', 'трудяг')}, первый встанет в {formatTimeOfDay(digest.restingEarliest, now)}
                                </span>
                            </div>
                        }

                        {digest.workersSick > 0 && digest.sickEarliest != null &&
                            <div className="household-row">
                                <AbstractSprite logicName="fatigue_rest" size={24} className="household-row-ico" aria-hidden="true" />
                                <span className="household-row-text">
                                    {pluralRu(digest.workersSick, 'Хворает', 'Хворают', 'Хворают')} {digest.workersSick} {pluralRu(digest.workersSick, 'трудяга', 'трудяги', 'трудяг')}, первый поправится в {formatTimeOfDay(digest.sickEarliest, now)}
                                </span>
                            </div>
                        }

                        {digest.upgradeableBuildings.length > 0 &&
                            <BuildingChips icon={<ArrowUpIcon className="household-row-ico" aria-hidden="true" />} label="Готовы к улучшению"
                                buildings={digest.upgradeableBuildings} onSelectDomik={onSelectDomik} />
                        }
                    </div>
                }
            </div>

            <div className="household-block">
                <span className="panel-label">Наряды</span>
                {digest.standingShifts.length === 0 &&
                    <p className="household-empty">Нарядов не поставлено – каждая смена запускается вручную.</p>
                }
                {digest.standingShifts.length > 0 &&
                    <div className="household-rows">
                        {digest.standingShifts.map(shift => (
                            <div key={shift.manufactureId} className="household-row household-shift-row">
                                <RepeatIcon className="household-row-ico" aria-hidden="true" />
                                <span className="household-row-text">Наряд: {shift.receiptName} · {shift.domikName} · снова в {formatTimeOfDay(shift.finishDate, now)}</span>
                                <ActionButton className="btn-game btn-ghost household-shift-action"
                                    onClick={() => onToggleRepeat(shift.manufactureId, false)}>
                                    Снять наряд
                                </ActionButton>
                            </div>
                        ))}
                    </div>
                }
            </div>
        </section>
    );
};
