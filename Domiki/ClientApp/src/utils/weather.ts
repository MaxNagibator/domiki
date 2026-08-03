import type { SickTypeDto, WeatherPeriodDto } from '../types/api';

export const SICK_CHANCE_PER_BONUS_POINT = 0.3;

export interface WeatherMarkView {
    outputPercent: number;
    delta: number;
    buff: boolean;
    weatherName: string;
    weatherLogicName: string;
    title: string;
}

export function formatOutputDelta(delta: number) {
    return `${delta > 0 ? '+' : '−'}${Math.abs(delta)} %`;
}

export function weatherMark(weather: WeatherPeriodDto | null | undefined, domikTypeId: number): WeatherMarkView | null {
    const effect = weather?.effects.find(item => item.domikTypeId === domikTypeId);
    if (weather == null || effect == null || effect.outputPercent === 100) {
        return null;
    }
    const delta = effect.outputPercent - 100;
    const buff = delta > 0;
    return {
        outputPercent: effect.outputPercent,
        delta,
        buff,
        weatherName: weather.weatherName,
        weatherLogicName: weather.logicName,
        title: `${weather.weatherName} ${buff ? 'помогает' : 'мешает'}: ${formatOutputDelta(delta)} выход`,
    };
}

export function weatherMarkSpeech(mark: WeatherMarkView) {
    const direction = mark.buff ? 'помогает, выход больше на' : 'мешает, выход меньше на';
    return `, ${mark.weatherName.toLocaleLowerCase()} ${direction} ${Math.abs(mark.delta)} %`;
}

export function sickRiskPercent(outputPercent: number) {
    return outputPercent <= 100 ? 0 : Math.round((outputPercent - 100) * SICK_CHANCE_PER_BONUS_POINT);
}

export function sickTypeForWeather(sickTypes: SickTypeDto[], weatherTypeId: number | null | undefined) {
    return weatherTypeId == null ? null : sickTypes.find(sickType => sickType.weatherTypeId === weatherTypeId) ?? null;
}
