import { Injectable } from '@angular/core';
import { RngParams } from './functions/rng-params-search';
import { WorkerWrapper } from './worker-wrapper';

type SearchState = {
	seeds: number[],
	params: RngParams,
	year: number,
	key: string,
	highPriority: boolean,
}
@Injectable({
	providedIn: 'root'
})
export class TimeFinderService {
	private timePromiseResolvers: { [key: string]: ((value: Date) => void) | undefined } = {};
	private timePromises: { [key: string]: Promise<Date> | undefined } = {};
	private baseSeconds: number = -1;

	private searchStates: SearchState[] = [];
	private paused: boolean = false;

	constructor() { }

	setBaseSeconds(sec: number) {
		this.baseSeconds = sec;
	}

	/** Begin searching for a time for which params will yeild the seeds specified by route. */
	addParams(params: RngParams, route: string | number[]) {
		if (this.baseSeconds === -1)
			throw 'Set the base seconds before trying to search!';

		let seeds: number[];
		if (typeof(route) === 'string') {
			seeds = route === 'mini' ? this.getMiniSeeds() : this.getNormalSeeds();
		} else {
			seeds = route;
		}

		const key = this.generateKey(params, seeds[0]);
		if (this.timePromises[key] || this.searchStates.find((s) => s.key === key))
			return;

		const searchState = {
			seeds, key,
			params: { ...params },
			year: 2000,
			highPriority: false
		};
		searchState.params.datetime.setSeconds(this.baseSeconds);
		this.searchStates.unshift(searchState);
		// Set up promise that we'll resolve once work is complete
		let resolvePromise: (value: Date) => void;
		this.timePromises[key] = new Promise<Date>(function (resolve, _) {
			resolvePromise = resolve;
		});
		this.timePromiseResolvers[key] = resolvePromise!;

		if (!this.paused)
			this.executeSearch();
	}

	private generateKey(params: RngParams, firstSeed: number): string {
		return `${params.timer0} ${params.vFrame} ${params.vCount} ${firstSeed}`;
	}

	/** Move a state to the next year (changing second if needed) */
	private nextYear(searchState: SearchState) {
		let newState: SearchState = { ...searchState }
		newState.params.datetime = new Date(newState.params.datetime); // clone
		newState.year++;
		let maxYear = newState.params.is3DS ? 2050 : 2100;
		if (newState.year === maxYear) {
			newState.year = 2000;
			newState.params.datetime.setSeconds(newState.params.datetime.getSeconds() + 1);
		}
		return newState;
	}

	/** Retrieve the next state to search, and update list of states */
	private nextState() {
		const nextState = this.searchStates[0];
		this.searchStates[0] = this.nextYear(nextState);
		// TEST
		if (nextState.year % 10 == 0) console.log(`${nextState.year}, ${nextState.key}`)
		if (this.searchStates.length === 1 || nextState.highPriority)
			return nextState;

		const secondDiff = this.searchStates[0].params.datetime.getSeconds() - this.searchStates[1].params.datetime.getSeconds();
		if (secondDiff > 0 || (secondDiff === 0 && this.searchStates[0].year > this.searchStates[1].year))
			this.searchStates.push(this.searchStates.shift()!);
		return nextState;
	}

	private promises: Promise<void>[] = [];
	/** Begins searching on multiple worker threads, unless they are already running. */
	private async executeSearch(highPriority: boolean = false) {
		let executeOne = async () => {
			let worker = new WorkerWrapper();
			// Loop until pause requested, or there is no more searching to do.
			while (!this.paused && this.searchStates.length !== 0) {
				let nextState = this.nextState();
				let result: Date | null = null;
				result = await this.searchOneYear(nextState, worker);
				if (result) {
					// Resolve promise and remove this state from list to process.
					this.timePromiseResolvers[nextState.key]!(result);
					this.searchStates = this.searchStates.filter((s) => s.key !== nextState.key);
				}
			}
		}

		let maxWorkers: number;
		if (highPriority)
			maxWorkers = Math.max(1, navigator.hardwareConcurrency - 1);
		else
			maxWorkers = Math.max(1, navigator.hardwareConcurrency / 2);
		while (this.promises.length < maxWorkers) {
			let promise = executeOne();
			this.promises.push(promise);
			// Remove from list of active promises when it exits
			promise.then(() => {
				this.promises = this.promises.filter((p) => p !== promise);
			});
			// No errors are ever expected.
			promise.catch((reason) => {
				// Since we don't know what might cause an error, the only thing we can do is let the user know.
				// This will be done by passing an invalid date and checking for it later.
				for (let key in this.timePromiseResolvers) {
					this.timePromiseResolvers[key]!(new Date('x'));
				}
			})
		}
	}

