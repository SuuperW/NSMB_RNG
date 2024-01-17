import { findRow2Matches } from "./functions/tiles";
import * as paramSearch from "./functions/rng-params-search";
import { RngParams, SearchParams } from "./functions/rng-params-search";

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
		if (this.worker) {
			// Use 4 workers if the search params aren't constrained.
			if (options.minVCount == 0 && options.maxVCount == 263) {
				let subOptions: SearchParams[] = [];
				subOptions.push(new SearchParams({
					...options,
					minTimer0: options.minTimer0,
					minVCount: options.minVCount,
					maxTimer0: (options.maxTimer0 + options.minTimer0) >>> 1,
					maxVCount: (options.maxVCount + options.minVCount) >>> 1,
				}));
				subOptions.push(new SearchParams({
					...options,
					minTimer0: subOptions[0].maxTimer0 + 1,
					minVCount: options.minVCount,
					maxTimer0: options.maxTimer0,
					maxVCount: (options.minVCount + options.minVCount) >>> 1,
				}));
				subOptions.push(new SearchParams({
					...options,
					minTimer0: options.minTimer0,
					minVCount: subOptions[0].maxVCount + 1,
					maxTimer0: (options.maxTimer0 + options.minTimer0) >>> 1,
					maxVCount: options.maxVCount,
				}));
				subOptions.push(new SearchParams({
					...options,
					minTimer0: subOptions[0].maxTimer0 + 1,
					minVCount: subOptions[0].maxVCount + 1,
					maxTimer0: options.maxTimer0,
					maxVCount: options.maxVCount,
				}));

				let promises: Promise<RngParams[]>[] = [];
				promises.push(this.searchForSeeds(seeds, subOptions[0]));
				for (let i = 1; i < subOptions.length; i++) {
					promises.push(new WorkerWrapper().searchForSeeds(seeds, subOptions[i]));
				}
				let result = [];
				for (let p of promises) {
					result.push(...(await p));
				}
				return result;
			} else {
				return await this.call(paramSearch.searchForSeeds.workerName, { seeds: seeds, options: options }) as paramSearch.RngParams[];
			}
		}
		else
			return paramSearch.searchForSeeds(seeds, options);
	}

	async searchForTime(seeds: number[], params: paramSearch.RngParams, minYear?: number, maxYear?: number) {
		if (this.worker) {
			if (!minYear) minYear = 2000;
			if (!maxYear) maxYear = params.is3DS ? 2050 : 2100;
			if (minYear + 1 != maxYear) {
				// Initialize
				let foundTime: Date | null = null;
				let cYear = 2000;
				let initialSeconds = params.datetime.getSeconds();
				let workers: WorkerWrapper[] = [this];
				for (let i = 1; i < Math.max(1, navigator.hardwareConcurrency - 1); i++) {
					workers.push(new WorkerWrapper());
				}

				// Set up promise that we'll resolve once work is complete
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
						attachThen(workers[workerId].searchForTime(seeds, params, cYear, ++cYear), workerId);
					});
				}

				// Start all workers
				for (let i = 0; i < navigator.hardwareConcurrency - 1; i++) {
					attachThen(workers[i].searchForTime(seeds, params, cYear, ++cYear), i);
				}

				return await promise;
			} else {
				return await this.call(paramSearch.searchForTime.workerName, {
					seeds: seeds,
					params: params,
					minYear: minYear,
					maxYear: maxYear,
				}) as Date | null;
			}
		} else
			return paramSearch.searchForTime(new Set(seeds), params, 2000, params.is3DS ? 2050 : 2100);
	}
}
