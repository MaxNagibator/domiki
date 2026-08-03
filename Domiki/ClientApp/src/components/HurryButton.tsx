import type { ResourceTypeDto } from '../types/api';
import { canInstaFinish, instaFinishCost } from '../utils/game';
import { ActionButton } from './ActionButton';
import { AbstractSprite, ResourceSprite } from './sprites';

interface HurryButtonProps {
    finishDate: string;
    now: number;
    goldValue: number;
    goldType?: ResourceTypeDto | undefined;
    remainingText: string;
    onHurry: () => void;
}

export const HurryButton = ({ finishDate, now, goldValue, goldType, remainingText, onHurry }: HurryButtonProps) => {
    const hurryCost = instaFinishCost(finishDate, now);
    const shownCost = Math.max(1, hurryCost);
    const tooFar = !canInstaFinish(finishDate, now);
    const notEnoughGold = goldValue < hurryCost;
    const hurryTitle = tooFar
        ? `До конца ${remainingText}; ускорение доступно в последние 6 ч`
        : notEnoughGold ? `Не хватает золота: ${hurryCost - goldValue}` : undefined;

    return (
        <ActionButton className="btn-game hurry-btn"
            disabled={tooFar || notEnoughGold}
            title={hurryTitle}
            onClick={onHurry}>
            <span className="hurry-btn-cell hurry-btn-ico">
                <AbstractSprite logicName="hurry" size={24} aria-hidden="true" />
            </span>
            <span className="hurry-btn-label">Поторопить</span>
            <span className={'hurry-btn-cell hurry-btn-price' + (notEnoughGold ? ' hurry-btn-price--short' : '')}
                aria-label={`цена ${shownCost}`}>
                {goldType != null && <ResourceSprite logicName={goldType.logicName} size={24} aria-hidden="true" />}
                {shownCost}
            </span>
        </ActionButton>
    );
};
