/// <reference lib="webworker" />

import { findRow2Matches } from "./functions/tiles";
import { RngParams, SearchParams, searchForSeeds, searchForTime } from './functions/rng-params-search';
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
					...event.data.options,
					minTimer0: event.data.options.minTimer0,
					minVCount: event.data.options.minVCount,
					maxTimer0: (event.data.options.maxTimer0 + event.data.options.minTimer0) >>> 1,
					maxVCount: (event.data.options.maxVCount + event.data.options.minVCount) >>> 1,
				}));
				options.push(new SearchParams({
					...event.data.options,
					minTimer0: options[0].maxTimer0 + 1,
					minVCount: event.data.options.minVCount,
					maxTimer0: event.data.options.maxTimer0,
					maxVCount: (event.data.options.minVCount + event.data.options.minVCount) >>> 1,
				}));
				options.push(new SearchParams({
					...event.data.options,
					minTimer0: event.data.options.minTimer0,
					minVCount: options[0].maxVCount + 1,
					maxTimer0: (event.data.options.maxTimer0 + event.data.options.minTimer0) >>> 1,
					maxVCount: event.data.options.maxVCount,
				}));
				options.push(new SearchParams({
					...event.data.options,
					minTimer0: options[0].maxTimer0 + 1,
					minVCount: options[0].maxVCount + 1,
					maxTimer0: event.data.options.maxTimer0,
					maxVCount: event.data.options.maxVCount,
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
		case searchForTime.name: {
			// We received simple data. Convert to proper types.
			let seeds = new Set<number>(event.data.seeds);
			let params: RngParams = event.data.params;
			params.datetime = new Date(params.datetime);

			if (event.data.minYear) {
				postMessage({
					data: searchForTime(seeds, params, event.data.minYear, event.data.maxYear),
					id: event.data.id,
				});
			} else {
				// Initialize
				let cYear = 2000;
				let maxYear = params.is3DS ? 2050 : 2100;
				let initialSeconds = params.datetime.getSeconds();
				let workers: WorkerWrapper[] = [];
				for (let i = 0; i < Math.max(1, navigator.hardwareConcurrency - 1); i++) {
					workers.push(new WorkerWrapper());
				}

				// Set up promise that we'll resolve once work is complete
				let foundTime: Date | null = null;
				let resolvePromise: (value: Date | null) => void;
				let promise = new Promise<Date | null>(function (resolve, _) {
					resolvePromise = resolve;
				});

				// Function to start checking next year after searching current year finishes
				let attachThen = (p: Promise<Date | null>, workerId: number) => {
					p.then((dt) => {
						if (foundTime) return; // another worker found a time already
						if (dt) { // this worker found a time
							foundTime = dt;
							resolvePromise(dt);
							return;
						}
						// If we have gone through all years, increment seconds and start over.
						if (cYear == maxYear) {
							cYear = 2000;
							params.datetime.setSeconds(params.datetime.getSeconds() + 1);
							if (params.datetime.getSeconds() == initialSeconds) {
								// This will probably never happen
								foundTime = new Date(); // set this so other workers will stop
								resolvePromise(null);
								return;
							}
						}
						attachThen(workers[workerId].searchForTime(event.data.seeds, params, cYear, ++cYear), workerId);
					});
				}

				// Start all workers
				for (let i = 0; i < navigator.hardwareConcurrency - 1; i++) {
					attachThen(workers[i].searchForTime(event.data.seeds, params, cYear, ++cYear), i);
				}

				postMessage({
					data: await promise,
					id: event.data.id,
				});
			}

			return;
		}
		default: {
			throw `Worker received an invalid function name: ${event.data.func}.`;
		}
	}
});
