import { useSyncExternalStore } from 'react';

const NARROW_QUERY = '(max-width: 900px)';

function subscribeNarrowScreen(onChange: () => void): () => void {
    const query = window.matchMedia(NARROW_QUERY);
    query.addEventListener('change', onChange);
    return () => { query.removeEventListener('change', onChange); };
}

export function useNarrowScreen(): boolean {
    return useSyncExternalStore(subscribeNarrowScreen, () => window.matchMedia(NARROW_QUERY).matches);
}
