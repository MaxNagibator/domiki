import type { ReactNode } from 'react';
import ClockIcon from 'pixelarticons/svg/clock.svg?react';
import HomeIcon from 'pixelarticons/svg/home.svg?react';
import type { HudDigest } from '../utils/hud';
import { pluralRu } from '../utils/plural';
import { MechanicSprite, NeighborSprite } from './sprites';

interface HudRibbonProps {
    digest: HudDigest;
    onOpenHousehold: () => void;
}

const RIBBON_LIMIT = 2;

export const HudRibbon = ({ digest, onOpenHousehold }: HudRibbonProps) => {
    const items: { key: string; text: string; node: ReactNode }[] = [];

    if (digest.soonestOrder != null) {
        items.push({
            key: 'order',
            text: `заказ ${digest.soonestOrder.neighborName}, ${digest.soonestOrder.hours}ч`,
            node: (
                <span className="hud-ribbon-item hud-ribbon-order"
                    title={`Ближайший заказ истекает через ${digest.soonestOrder.hours} ч`}>
                    <NeighborSprite logicName={digest.soonestOrder.neighborLogicName} size={24} className="hud-ribbon-ico" aria-hidden="true" />
                    заказ {digest.soonestOrder.neighborName}
                    <ClockIcon className="hud-ribbon-clock" aria-hidden="true" />{digest.soonestOrder.hours}ч
                </span>
            ),
        });
    }

    if (digest.expeditionsBack > 0) {
        items.push({
            key: 'expeditions',
            text: digest.expeditionsBack === 1 ? 'поход вернулся' : `${digest.expeditionsBack} похода вернулись`,
            node: (
                <span className="hud-ribbon-item" title="Поход вернулся – загляните за добычей">
                    <MechanicSprite logicName="expeditions" size={24} className="hud-ribbon-ico" aria-hidden="true" />
                    {digest.expeditionsBack === 1
                        ? 'поход вернулся'
                        : `${digest.expeditionsBack} ${pluralRu(digest.expeditionsBack, 'поход', 'похода', 'походов')} вернулись`}
                </span>
            ),
        });
    }

    if (digest.idleDomiks > 0 && digest.workersFree > 0) {
        items.push({
            key: 'idle',
            text: `${digest.idleDomiks} ${pluralRu(digest.idleDomiks, 'домик', 'домика', 'домиков')} в простое`,
            node: (
                <span className="hud-ribbon-item"
                    title="Домики без запущенного производства – докиньте входы и запустите смену">
                    <HomeIcon className="hud-ribbon-ico" aria-hidden="true" />
                    {digest.idleDomiks} {pluralRu(digest.idleDomiks, 'домик', 'домика', 'домиков')} в простое
                </span>
            ),
        });
    }

    if (items.length === 0) {
        return null;
    }

    const shown = items.slice(0, RIBBON_LIMIT);
    const hidden = items.slice(RIBBON_LIMIT);

    return (
        <div className="hud-ribbon">
            {shown.map(item => <span key={item.key} className="hud-ribbon-slot">{item.node}</span>)}
            {hidden.length > 0 &&
                <button type="button" className="hud-ribbon-more" title={hidden.map(item => item.text).join(', ')}
                    onClick={onOpenHousehold}>
                    +{hidden.length}
                </button>}
        </div>
    );
};
