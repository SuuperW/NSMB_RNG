import { HttpClient } from "@angular/common/http";
import { RngParams, SearchParams } from "../functions/rng-params-search";

export type RngParamsSearch = {
	result: RngParams[],
	seeds: number[],
	row1: string,
	row2: string,
	offsetUsed: number,
}

export type SearchInputs = {
	seeds: number[],
	row1: string,
	row2: string,
	time: Date,
}

type ParamsWithCount = {
	timer0: number,
	vCount: number,
	vFrame: number,
	count: number
}

export class RngParamsSearchResultManager {
	private http: HttpClient | undefined;
	private sessionId: string | undefined;

	private _distinctParamsCount: number = 0;
	private set distinctParamsCount(value: number) { this._distinctParamsCount = value; }
	public get distinctParamsCount(): number { return this._distinctParamsCount; }

	private _submitCount: number = 0;
	private set submitCount(value: number) { this._submitCount = value; }
	public get submitCount(): number { return this._submitCount; }

	public emptySearches: SearchInputs[] = [];
	private submittedTimes: Set<number> = new Set<number>();

	private range: SearchParams;

	private distinctParams: ParamsWithCount[] = [];

	private maxExpectedTimer0Diff = 25;
	private maxExpectedVCountDiff = 2;


	constructor(date: Date, http: HttpClient | undefined = undefined) {
		this.range = new SearchParams({
			mac: localStorage.getItem('mac') ?? '',
			datetime: new Date(date),
			is3DS: localStorage.getItem('consoleType') == '3DS',
			minTimer0: -1,
			maxTimer0: -1,
		})

		if (this.range.mac == '')
			throw 'No MAC in local storage for RngParamsSearchResultManager';

		if (http)
			this.http = http;
	}

	getDistinctParamsCount(): number {
		return this.distinctParams.length;
	}

	getPriorResult(params: RngParams | ParamsWithCount): ParamsWithCount | undefined {
		let priorResult = this.distinctParams.find((p) => 
			p.timer0 === params.timer0 && 
			p.vCount === params.vCount && 
			p.vFrame === params.vFrame
		);
		return priorResult;

	}

	isNearRange(params: RngParams | ParamsWithCount, range: SearchParams) {
		if (
			params.timer0 >= range.minTimer0 - this.maxExpectedTimer0Diff && params.timer0 <= range.maxTimer0 + this.maxExpectedTimer0Diff &&
			params.vCount >= range.minVCount - this.maxExpectedVCountDiff && params.vCount <= range.maxVCount + this.maxExpectedVCountDiff &&
			params.vFrame === range.minVFrame
		) {
			return true;
		} else {
			return false;
		}
	}

	private incrementCount(params: RngParams | ParamsWithCount) {
		let existing = this.getPriorResult(params);
		if (existing)
			existing.count++;
		else
			this.distinctParams.push({ timer0: params.timer0, vCount: params.vCount, vFrame: params.vFrame, count: 1 });
	}
	private useForRange(params: RngParams) {
		this.range.minTimer0 = this.range.maxTimer0 = params.timer0;
		this.range.minVCount = this.range.maxVCount = params.vCount;
		this.range.minVFrame = this.range.maxVFrame = params.vFrame;
	}
	private expandRange(params: RngParams | ParamsWithCount) {
		this.range.minTimer0 = Math.min(this.range.minTimer0, params.timer0);
		this.range.maxTimer0 = Math.max(this.range.minTimer0, params.timer0);
		this.range.minVCount = Math.min(this.range.minVCount, params.vCount);
		this.range.maxVCount = Math.max(this.range.maxVCount, params.vCount);
	}
	/**
	 * This method should not be called twice with the same time, until "most likely" RngParams are obtained.
	 * @param result The result of the RNG parameter search.
	 * @param time The time of expected RNG initialization. Required until "most likely" RngParams are obtained.
	 */
	submitResult(result: RngParamsSearch, time?: Date) {
		if (!this.mostLikely) {
			if (!time || this.submittedTimes.has(time.valueOf())) {
				// this should never happen; caller should ensure no two calls have the same time
				alert('something went wrong; submission discarded');
				return;
			}
		}

		this.submitCount++;

		if (result.result.length === 0) {
			if (this.emptySearches.findIndex((v) => v.row1 == result.row1 && v.row2 == result.row2) === -1) {
				this.emptySearches.push({
					row1: result.row1,
					row2: result.row2,
					seeds: result.seeds,
					time: time!,
				});
			}
		}
		for (let params of result.result) {	
			let prior = this.getPriorResult(params);
			this.incrementCount(params); // this adds the result, so getPriorResult is called before this
			if (prior)
				continue;
			this.distinctParamsCount++;

			// The rest of the loop is concerned with updating this.range.
			if (this.distinctParamsCount === 1) {
				this.useForRange(params);
			} else if (this.isNearRange(params, this.range)) {
				this.expandRange(params);
			} else {
				// This params, or the one which this.range is based on, is a false positive.
				// Check which possible range has the most values
				let nearRangeCount = this.distinctParams.filter(p => this.isNearRange(p, this.range)).length;
				let newRange = new SearchParams(this.range);
				newRange.minTimer0 = newRange.maxTimer0 = params.timer0;
				newRange.minVCount = newRange.maxVCount = params.vCount;
				newRange.minVFrame = newRange.maxVFrame = params.vFrame;
				let nearNewRange = this.distinctParams.filter(p => this.isNearRange(p, newRange));
				// If the new range based on this params fits more total params, we will use this new range.
				// The only time this should ever happen is if the first submitted result has a false positive.
				// Otherwise, we expect true positives to greatly outnumber false positives.
				if (nearNewRange.length > nearRangeCount) {
					this.range = newRange;
					for (let p of nearNewRange)
						this.expandRange(p);
				}
			}
		}

		if (time !== undefined)
			this.postResults(result);
	}

