/// <reference lib="webworker" />

import { findRow2Matches } from './functions/rng';

addEventListener('message', (event: MessageEvent) => {
	switch (event.data.func) {
		case findRow2Matches.name: {
			postMessage({
				data: findRow2Matches(event.data.seeds, event.data.row2),
				id: event.data.id,
			});
			return;
		}
		default: {
			throw `Worker received an invalid function name: ${event.data.func}.`;
		}
	}
});
