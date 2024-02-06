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

	private _totalMatchedPatterns: number = 0;
	private set totalMatchedPatterns(value: number) { this._totalMatchedPatterns = value; }
	public get totalMatchedPatterns(): number { return this._totalMatchedPatterns; }

	private _submitCount: number = 0;
	private set submitCount(value: number) { this._submitCount = value; }
	public get submitCount(): number { return this._submitCount; }


	private results: (RngParamsSearch & { count: number })[] = [];

	private generalPostData: { mac: string, is3DS: boolean, dtStr: string, gameVersion: string };

	private range: SearchParams;

	private gotTwoRngParamsInOneResult = false;

	constructor(date: Date, http: HttpClient | undefined = undefined) {
		// The default behaviour for POSTing Date values is to convert them to UTC. We do not want that, we want to ignore timezones entirely.
		let convertedDate = new Date(date);
		convertedDate.setMinutes(convertedDate.getMinutes() - convertedDate.getTimezoneOffset());
		this.generalPostData = {
			dtStr: convertedDate.toISOString().slice(0, -1),
			gameVersion: localStorage.getItem('gameVersion') ?? 'null',
			mac: localStorage.getItem('mac') ?? '',
			is3DS: localStorage.getItem('consoleType') == '3DS',
		}

		this.range = new SearchParams({
			mac: this.generalPostData.mac,
			datetime: new Date(date),
			is3DS: this.generalPostData.is3DS,
			minTimer0: -1,
			maxTimer0: -1,
		})

		if (this.generalPostData.mac == '')
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

	getDistinctPatternCount(): number {
		return this.results.length;
	}

	submitResult(result: RngParamsSearch) {
		this.submitCount++;

		let existing = this.getFor(result.row1, result.row2);
		if ((!existing || existing.result.length == 0) && result.result.length > 0)
			this.totalMatchedPatterns++;

		if (existing) {
			existing.count++;
			existing.result = result.result;
			return;
		}

		if (result.result.length !== 0) {
			let params = result.result[0];
			if (result.result.length > 1)
				this.gotTwoRngParamsInOneResult = true;

			if (this.range.maxTimer0 === -1) {
				this.range.minTimer0 = this.range.maxTimer0 = params.timer0;
				this.range.minVCount = this.range.maxVCount = params.vCount;
				this.range.minVFrame = this.range.maxVFrame = params.vFrame;
			} else {
				// TODO: handle false positives
				this.range.minTimer0 = Math.min(this.range.minTimer0, params.timer0);
				this.range.maxTimer0 = Math.max(this.range.minTimer0, params.timer0);
				this.range.minVCount = Math.min(this.range.minVCount, params.vCount);
				this.range.maxVCount = Math.max(this.range.maxVCount, params.vCount);
			}
		}

		let r = { ...result, count: 1 };
		this.results.push(r);
		if (r.result === undefined)
			throw "why is result undef??";
		this.postResults(r);
	}

	isFalsePositiveSuspected(): boolean {
		return false; // TODO
	}

	private getAllRngParams() {
		return this.results.flatMap((result) => result.result);
	}
	getMostLikelyResult() {
		// TODO

		// OLD:
		// If two results are positive and identical, use it if another result is off by one.
		const anyOffByOne = (params: RngParams) => {
			for (let r of this.getAllRngParams()) {
				let dt = params.timer0 - r.timer0;
				let dv = params.vCount - r.vCount;
				if (dt * dv == 0 && Math.abs(dt + dv) == 1)
					return true;
			}
			return false;
		};
		let paramsToUse: RngParams | undefined;
		for (let r of this.results) {
			if (r.count > 1) {
				for (let p of r.result) {
					if (anyOffByOne(p)) {
						paramsToUse = p;
						break;
					}
				}
				if (paramsToUse) break;
			}
		}

		if (!paramsToUse) {
			if (this.totalMatchedPatterns === 1 && this.results.length === 1 && this.submitCount > 3)
				paramsToUse = this.results[0].result[0];
		}

		return paramsToUse;
	}

	getSearchParams() {
		if (this.results.length == 0)
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


	private postResults(results: (RngParamsSearch & { count: number })) {
		if (!this.http) return;

		let postData: any = {
			...this.generalPostData,
			...results,
		};
		this.http.post<string>('asp/submitResults', postData).subscribe(); // need to subscribe or it won't actually send the request?
	}

}
