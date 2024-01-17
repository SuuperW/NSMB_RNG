/// <reference lib="webworker" />

import { findRow2Matches } from "./functions/tiles";
import { RngParams, searchForSeeds, searchForTime } from './functions/rng-params-search';

addEventListener('message', async (event: MessageEvent) => {
	switch (event.data.func) {
		// All function names should be given a workerName property in the file in which they're defined.
		// The reason for this is that minification results in functions having different actual names
		// in the worker's js file compared to the main js file.
		case findRow2Matches.workerName: {
			postMessage({
				data: findRow2Matches(event.data.seeds, event.data.row2),
				id: event.data.id,
			});
			return;
		}
		case searchForSeeds.workerName: {
			postMessage({
				data: searchForSeeds(event.data.seeds, event.data.options),
				id: event.data.id,
			});
			return;
		}
		case searchForTime.workerName: {
			// We received simple data. Convert to proper types.
			let seeds = new Set<number>(event.data.seeds);
			let params: RngParams = event.data.params;
			params.datetime = new Date(params.datetime);

			postMessage({
				data: searchForTime(seeds, params, event.data.minYear, event.data.maxYear),
				id: event.data.id,
			});
			return;
		}
		default: {
			throw `Worker received an invalid function name: ${event.data.func}.`;
		}
	}
});
