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
}

export class RngParamsSearchResultManager {
	private http: HttpClient | undefined;
	private sessionId: string | undefined;

	private _totalMatchedPatterns: number = 0;
	private set totalMatchedPatterns(value: number) { this._totalMatchedPatterns = value; }
	public get totalMatchedPatterns(): number { return this._totalMatchedPatterns; }

	private _submitCount: number = 0;
	private set submitCount(value: number) { this._submitCount = value; }
	public get submitCount(): number { return this._submitCount; }


	private results: RngParamsSearch[] = [];

	private range: SearchParams;
	private otherRngParams: RngParams | undefined;

	// the RNG params within the expected range; excludes false positives
	private distinctParams: { timer0: number, vCount: number, count: number }[] = [];

	private gotTwoRngParamsInOneResult = false;
	private _giveUp = false;
	private set givenUp(value: boolean) { this._giveUp = value; }
	public get givenUp(): boolean { return this._giveUp; }


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

	getInputsWithNoFoundRngParams(): SearchInputs[] {
		return this.results.filter((r) => {
			return r.result.length === 0;
		}).map((r) => {
			return {
				seeds: r.seeds,
				row1: r.row1,
				row2: r.row2,
			};
		});
	}

	getFor(row1: string, row2: string) {
		for (let r of this.results) {
			if (r.row1 == row1 && r.row2 == row2) {
				return r;
			}
		}
		return null;
	}

	getDistinctParamsCount(): number {
		return this.distinctParams.length + (!!this.otherRngParams ? 1 : 0);
	}

	private incrementCount(params: RngParams) {
		let existing = this.distinctParams.find((v) => v.timer0 === params.timer0 && v.vCount === params.vCount);
		if (existing)
			existing.count++;
		else
			this.distinctParams.push({ timer0: params.timer0, vCount: params.vCount, count: 1 });
	}
	private useForRange(params: RngParams) {
		this.distinctParams = [{ timer0: params.timer0, vCount: params.vCount, count: 1 }];
		this.range.minTimer0 = this.range.maxTimer0 = params.timer0;
		this.range.minVCount = this.range.maxVCount = params.vCount;
		this.range.minVFrame = this.range.maxVFrame = params.vFrame;
	}
	private expandRange(params: RngParams) {
		this.incrementCount(params);
		this.range.minTimer0 = Math.min(this.range.minTimer0, params.timer0);
		this.range.maxTimer0 = Math.max(this.range.minTimer0, params.timer0);
		this.range.minVCount = Math.min(this.range.minVCount, params.vCount);
		this.range.maxVCount = Math.max(this.range.maxVCount, params.vCount);
	}
	submitResult(result: RngParamsSearch) {
		this.submitCount++;

		let existing = this.getFor(result.row1, result.row2);
		if ((!existing || existing.result.length == 0) && result.result.length > 0)
			this.totalMatchedPatterns++;

		if (existing) {
			result.result.forEach((r) => this.incrementCount(r));
			existing.result = result.result;
			this.postResults(result);
			return;
		}

		if (result.result.length > 1)
			this.gotTwoRngParamsInOneResult = true;
		for (let params of result.result) {
			if (this.range.minTimer0 === -1) {
				// This is our first RNG params
				this.useForRange(params);
			} else if /* this params is within the expected range */ (
				params.vFrame === this.range.minVFrame &&
				params.vCount >= this.range.minVCount - 2 && params.vCount <= this.range.maxVCount + 2 &&
				params.timer0 >= this.range.minTimer0 - 25 && params.timer0 <= this.range.maxTimer0 + 25
			) {
				this.expandRange(params);
			} else if (!this.otherRngParams) {
				// This is our first result that doesn't fit the expected range.
				this.otherRngParams = params;
			} else if ( // Should we make this result be the expected range?
				this.range.minTimer0 === this.range.maxTimer0 && this.range.minVCount === this.range.maxVCount &&
				params.vFrame === this.otherRngParams.vFrame &&
				params.vCount >= this.otherRngParams.vCount - 2 && params.vCount <= this.otherRngParams.vCount + 2 &&
				params.timer0 >= this.otherRngParams.timer0 - 25 && params.timer0 <= this.otherRngParams.timer0 + 25
			) {
				this.useForRange(this.otherRngParams);
				this.expandRange(params);
				// At this point it doesn't matter what's in otherRngParams; we'll never use it again.
			} else {
				this.givenUp = true; // we're going to just give up at this point, it should be extremely rare
			}
		}

		this.results.push(result);
		this.postResults(result);
	}

