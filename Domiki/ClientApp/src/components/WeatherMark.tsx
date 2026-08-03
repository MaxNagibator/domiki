import type { WeatherMarkView } from '../utils/weather';
import { formatOutputDelta } from '../utils/weather';
import { WeatherSprite } from './sprites';

interface WeatherMarkProps {
    mark: WeatherMarkView;
    full?: boolean;
}

export const WeatherMark = ({ mark, full = false }: WeatherMarkProps) => (
    <span className={'weather-mark' + (mark.buff ? ' weather-mark-buff' : ' weather-mark-nerf')} title={mark.title}>
        <WeatherSprite logicName={mark.weatherLogicName} className="weather-mark-ico" size={24} aria-hidden="true" />
        <span className="weather-mark-value">{formatOutputDelta(mark.delta)}{full ? ' выход' : ''}</span>
    </span>
);
