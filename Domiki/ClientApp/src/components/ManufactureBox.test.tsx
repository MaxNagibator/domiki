import { fireEvent, render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import type { ManufactureDto, ReceiptDto } from '../types/api';
import { ManufactureBox } from './ManufactureBox';

const manufacture: ManufactureDto = {
    id: 17,
    finishDate: '2026-07-13T12:30:00.000Z',
    durationSeconds: 3600,
    plodderCount: 2,
    receiptId: 4,
    autoRepeat: true,
};

const receipt = {
    id: 4,
    name: 'Обжечь кирпич',
    durationSeconds: 3600,
} as ReceiptDto;

const renderBox = (value: ManufactureDto, onToggle = vi.fn()) => {
    render(<ManufactureBox manufacture={value} receipt={receipt} now={Date.parse(value.finishDate) - 1000}
        remainingText="1 с" goldValue={0} onHurry={vi.fn()} onToggleAutoRepeat={onToggle} />);
    return onToggle;
};

describe('ManufactureBox наряд controls', () => {
    it('explains the standing наряд and lets the player lift it', () => {
        const onToggle = renderBox(manufacture);

        expect(screen.getByText('Наряд поставлен')).toBeInTheDocument();
        expect(screen.queryByText(/снова возьмутся за «Обжечь кирпич»/)).not.toBeInTheDocument();

        fireEvent.click(screen.getByRole('button', { name: 'Наряд поставлен' }));
        expect(screen.getByText(/снова возьмутся за «Обжечь кирпич»/)).toBeInTheDocument();
        expect(screen.getByText('Текущая смена завершится как обычно')).toBeInTheDocument();

        fireEvent.click(screen.getByRole('button', { name: 'Снять наряд' }));
        expect(onToggle).toHaveBeenCalledWith(17, false);
    });

    it('lets the player put a наряд on the current shift', () => {
        const onToggle = renderBox({ ...manufacture, autoRepeat: false });

        expect(screen.getByText('Наряда нет')).toBeInTheDocument();
        fireEvent.click(screen.getByRole('button', { name: 'Наряда нет' }));
        fireEvent.click(screen.getByRole('button', { name: 'Поставить наряд' }));
        expect(onToggle).toHaveBeenCalledWith(17, true);
    });
});
