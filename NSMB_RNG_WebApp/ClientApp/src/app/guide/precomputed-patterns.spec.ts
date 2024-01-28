import { assert } from '../../test/assert';
import { SearchParams } from '../functions/rng-params-search';
import { PrecomputedPatterns } from './precomputed-patterns';



describe('PrecomputedPatterns', () => {
	let pp: PrecomputedPatterns;

	beforeEach(() => {
		pp = new PrecomputedPatterns();
		pp.addParams(new SearchParams({
			mac: '40:f4:07:f7:d4:21',
			minTimer0: 1373,
			maxTimer0: 1393,
			minVCount: 36,
			maxVCount: 42,
			minVFrame: 5,
			maxVFrame: 5,
			datetime: new Date(2017, 10, 7, 13, 34, 16),
			is3DS: false,
		}), 1);
	});

	it('should create an instance', () => {
		expect(pp).toBeTruthy();
	});

	it('should recognize known patterns', () => {
		let result = pp.getPatternInfo('cpccccc');
		assert(result.ambiguous === false);
		assert(result.match?.seconds === 0);

		result = pp.getPatternInfo('bbbbeeb');
		assert(result.ambiguous === false);
		assert(result.match?.seconds === 0);

		result = pp.getPatternInfo('ssespec');
		assert(result.ambiguous === false);
		assert(result.match?.seconds === 0);
	});

	it('should recognize pattern from +1 second', () => {
		let result = pp.getPatternInfo('bbsiise');
		assert(result.ambiguous === false);
		assert(result.match?.seconds === 1);
	});

	it('should recognize patterns less than 7 tiles', () => {
		let result = pp.getPatternInfo('bes');
		assert(result.ambiguous === true);

		result = pp.getPatternInfo('besb');
		assert(result.ambiguous === false);
		assert(result.match?.seconds === 0);

		result = pp.getPatternInfo('besbi');
		assert(result.ambiguous === false);
		assert(result.match?.seconds === 0);
	});

	it('should know when pattern does not match', () => {
		let result = pp.getPatternInfo('bbbbeb');
		assert(result.ambiguous === false);
		assert(result.match === undefined);

		result = pp.getPatternInfo('iii');
		assert(result.ambiguous === false);
		assert(result.match === undefined);
	});
});
