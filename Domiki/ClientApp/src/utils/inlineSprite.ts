const hoistedSpriteCss = new Set<string>();
let spriteStyleHost: HTMLStyleElement | null = null;

// TODO: правила спрайтов вырезать из SVG на сборке (плагин Vite), пока переносим в общий лист на монтировании
const hoistSpriteStyles = (node: SVGSVGElement) => {
    node.querySelectorAll('style').forEach(element => {
        const css = element.textContent;
        element.remove();
        if (css === '' || hoistedSpriteCss.has(css)) {
            return;
        }

        hoistedSpriteCss.add(css);
        if (spriteStyleHost == null) {
            spriteStyleHost = document.createElement('style');
            spriteStyleHost.dataset.spriteStyles = '';
            document.body.appendChild(spriteStyleHost);
        }

        spriteStyleHost.append(css);
    });
};

export const prepareInlineSprite = (node: SVGSVGElement | null) => {
    if (node == null) {
        return;
    }

    node.querySelectorAll('title, desc').forEach(element => element.remove());
    hoistSpriteStyles(node);
};
