import { memo } from 'react';
import type { SVGProps } from 'react';
import WorkerPortrait from '../../assets/workers/portrait.svg?react';
import { cleanSpriteProps } from './core';

type WorkerLook = [skin: number, hair: number, style: string, beard: number, hat: number, shirt: number, extra: number];

const workerLooks: Record<string, WorkerLook> = {
    'Аким': [1, 1, 'm1', 1, 0, 1, 0],
    'Агафья': [1, 1, 'f2', 0, 0, 1, 0],
    'Бажен': [2, 2, 'm4', 0, 4, 4, 2],
    'Борис': [1, 5, 'm3', 3, 0, 2, 0],
    'Варвара': [1, 2, 'f1', 0, 0, 3, 3],
    'Велена': [2, 1, 'f4', 0, 4, 2, 2],
    'Глеб': [1, 3, 'm2', 0, 0, 3, 1],
    'Гордей': [2, 1, 'm2', 2, 0, 5, 0],
    'Дарья': [1, 3, 'f3', 0, 0, 1, 1],
    'Демьян': [1, 4, 'm4', 1, 0, 2, 0],
    'Егор': [1, 4, 'm1', 0, 1, 4, 1],
    'Аксинья': [1, 2, 'f4', 0, 0, 5, 0],
    'Ждан': [2, 1, 'm4', 0, 0, 1, 0],
    'Захар': [1, 2, 'm1', 2, 0, 3, 0],
    'Злата': [1, 3, 'f1', 0, 0, 4, 0],
    'Илья': [1, 1, 'm2', 0, 1, 2, 0],
    'Кира': [2, 1, 'f2', 0, 4, 3, 0],
    'Лада': [1, 2, 'f3', 0, 4, 2, 3],
    'Лукерья': [1, 5, 'f2', 0, 0, 5, 0],
    'Марфа': [1, 4, 'f2', 0, 0, 2, 1],
    'Мирон': [2, 5, 'm4', 1, 0, 4, 0],
    'Назар': [2, 1, 'm1', 2, 1, 5, 0],
    'Нина': [2, 1, 'f1', 0, 0, 5, 0],
    'Остап': [1, 3, 'm3', 1, 0, 5, 0],
    'Пелагея': [1, 5, 'f2', 0, 3, 4, 0],
    'Прасковья': [2, 5, 'f1', 0, 3, 2, 0],
    'Роман': [1, 2, 'm2', 0, 2, 1, 0],
    'Савва': [1, 3, 'm4', 0, 0, 2, 3],
    'Тая': [1, 1, 'f3', 0, 0, 4, 3],
    'Ульяна': [1, 4, 'f1', 0, 4, 3, 0],
    'Фёдор': [1, 5, 'm1', 2, 1, 3, 0],
    'Ярина': [2, 2, 'f4', 0, 0, 1, 2],
    'Авдотья': [2, 3, 'f1', 0, 0, 2, 1],
    'Архип': [1, 2, 'm3', 2, 0, 4, 0],
    'Василиса': [1, 4, 'f3', 0, 4, 3, 2],
    'Влас': [2, 1, 'm2', 3, 0, 1, 0],
    'Глафира': [1, 5, 'f2', 0, 3, 5, 0],
    'Гаврила': [1, 3, 'm4', 1, 1, 2, 0],
    'Домна': [2, 2, 'f4', 0, 0, 4, 3],
    'Данила': [1, 4, 'm1', 0, 2, 3, 1],
    'Ефросинья': [1, 1, 'f1', 0, 4, 2, 0],
    'Кузьма': [2, 5, 'm2', 2, 0, 5, 0],
    'Забава': [1, 3, 'f3', 0, 4, 1, 3],
    'Лукьян': [1, 2, 'm4', 1, 0, 4, 2],
    'Любава': [2, 4, 'f2', 0, 0, 3, 1],
    'Макар': [1, 5, 'm1', 3, 1, 2, 0],
    'Матрёна': [1, 2, 'f4', 0, 3, 5, 0],
    'Пахом': [2, 1, 'm3', 2, 0, 3, 0],
    'Настасья': [1, 4, 'f1', 0, 4, 4, 2],
    'Прохор': [1, 3, 'm2', 1, 0, 1, 0],
    'Олеся': [2, 5, 'f3', 0, 0, 2, 3],
    'Тихон': [1, 1, 'm4', 3, 2, 5, 0],
    'Устинья': [1, 2, 'f2', 0, 3, 3, 0],
    'Трофим': [2, 4, 'm1', 2, 0, 4, 1],
    'Фёкла': [1, 5, 'f4', 0, 0, 1, 2],
    'Фрол': [1, 3, 'm3', 1, 1, 2, 0],
};

const fallbackLook = (name: string): WorkerLook => {
    let hash = 0;
    for (let i = 0; i < name.length; i++) {
        hash = (hash * 31 + name.charCodeAt(i)) | 0;
    }
    hash = Math.abs(hash);
    return [1 + hash % 2, 1 + Math.floor(hash / 2) % 5, Math.floor(hash / 16) % 2 === 0 ? 'm1' : 'm4', 0, 0, 1 + Math.floor(hash / 32) % 5, 0];
};

interface WorkerSpriteProps extends SVGProps<SVGSVGElement> {
    name: string;
    state?: 'idle' | 'working' | 'resting' | 'sick';
    skilled?: boolean;
}

export const WorkerSprite = memo(({ name, state = 'idle', skilled = false, ...props }: WorkerSpriteProps) => {
    const [skin, hair, style, beard, hat, shirt, extra] = workerLooks[name] ?? fallbackLook(name);
    return (
        <WorkerPortrait data-skin={skin} data-hair={hair} data-style={style} data-state={state}
            data-skilled={skilled ? 'true' : 'false'}
            data-beard={beard} data-hat={hat} data-shirt={shirt} data-extra={extra} {...cleanSpriteProps(props)} />
    );
});
