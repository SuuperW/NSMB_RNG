/// <reference lib="webworker" />

import { findRow2Matches } from './functions/rng';
import { RngParams, SearchParams, searchForSeeds } from './functions/rng-params-search';
// This import causes a warning about circular dependency. Apparently it's from webpack and could lead to improper caching by browsers...???
// I don't understand how it works, and I'm going to ignore it because I cannot find a staisfactory solution.
import { WorkerWrapper } from './worker-wrapper';

addEventListener('message', async (event: MessageEvent) => {
	switch (event.data.func) {
		case findRow2Matches.name: {
			postMessage({
				data: findRow2Matches(event.data.seeds, event.data.row2),
				id: event.data.id,
			});
			return;
		}
		case searchForSeeds.name: {
			if (event.data.options.minVCount > 0 || event.data.options.maxVCount < 263) {
				// We have restricted search params, probably not very helpful to create workers for a relatively short operation.
				// Also we don't want to recursively create 4, 16, 64, etc. workers.
				postMessage({
					data: searchForSeeds(event.data.seeds, event.data.options),
					id: event.data.id,
				});
			} else {
				// Use 4 workers
				let options: SearchParams[] = [];
				options.push(new SearchParams({
					minTimer0: event.data.options.minTimer0,
					minVCount: event.data.options.minVCount,
					maxTimer0: (event.data.options.maxTimer0 + event.data.options.minTimer0) >>> 1,
					maxVCount: (event.data.options.maxVCount + event.data.options.minVCount) >>> 1,
					...event.data.options
				}));
				options.push(new SearchParams({
					minTimer0: options[0].maxTimer0 + 1,
					minVCount: event.data.options.minVCount,
					maxTimer0: event.data.options.maxTimer0,
					maxVCount: (event.data.options.minVCount + event.data.options.minVCount) >>> 1,
					...event.data.options
				}));
				options.push(new SearchParams({
					minTimer0: event.data.options.minTimer0,
					minVCount: options[0].maxVCount + 1,
					maxTimer0: (event.data.options.maxTimer0 + event.data.options.minTimer0) >>> 1,
					maxVCount: event.data.options.maxVCount,
					...event.data.options
				}));
				options.push(new SearchParams({
					minTimer0: options[0].maxTimer0 + 1,
					minVCount: options[0].maxVCount + 1,
					maxTimer0: event.data.options.maxTimer0,
					maxVCount: event.data.options.maxVCount,
					...event.data.options
				}));

				let promises: Promise<RngParams[]>[] = [];
				for (let i = 0; i < 3; i++) {
					promises.push(new WorkerWrapper().searchForSeeds(event.data.seeds, options[i]));
				}
				let result = searchForSeeds(event.data.seeds, options[3]);
				for (let p of promises) {
					result.push(...(await p));
				}
				postMessage({ data: result, id: event.data.id });
			}
			return;
		}
		default: {
			throw `Worker received an invalid function name: ${event.data.func}.`;
		}
	}
});