	private async searchOneYear(ss: SearchState, worker: WorkerWrapper) {
		return await worker.searchForTime(ss.seeds, ss.params, ss.year, ss.year + 1);
	}

	/** Return a promise for the requested time. If no search with params was started, returns undefined.
	 * If search is in progress, marks the search high-priority.
	 */
	getTime(params: RngParams, route: string | number[]) {
		let seeds: number[];
		if (typeof(route) === 'string') {
			seeds = route === 'mini' ? this.getMiniSeeds() : this.getNormalSeeds();
		} else {
			seeds = route;
		}

		const key = this.generateKey(params, seeds[0]);
		// Mark high-priority
		let searchState = this.searchStates.find((s) => s.key === key);
		if (searchState) {
			searchState.highPriority = true;
			this.paused = false;
			this.executeSearch(true);
		}

		// Once we've retrieved a time, it is unlikely we'll want any of the others. Don't waste processing power.
		this.timePromises[key]?.then((v) => this.pauseSearch());
		return this.timePromises[key];
		
	}

	pauseSearch() {
		this.paused = true;
	}

	resumeSearch() {
		this.paused = false;
		if (this.searchStates.length !== 0)
			this.executeSearch();
	}

	private getNormalSeeds(): number[] {
		return [
			0xaa99ad81, 0x2aa12d89, 0xa2a1a589, 0xaaa3ad8b, 0xaa21ad09, 0xcaa1cd89, 0xaca1af89, 0x11281410,
			0x4433471b, 0xc43ac722, 0x3c3b3f23, 0x43bb46a3, 0x443d4725, 0x643b6723, 0x463b4923, 0xaac1ada9,
			0xddcce0b4, 0xd5d4d8bc, 0x5dd460bc, 0xddd6e0be, 0xdd54e03c, 0xfdd500bc, 0xdfd4e2bc, 0x445b4743,
			0x77667a4e, 0xf76dfa55, 0x6f6e7256, 0x76ee79d6, 0x77707a58, 0x976e9a56, 0x796e7c56, 0xddf4e0dc,
			0x110013e8, 0x910793ef, 0x09080bf0, 0x130815f0, 0x310833f0, 0x10881370, 0x110a13f2, 0x778e7a76,
		];
	}

	private getMiniSeeds(): number[] {
		return [
			0x449b4783, 0x2aa02d88, 0xa921ac09, 0x0aa20d8a, 0x92a19589, 0xb0a1b389, 0x4423470b, 0xaaa7ad8f,
			0xde34e11c, 0xc439c721, 0x42bb45a3, 0x2c3b2f23, 0x4a3b4d23, 0xa43ba723, 0xddbce0a4, 0x44414729,
			0x77ce7ab6, 0x5dd360bb, 0xe3d4e6bc, 0x3dd540bd, 0x77567a3e, 0xc5d4c8bc, 0xdc54df3c, 0xdddae0c2,
			0x11681450, 0xf76cfa54, 0x75ee78d6, 0x5f6e6256, 0x7d6e8056, 0x10f013d8, 0xd76eda56, 0x77747a5c,
			0xab01ade9, 0x910693ee, 0xf907fbef, 0x0f881270, 0x170819f0, 0xaa89ad71, 0x710873f0, 0x110e13f6,
		];
	}
}