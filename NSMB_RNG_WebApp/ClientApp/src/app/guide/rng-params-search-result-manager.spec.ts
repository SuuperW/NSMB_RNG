import { RngParamsSearchResultManager } from './rng-params-search-result-manager';

describe('RngParamsSearchResultManager', () => {
	let manager: RngParamsSearchResultManager;
	beforeEach(() => {
		let mac = '40:f4:07:f7:d4:21';
		localStorage.setItem('mac', mac);

		manager = new RngParamsSearchResultManager(new Date(2023, 10, 24, 1, 0, 15));
	});

	it('should create an instance', () => {
		expect(manager).toBeTruthy();
	});
});
