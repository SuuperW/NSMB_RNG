import * as rng from "./functions/rng";
import { findRow2Matches } from "./functions/tiles";
import * as paramSearch from "./functions/rng-params-search";

let workerSupported: boolean = !!Worker;
type dict = { [key: string]: any };

// A class that wraps a web worker, allowing outside code to call worker methods as async methods.
export class WorkerWrapper {
	private worker: Worker | undefined;
	private resolvers: { [i: number]: ((value: any | PromiseLike<any>) => void) } = {};
	private lastId = 0;

	constructor() {
		if (workerSupported) {
			this.worker = new Worker(new URL('app.worker.ts', import.meta.url));
			// When the worker is done with its computation, it'll send a message.
			// We inspect the ID and resolve the corresponding Promise.
			this.worker.onmessage = ({ data }) => {
				if (this.resolvers[data.id]) {
					this.resolvers[data.id](data.data);
					delete this.resolvers[data.id];
				} else {
					throw 'Invalid id received from worker.';
				}
			};
		}
	}

	private async call(functionName: string, copyData: dict) {
		this.lastId++;

		// Create a Promise that we can resolve in the worker's onmessage handler.
		let id = this.lastId;
		let this2 = this;
		let promise = new Promise<any>(function (resolve, _) {
			this2.resolvers[id] = resolve;
		});

		// Send the message and await the response via our Promise.
		this.worker!.postMessage({
			...copyData,
			id: id,
			func: functionName,
		});

		return await promise;
	}

	async findRow2Matches(seedsRow1: number[], row2: string): Promise<number[]> {
		// Using the worker is noticeably slower than calling it directly.
		// According to articles online, it SHOULD be no more than a few milliseconds...
		// unless I'm transferring a lot of data. Then we should use a Transferrable.
		// Yet, making a Uint32Array and transferring the buffer seems no faster.
		if (this.worker)
			return await this.call(findRow2Matches.workerName, { seeds: seedsRow1, row2: row2 }) as number[];
		else
			return findRow2Matches(seedsRow1, row2);
	}

	async searchForSeeds(seeds: number[], options: paramSearch.SearchParams) {
		if (this.worker)
			return await this.call(paramSearch.searchForSeeds.workerName, { seeds: seeds, options: options }) as paramSearch.RngParams[];
		else
			return paramSearch.searchForSeeds(seeds, options);
	}

	async searchForTime(seeds: number[], params: paramSearch.RngParams, minYear?: number, maxYear?: number) {
		if (this.worker)
			return await this.call(paramSearch.searchForTime.workerName, {
				seeds: seeds,
				params: params,
				minYear: minYear,
				maxYear: maxYear,
			}) as Date | null;
		else
			return paramSearch.searchForTime(new Set(seeds), params, 2000, params.is3DS ? 2050 : 2100);
	}
}