	private mostLikely: ParamsWithCount | undefined;
	private toParams(p: ParamsWithCount): RngParams {
		return {
			timer0: p.timer0,
			vCount: p.vCount,
			vFrame: p.vFrame,
			datetime: this.range.datetime,
			is3DS: this.range.is3DS,
			mac: this.range.mac,
			buttons: this.range.buttons,
		}
	}
	getMostLikelyResult(): RngParams | undefined {
		// A result can be considered "likely" if we've seen identical RNG params at lesat twice.
		// Previously we would also verify that it wasn't a false positive by checking for nearby results.
		// However, we now ensure each submitted result has a distinct time. This means any RNG
		// params that were found twice are 99.99999% real since collisions are incredibly rare.
		if (this.distinctParams.length === 0)
			return undefined;
		let highestCount = this.distinctParams.sort((a, b) => b.count - a.count)[0];
		if (highestCount.count === 1)
			return undefined;
		else {
			// Update most likely only if there is a higher (not equal) count
			if (!this.mostLikely || highestCount.count > this.mostLikely.count)
				this.mostLikely = highestCount;
		}

		return this.toParams(this.mostLikely);
	}

	getSearchParams(dt: Date) {
		if (this.distinctParamsCount === 0)
			return null;
		else {
			let sp = new SearchParams(this.range);
			sp.datetime = dt; // The manager doesn't know the time that's going to be searched.

			// The largest known timer0 range is 21. That console might be able to have a wider range, it wasn't extensively tested.
			// We don't know if found params are on the low or high side, so we'll spread out both ways.
			// Also extend beyond current even if we're at max, just in case.
			let timer0RangeExtension = Math.max(2, this.maxExpectedTimer0Diff - (sp.maxTimer0 - sp.minTimer0));
			sp.minTimer0 -= timer0RangeExtension;
			sp.maxTimer0 += timer0RangeExtension;
			// The largest observed vCount range is 3, on one single console. Most have 2, a few have 1.
			// But the number of consoles tested isn't very big, so we'll assume 4 is possible.
			let vCountRangeExtension = Math.max(1, this.maxExpectedVCountDiff - (sp.maxVCount - sp.minVCount));
			sp.minVCount -= vCountRangeExtension;
			sp.maxVCount += vCountRangeExtension;
			// Everything else should be consistent.
			return sp;
		}
	}

	suspectUserErrorOrStrangeConsole() {
		// These numbers are kinda arbitrary. The intent is to detect when something is wrong so we/user don't waste time endlessly entering bad patterns.
		return this.getMostLikelyResult() === undefined && (
			(this.submitCount >= 3 && this.distinctParamsCount == 0) ||
			(this.submitCount >= 5 && this.distinctParamsCount == 1) ||
			(this.submitCount >= 8 && this.distinctParamsCount == 2)
		);
	};

	private async createSession(retryOnError: boolean = true) {
		if (!this.http) return '';

		// The default behaviour for POSTing Date values is to convert them to UTC. We do not want that, we want to ignore timezones entirely.
		let convertedDate = new Date(this.range.datetime);
		convertedDate.setMinutes(convertedDate.getMinutes() - convertedDate.getTimezoneOffset());
		let postData = {
			datetime: convertedDate.toISOString().slice(0, -1),
			gameVersion: localStorage.getItem('gameVersion') ?? 'null',
			mac: this.range.mac,
			is3DS: this.range.is3DS,
		}

		let resolver: (value: string | PromiseLike<string>) => void = null!; // null! just tells code analysis to assume it has a value (it's set in next line but analyzer doesn't know that)
		let p = new Promise<string>((r) => { resolver = r });

		let somethingIsBrokenHere = { responseType: 'text' as 'json' }; // Wtf?
		this.http.post<string>('asp/submit/session', postData, somethingIsBrokenHere).subscribe({
			next: (v) => {
				if (v.length === 36)
					resolver(v);
				else
					resolver('');
			},
			error: async (_) => { // try again once on error
				if (retryOnError) resolver(await this.createSession(false));
				else resolver('');
			}
		});

		return await p;
	}
	private _sessionPromise: Promise<string> | undefined;
	private async postResults(results: RngParamsSearch) {
		if (!this.http) return;

		let submitCount = this.submitCount;
		let sid = this.sessionId;
		if (submitCount == 1) {
			this._sessionPromise = this.createSession();
			sid = await this._sessionPromise;
			this._sessionPromise = undefined;
		} else if (this._sessionPromise) {
			sid = await this._sessionPromise;
		}

		if (this.sessionId === undefined || this.sessionId.length !== 36)
			return;

		let postData: any = {
			...results,
			submissionId: submitCount,
			session: this.sessionId,
		};
		// Rename parameter to match expected name server-side
		postData.foundParams = results.result;
		delete postData.result;
		this.http.post<string>('asp/submit/result', postData).subscribe(); // need to subscribe or it won't actually send the request?
	}
}
