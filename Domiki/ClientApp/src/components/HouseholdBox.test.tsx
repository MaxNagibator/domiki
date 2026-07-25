import { fireEvent, render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import type { ResourceTypeDto } from '../types/api';
import type { HudDigest } from '../utils/hud';
import { HouseholdBox } from './HouseholdBox';

const resourceTypes: ResourceTypeDto[] = [
    { id: 200, name: 'Глина', logicName: 'clay', marketValue: 1, isFood: false },
];

const emptyDigest: HudDigest = {
    idleDomiks: 0,
    soonestOrder: null,
    expeditionsBack: 0,
    workersResting: 0,
    workersSick: 0,
    idleBuildings: [],
    blockedBuildings: [],
    restingEarliest: null,
    sickEarliest: null,
    upgradeableBuildings: [],
    standingShifts: [],
};

const NOW = Date.parse('2026-07-25T12:00:00.000Z');

const renderBox = (digest: HudDigest) => {
    const onSelectDomik = vi.fn();
    const onOpenTab = vi.fn();
    const onToggleRepeat = vi.fn();
    render(<HouseholdBox digest={digest} resourceTypes={resourceTypes} now={NOW}
        onSelectDomik={onSelectDomik} onOpenTab={onOpenTab} onToggleRepeat={onToggleRepeat} />);
    return { onSelectDomik, onOpenTab, onToggleRepeat };
};

describe('HouseholdBox empty states', () => {
    it('renders the calm line when nothing needs attention and no наряды stand', () => {
        renderBox(emptyDigest);

        expect(screen.getByText('В деревне всё при деле – староста доволен.')).toBeInTheDocument();
        expect(screen.getByText('Нарядов не поставлено – каждая смена запускается вручную.')).toBeInTheDocument();
    });
});

describe('HouseholdBox idle building row', () => {
    it('renders the building and navigates to it without starting any work', () => {
        const digest: HudDigest = {
            ...emptyDigest,
            idleBuildings: [{ domikId: 7, typeId: 10, logicName: 'forge', displayName: 'Кузница', level: 1 }],
        };
        const { onSelectDomik, onToggleRepeat } = renderBox(digest);

        fireEvent.click(screen.getByRole('button', { name: /Кузница/ }));

        expect(onSelectDomik).toHaveBeenCalledWith(7, 'forge');
        expect(onToggleRepeat).not.toHaveBeenCalled();
    });
});

describe('HouseholdBox idle building chip expander', () => {
    it('shows 5 chips by default and reveals the rest via «ещё N», toggling aria-expanded', () => {
        const idleBuildings = Array.from({ length: 7 }, (_, i) => ({
            domikId: i + 1, typeId: 10, logicName: 'forge', displayName: `Кузница ${i + 1}`, level: 1,
        }));
        const digest: HudDigest = { ...emptyDigest, idleBuildings };
        renderBox(digest);

        expect(screen.getAllByRole('button', { name: /^Кузница \d$/ })).toHaveLength(5);
        const moreButton = screen.getByRole('button', { name: /ещё 2/ });
        expect(moreButton).toHaveAttribute('aria-expanded', 'false');

        fireEvent.click(moreButton);

        expect(screen.getAllByRole('button', { name: /^Кузница \d$/ })).toHaveLength(7);
        expect(screen.getByRole('button', { name: /свернуть/ })).toHaveAttribute('aria-expanded', 'true');
    });
});

describe('HouseholdBox blocked building row', () => {
    it('names the missing resource and still navigates on click without lifting any наряд', () => {
        const digest: HudDigest = {
            ...emptyDigest,
            blockedBuildings: [{ domikId: 3, typeId: 10, logicName: 'forge', displayName: 'Гончарня', level: 1, missing: [{ typeId: 200, value: 4 }] }],
        };
        const { onSelectDomik, onToggleRepeat } = renderBox(digest);

        expect(screen.getByText(/Гончарня стоит:/)).toBeInTheDocument();
        expect(screen.getByText('не хватает')).toBeInTheDocument();

        fireEvent.click(screen.getByRole('button', { name: /Гончарня стоит/ }));
        expect(onSelectDomik).toHaveBeenCalledWith(3, 'forge');
        expect(onToggleRepeat).not.toHaveBeenCalled();
    });
});

describe('HouseholdBox blocked building cap', () => {
    it('caps blocked rows at 3 and shows a muted overflow line for the rest', () => {
        const blockedBuildings = Array.from({ length: 5 }, (_, i) => ({
            domikId: i + 1, typeId: 10, logicName: 'forge', displayName: `Домик ${i + 1}`, level: 1,
            missing: [{ typeId: 200, value: 1 }],
        }));
        const digest: HudDigest = { ...emptyDigest, blockedBuildings };
        renderBox(digest);

        expect(screen.getAllByRole('button', { name: /стоит: не хватает/ })).toHaveLength(3);
        expect(screen.getByText('ещё 2 построек стоят без припасов')).toBeInTheDocument();
    });
});

describe('HouseholdBox наряды block', () => {
    it('lets the player lift a standing наряд', () => {
        const digest: HudDigest = {
            ...emptyDigest,
            standingShifts: [{
                manufactureId: 5, domikId: 3, domikName: 'Гончарня',
                receiptId: 2, receiptName: 'Обжечь кирпич', finishDate: '2026-07-25T15:40:00.000Z',
            }],
        };
        const { onToggleRepeat } = renderBox(digest);

        expect(screen.getByText(/Наряд: Обжечь кирпич · Гончарня · снова в/)).toBeInTheDocument();

        fireEvent.click(screen.getByRole('button', { name: 'Снять наряд' }));
        expect(onToggleRepeat).toHaveBeenCalledWith(5, false);
    });
});