	isFalsePositiveSuspected(): boolean {
		return this.gotTwoRngParamsInOneResult || !!this.otherRngParams;
	}

	private toParams(p: { timer0: number, vCount: number }): RngParams {
		return {
			timer0: p.timer0,
			vCount: p.vCount,
			vFrame: this.range.minVFrame,
			datetime: this.range.datetime,
			is3DS: this.range.is3DS,
			mac: this.range.mac,
			buttons: this.range.buttons,
		}
	}
	getMostLikelyResult(): RngParams | undefined {
		if (this.totalMatchedPatterns === 0) return undefined;
		if (this.givenUp) return undefined;

		// A result can be considered "likely" if we've seen identical RNG params at lesat twice.
		// Of these the most frequently occuring (highest count) should be preferred.
		// To reduce chances of returning a false positive, we verify that we also have another nearby result,
		// unless we have only a single result 4+ times (because some consoles are highly consistent).
		if (this.totalMatchedPatterns === 1) {
			if (this.results.length === 1 && this.submitCount > 3 && !this.isFalsePositiveSuspected())
				return this.results[0].result[0];
			else
				return undefined;
		}
		let candidates = this.distinctParams.sort((a, b) => b.count - a.count);
		if (candidates[0].count === 1) // guaranteed length >= 1 by totalMatchedPatterns
			return undefined;
		// One other result that is off by just 1 in timer0 and/or vCount is sufficient. Most console should be like this.
		// Otherwise, we should have at least two other distinct params.
		if (this.distinctParams.length > 2) {
			return this.toParams(candidates[0]);
		} else {
			let dt = candidates[0].timer0 - candidates[1].timer0;
			let dv = candidates[0].vCount - candidates[1].vCount;
			if (Math.abs(dt) < 2 && Math.abs(dv) < 2)
				return this.toParams(candidates[0]);
			else
				return undefined;
		}
	}

	getSearchParams() {
		if (this.range.maxTimer0 === -1)
			return null;
		else {
			let sp = new SearchParams(this.range);
			// The largest known timer0 range is 21. That console might be able to have a wider range, it wasn't extensively tested.
			// We don't know if found params are on the low or high side, so we'll spread out both ways.
			// Also extend beyond current even if we're at max, just in case.
			let timer0RangeExtension = Math.max(2, 25 - (sp.maxTimer0 - sp.minTimer0));
			sp.minTimer0 -= timer0RangeExtension;
			sp.maxTimer0 += timer0RangeExtension;
			// The largest observed vCount range is 3, on one single console. Most have 2, a few have 1.
			// But the number of consoles tested isn't very big, so we'll assume 4 is possible.
			let vCountRangeExtension = Math.max(1, 3 - (sp.maxVCount - sp.minVCount));
			sp.minVCount -= vCountRangeExtension;
			sp.maxVCount += vCountRangeExtension;
			// Everything else should be consistent.
			return sp;
		}
	}

	suspectUserErrorOrStrangeConsole() {
		// These numbers are kinda arbitrary. The intent is to detect when something is wrong so we/user don't waste time endlessly entering bad patterns.
		return this.totalMatchedPatterns < this.results.length && (
			(this.submitCount >= 3 && this.totalMatchedPatterns == 0) ||
			(this.submitCount >= 5 && this.totalMatchedPatterns == 1) ||
			(this.submitCount >= 8 && this.totalMatchedPatterns == 2)
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
		if (submitCount == 1) {
			this._sessionPromise = this.createSession();
			this.sessionId = await this._sessionPromise;
			this._sessionPromise = undefined;
		} else if (this._sessionPromise) {
			await this._sessionPromise;
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
