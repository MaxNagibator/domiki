import WeatherClearSprite from '../../assets/weather/clear.svg?react';
import WeatherRainSprite from '../../assets/weather/rain.svg?react';
import WeatherDroughtSprite from '../../assets/weather/drought.svg?react';
import WeatherFairDaySprite from '../../assets/weather/fair_day.svg?react';
import WeatherFrostSprite from '../../assets/weather/frost.svg?react';
import WeatherWindSprite from '../../assets/weather/wind.svg?react';
import { renderIconSprite } from './core';
import type { IconSpriteProps, SpriteComponent } from './core';

const weatherSprites: Record<string, SpriteComponent> = {
    clear: WeatherClearSprite,
    rain: WeatherRainSprite,
    drought: WeatherDroughtSprite,
    frost: WeatherFrostSprite,
    wind: WeatherWindSprite,
    fair_day: WeatherFairDaySprite,
};

export const WeatherSprite = (props: IconSpriteProps) => <>{renderIconSprite('weather', weatherSprites, undefined, props)}</>;
