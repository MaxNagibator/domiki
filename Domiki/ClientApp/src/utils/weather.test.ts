import { describe, expect, it } from 'vitest';
import type { SickTypeDto, WeatherPeriodDto } from '../types/api';
import { sickRiskPercent, sickTypeForWeather, weatherMark, weatherMarkSpeech } from './weather';

const period = (weatherTypeId: number, weatherName: string, logicName: string, effects: { domikTypeId: number; outputPercent: number }[]): WeatherPeriodDto => ({
    weatherTypeId,
    weatherName,
    logicName,
    startDate: '2026-08-01T00:00:00Z',
    endDate: '2026-08-01T08:00:00Z',
    effects,
});

const rain = period(2, 'Дождь', 'rain', [{ domikTypeId: 3, outputPercent: 150 }, { domikTypeId: 4, outputPercent: 75 }, { domikTypeId: 5, outputPercent: 100 }]);

describe('weatherMark', () => {
    it.each([
        [3, '+50 %', true, 'Дождь помогает: +50 % выход'],
        [4, '−25 %', false, 'Дождь мешает: −25 % выход'],
    ])('размечает постройку %i', (domikTypeId, delta, buff, title) => {
        const mark = weatherMark(rain, domikTypeId);
        expect(mark).not.toBeNull();
        expect(mark?.buff).toBe(buff);
        expect(mark?.title).toBe(title);
        expect(mark?.title).toContain(delta);
    });

    it.each([
        ['нетронутую постройку', 5],
        ['постройку без записи', 99],
    ])('не метит %s', (_case, domikTypeId) => {
        expect(weatherMark(rain, domikTypeId)).toBeNull();
    });

    it('молчит без погоды', () => {
        expect(weatherMark(null, 3)).toBeNull();
    });

    it('озвучивает направление словом, а не знаком', () => {
        const mark = weatherMark(rain, 4);
        expect(mark == null ? null : weatherMarkSpeech(mark)).toBe(', дождь мешает, выход меньше на 25 %');
    });
});

describe('sickRiskPercent', () => {
    it.each([
        [150, 15],
        [125, 8],
        [100, 0],
        [75, 0],
    ])('шанс при выходе %i%% равен %i%%', (outputPercent, expected) => {
        expect(sickRiskPercent(outputPercent)).toBe(expected);
    });
});

describe('sickTypeForWeather', () => {
    const sickTypes: SickTypeDto[] = [
        { id: 1, name: 'Простуда', logicName: 'cold', weatherTypeId: 2, cloakProtects: true },
        { id: 3, name: 'Озноб', logicName: 'chill', weatherTypeId: 4, cloakProtects: true },
    ];

    it('берёт хворь текущей погоды', () => {
        expect(sickTypeForWeather(sickTypes, 4)?.name).toBe('Озноб');
    });

    it.each([
        ['погоды без хвори', 9],
        ['отсутствующей погоды', null],
    ])('молчит для %s', (_case, weatherTypeId) => {
        expect(sickTypeForWeather(sickTypes, weatherTypeId)).toBeNull();
    });
});
