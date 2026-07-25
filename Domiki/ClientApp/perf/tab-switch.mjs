// Замер переключения нижних вкладок деревни. Приложение должно быть поднято
// (dotnet run --project Domiki), вход – демо-учётка. Запуск из Domiki/ClientApp:
//   npm run perf              сверка с perf/baseline.json
//   npm run perf:update       перезапись базы
// Переменные: PERF_BASE (https://localhost:44444), PERF_CHROME (путь к chrome).

import { existsSync, readFileSync, writeFileSync } from 'node:fs';
import { fileURLToPath } from 'node:url';
import puppeteer from 'puppeteer-core';

const BASE = process.env.PERF_BASE ?? 'https://localhost:44444';
const BASELINE = fileURLToPath(new URL('./baseline.json', import.meta.url));
const ROUNDS = 5;
const COUNT_TOLERANCE = 1.05;
const TIME_TOLERANCE = 1.6;
// узлов и спрайтов на экране зависит от состояния деревни, эти счётчики только для глаз
const GATED = ['styleSheets', 'cssRules', 'spriteStyles'];

const update = process.argv.includes('--update');
const sleep = ms => new Promise(resolve => setTimeout(resolve, ms));
const best = values => Math.min(...values);

const chrome = [
    process.env.PERF_CHROME,
    'C:/Program Files/Google/Chrome/Application/chrome.exe',
    'C:/Program Files (x86)/Google/Chrome/Application/chrome.exe',
    '/usr/bin/google-chrome',
    '/usr/bin/chromium',
].filter(path => path != null).find(path => existsSync(path));

if (chrome == null) {
    console.error('Chrome не найден – задай путь через PERF_CHROME.');
    process.exit(2);
}

const browser = await puppeteer.launch({
    executablePath: chrome, headless: 'new',
    args: ['--ignore-certificate-errors', '--no-sandbox', '--window-size=1500,1000'],
    defaultViewport: { width: 1440, height: 940 },
});

let report;
try {
    const page = await browser.newPage();
    await page.goto(`${BASE}/`, { waitUntil: 'domcontentloaded', timeout: 30000 });
    const login = await page.evaluate(() => fetch('/authentication/demo', { method: 'POST', credentials: 'same-origin' }).then(response => response.status));
    if (login !== 200) {
        throw new Error(`демо-вход вернул ${login} – проверь секцию Demo в конфиге`);
    }

    await page.goto(`${BASE}/domiki-page`, { waitUntil: 'networkidle2', timeout: 40000 });
    await page.waitForSelector('[data-game-tab]', { timeout: 20000 });
    await sleep(2500);

    const document = await page.evaluate(() => {
        let rules = 0;
        for (const sheet of window.document.styleSheets) {
            try {
                rules += sheet.cssRules.length;
            } catch {
                // лист с другого источника – правила недоступны
            }
        }
        return {
            styleSheets: window.document.styleSheets.length,
            cssRules: rules,
            spriteStyles: window.document.querySelectorAll('svg style').length,
            nodes: window.document.querySelectorAll('*').length,
            sprites: window.document.querySelectorAll('svg').length,
        };
    });

    const keys = await page.evaluate(() => [...window.document.querySelectorAll('[data-game-tab]')].map(button => button.dataset.gameTab));
    const samples = new Map(keys.map(key => [key, []]));
    for (let round = 0; round < ROUNDS; round++) {
        for (const key of keys) {
            const ms = await page.evaluate(async key => {
                const afterPaint = () => new Promise(resolve => requestAnimationFrame(() => requestAnimationFrame(() => { resolve(performance.now()); })));
                await afterPaint();
                const started = performance.now();
                window.document.querySelector(`[data-game-tab="${key}"]`).click();
                return Math.round(await afterPaint() - started);
            }, key);
            samples.get(key).push(ms);
            await sleep(250);
        }
    }

    report = { document, tabs: Object.fromEntries([...samples].map(([key, values]) => [key, best(values)])) };
} finally {
    await browser.close();
}

if (update || !existsSync(BASELINE)) {
    writeFileSync(BASELINE, `${JSON.stringify(report, null, 4)}\n`, 'utf8');
    console.log(`база записана: ${BASELINE}`);
    console.log(JSON.stringify(report, null, 4));
    process.exit(0);
}

const baseline = JSON.parse(readFileSync(BASELINE, 'utf8'));
const failures = [];

console.log('счётчики документа:');
for (const [name, value] of Object.entries(report.document)) {
    const was = baseline.document[name];
    const gated = GATED.includes(name);
    const limit = was === 0 ? 0 : Math.max(Math.round(was * COUNT_TOLERANCE), was + 5);
    const bad = gated && was != null && value > limit;
    const note = gated ? `  предел ${String(limit).padStart(6)}` : '  справочно';
    console.log(`  ${name.padEnd(13)} ${String(value).padStart(6)}  база ${String(was ?? '–').padStart(6)}${note}${bad ? '  ПРЕВЫШЕН' : ''}`);
    if (bad) {
        failures.push(`${name}: ${value} против ${was} в базе`);
    }
}

console.log('\nпереключение вкладок, лучшее из прогонов, мс (справочно):');
for (const [key, ms] of Object.entries(report.tabs)) {
    const was = baseline.tabs[key];
    const slow = was != null && ms > was * TIME_TOLERANCE && ms - was > 50;
    console.log(`  ${key.padEnd(13)} ${String(ms).padStart(6)}  база ${String(was ?? '–').padStart(6)}${slow ? '  замедление' : ''}`);
}

if (failures.length > 0) {
    console.error(`\nбюджет превышен:\n  ${failures.join('\n  ')}`);
    console.error('обычно это значит, что в документ снова попали стили спрайтов – смотри hoistSpriteStyles в src/utils/inlineSprite.ts');
    process.exit(1);
}

console.log('\nбюджет соблюдён');
