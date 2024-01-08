import { TestBed } from '@angular/core/testing';

import * as tiles from './tiles';
import { assert } from '../../test/assert';

describe('RNG functions', () => {
	beforeEach(() => {
		TestBed.configureTestingModule({});
	});

	it('example seeds from row 2', async () => {
		// Input seeds are a subset of possible seeds by row 1.
		// These are never calculated in the app; they are pre-computed and fetched from the server.
		const inputSeeds = [
			[486313, 714973, 2393298, 2542503, 3380138, 3415408, 4315598, 5080493, 5345848, 5983538, 3484600988],
			[368398, 3426548, 7609958, 9920043, 12301198, 12728828, 16284343, 19380978, 20962263, 23407193, 864523453, 3090445958],
		];
		const row2 = ['sbppebpcbbi', 'bssepspseee'];
		const expectedSeeds = [
			[365998418, 1224991877, 2083985336, 2942978795, 3801972254],
			[742534287, 1601527746, 2460521205, 3319514664, 4178508123, 585836825, 1444830284, 2303823743, 3162817202, 4021810661],
		];

		for (let i = 0; i < inputSeeds.length; i++) {
			let output = tiles.findRow2Matches(inputSeeds[i], row2[i].toUpperCase());
			assert(output.length == expectedSeeds[i].length);
			for (let j = 0; j < output.length; j++) {
				assert(expectedSeeds[i].indexOf(output[j]) != -1);
			}
		}
	});
});
