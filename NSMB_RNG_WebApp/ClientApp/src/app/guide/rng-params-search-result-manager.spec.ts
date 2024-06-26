import { assert } from '../../test/assert';
import { RngParams } from '../functions/rng-params-search';
import { RngParamsSearchResultManager } from './rng-params-search-result-manager';

let mac = '40:f4:07:f7:d4:21';
let dt = new Date(2023, 10, 24, 1, 0, 15);
let fakeResultId = 0;
let makeFakeResult = (timer0: number, vCount: number, vFrame: number) => {
	fakeResultId++;
	return {
		result: [{
			timer0: timer0,
			vCount: vCount,
			vFrame: vFrame,
			mac: mac,
			datetime: dt,
			is3DS: false,
			buttons: 0,
		}],
		seeds: [0],
		row1: fakeResultId.toString(), // result manager doesn't validate this, but does rely on rows to detect repeat submissions
		row2: '',
		offsetUsed: 0,
	};
}

describe('RngParamsSearchResultManager', () => {
	let manager: RngParamsSearchResultManager;

	// this won't be used for anything, but is a required parameter of submitResult
	let _dummyDate = new Date();
	let dummyDate = () => {
		_dummyDate.setMinutes(_dummyDate.getMinutes() - 1);
		return _dummyDate;
	 }; 

	beforeEach(() => {
		localStorage.setItem('mac', mac);

		manager = new RngParamsSearchResultManager(dt);
	});

	it('should create an instance', () => {
		expect(manager).toBeTruthy();
	});

	it('suggests most common params from a normal set', () => {
		// A 'normal' set means results of valid patterns from a console whose timer0 and vCount vary independently by 1 each.
		let result1 = makeFakeResult(0x566, 0x26, 5);
		let result2 = makeFakeResult(0x567, 0x26, 5);
		let result3 = makeFakeResult(0x566, 0x27, 5);
		let result4 = makeFakeResult(0x567, 0x27, 5);

		manager.submitResult(result1, dummyDate());
		manager.submitResult(result2, dummyDate());
		manager.submitResult(result2, dummyDate());
		manager.submitResult(result3, dummyDate());
		manager.submitResult(result4, dummyDate());
		manager.submitResult(result1, dummyDate());
		manager.submitResult(result3, dummyDate());
		manager.submitResult(result3, dummyDate());

		let params = manager.getMostLikelyResult() as RngParams;
		assert(params !== undefined);
		assert(params.timer0 === result3.result[0].timer0);
		assert(params.vCount === result3.result[0].vCount);
		assert(params.vFrame === result3.result[0].vFrame);
	});

	it('does not suggest any params until one result has been submitted twice', () => {
		let result1 = makeFakeResult(0x566, 0x26, 5);
		let result2 = makeFakeResult(0x567, 0x26, 5);
		let result3 = makeFakeResult(0x566, 0x27, 5);

		manager.submitResult(result1, dummyDate());
		manager.submitResult(result2, dummyDate());
		manager.submitResult(result3, dummyDate());

		let params = manager.getMostLikelyResult();
		assert(params === undefined);

		manager.submitResult(result2, dummyDate());
		params = manager.getMostLikelyResult();
		assert(params !== undefined);
	});

	it('suggests a result when result set has wide but realistic spread of timer0 values', () => {
		let result1 = makeFakeResult(1382, 232, 6);
		let result2 = makeFakeResult(1380, 232, 6);
		let result3 = makeFakeResult(1374, 232, 6);
		let result4 = makeFakeResult(1362, 232, 6);

		manager.submitResult(result1, dummyDate());
		manager.submitResult(result2, dummyDate());
		manager.submitResult(result3, dummyDate());
		manager.submitResult(result4, dummyDate());

		manager.submitResult(result2, dummyDate());

		assert(manager.getMostLikelyResult() !== undefined);
	});

	it('suggests result that has been submitted twice, when it is only result', () => {
		let result = makeFakeResult(0x566, 0x26, 5);
		manager.submitResult(result, dummyDate());
		manager.submitResult(result, dummyDate());

		assert(manager.getMostLikelyResult() !== undefined);
	});

	it('suggests likely rng params when a false positive was given first', () => {
		let result1 = makeFakeResult(0x566, 0x26, 5);
		let result2 = makeFakeResult(0x567, 0x26, 5);
		let resultFalse = makeFakeResult(0x333, 0xff, 5);

		manager.submitResult(resultFalse, dummyDate());
		manager.submitResult(result1, dummyDate());
		manager.submitResult(result2, dummyDate());
		manager.submitResult(result1, dummyDate());

		assert(manager.getMostLikelyResult() !== undefined);
	});

	it('suggests likely rng params when a false positive was given later', () => {
		let result1 = makeFakeResult(0x566, 0x26, 5);
		let result2 = makeFakeResult(0x567, 0x26, 5);
		let resultFalse = makeFakeResult(0x333, 0xff, 5);

		manager.submitResult(result1, dummyDate());
		manager.submitResult(result2, dummyDate());
		manager.submitResult(resultFalse, dummyDate());
		manager.submitResult(result1, dummyDate());

		assert(manager.getMostLikelyResult() !== undefined);
	});

	it('suggests likely rng params after 2 distinct false positives', () => {
		let result1 = makeFakeResult(0x566, 0x26, 5);
		let result2 = makeFakeResult(0x567, 0x26, 5);
		let resultFalse = makeFakeResult(0x333, 0xff, 5);
		let resultFalse2 = makeFakeResult(0x999, 0x01, 6);

		manager.submitResult(result1, dummyDate());
		manager.submitResult(resultFalse, dummyDate());
		manager.submitResult(result2, dummyDate());
		manager.submitResult(resultFalse2, dummyDate());
		manager.submitResult(result1, dummyDate());

		assert(manager.getMostLikelyResult() !== undefined);
	});

	it('does not give search params after a result that has no matches', () => {
		assert(manager.getSearchParams(dummyDate()) === null, 'Not having recommended search params should be indicated by a null value, but a non-null value was returned before any results were submitted.');

		let result = {
			result: [],
			seeds: [0],
			row1: '',
			row2: '',
			offsetUsed: 0,
		};
		manager.submitResult(result, dummyDate());

		assert(manager.getSearchParams(dummyDate()) === null, 'Search params were returned after only a bad (matchless) result was submitted. There is no information to base a range on, so no range should be returned.');
	});
});

