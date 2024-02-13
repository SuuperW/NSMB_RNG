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

		manager.submitResult(result1);
		manager.submitResult(result2);
		manager.submitResult(result2);
		manager.submitResult(result3);
		manager.submitResult(result4);
		manager.submitResult(result1);
		manager.submitResult(result3);
		manager.submitResult(result3);

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

		manager.submitResult(result1);
		manager.submitResult(result2);
		manager.submitResult(result3);

		let params = manager.getMostLikelyResult();
		assert(params === undefined);

		manager.submitResult(result2);
		params = manager.getMostLikelyResult();
		assert(params !== undefined);
	});

	it('flags obvious false positive', () => {
		// We must have a true positive for it to compare to
		let result = makeFakeResult(0x566, 0x26, 5);
		let resultFalse = makeFakeResult(0x333, 0xff, 5);

		manager.submitResult(result);
		manager.submitResult(resultFalse);

		assert(manager.isFalsePositiveSuspected());
	});

	it('suggests a result when result set has wide but realistic spread of timer0 values', () => {
		let result1 = makeFakeResult(1382, 232, 6);
		let result2 = makeFakeResult(1380, 232, 6);
		let result3 = makeFakeResult(1374, 232, 6);
		let result4 = makeFakeResult(1362, 232, 6);

		manager.submitResult(result1);
		manager.submitResult(result2);
		manager.submitResult(result3);
		manager.submitResult(result4);

		manager.submitResult(result2);

		assert(!manager.isFalsePositiveSuspected());
		assert(manager.getMostLikelyResult() !== undefined);
	});

	it('does not suggest result that has been submitted twice until submitted four times, when it is only result', () => {
		let result = makeFakeResult(0x566, 0x26, 5);
		manager.submitResult(result);
		manager.submitResult(result);
		manager.submitResult(result);

		assert(manager.getMostLikelyResult() === undefined);
		manager.submitResult(result);
		assert(manager.getMostLikelyResult() !== undefined);
	});

	it('suggests likely rng params when a false positive was given first', () => {
		let result1 = makeFakeResult(0x566, 0x26, 5);
		let result2 = makeFakeResult(0x567, 0x26, 5);
		let resultFalse = makeFakeResult(0x333, 0xff, 5);

		manager.submitResult(resultFalse);
		manager.submitResult(result1);
		manager.submitResult(result2);
		manager.submitResult(result1);

		assert(manager.isFalsePositiveSuspected());
		assert(manager.getMostLikelyResult() !== undefined);
	});

	it('suggests likely rng params when a false positive was given later', () => {
		let result1 = makeFakeResult(0x566, 0x26, 5);
		let result2 = makeFakeResult(0x567, 0x26, 5);
		let resultFalse = makeFakeResult(0x333, 0xff, 5);

		manager.submitResult(result1);
		manager.submitResult(result2);
		manager.submitResult(resultFalse);
		manager.submitResult(result1);

		assert(manager.isFalsePositiveSuspected());
		assert(manager.getMostLikelyResult() !== undefined);
	});

	it('does not suggest any params after 2 distinct false positives', () => {
		let result1 = makeFakeResult(0x566, 0x26, 5);
		let result2 = makeFakeResult(0x567, 0x26, 5);
		let resultFalse = makeFakeResult(0x333, 0xff, 5);
		let resultFalse2 = makeFakeResult(0x999, 0x01, 6);

		manager.submitResult(result1);
		manager.submitResult(resultFalse);
		manager.submitResult(result2);
		manager.submitResult(resultFalse2);
		manager.submitResult(result1);

		assert(manager.getMostLikelyResult() === undefined);
		assert(manager.isFalsePositiveSuspected());
	});
});
