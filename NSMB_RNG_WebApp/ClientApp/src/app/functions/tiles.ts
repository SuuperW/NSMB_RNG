import { nextState, previousState, tileIDFromState } from './rng';

const STEPS_BEFORE_ROW1 = 1937;
const TILES_PER_ROW = 27;
let letters = ['B', 'E', 'I', 'C', 'P', 'S'];

export const findRow2Matches = (seedsRow1: number[], row2: string): number[] => {
	let tiles: number[] = [];
	for (let i = 0; i < row2.length; i++) {
		let id = letters.indexOf(row2[i]);
		if (id == -1)
			return [];
		tiles.push(id);
	}

	let seedsRow2: number[] = [];
	for (let v of seedsRow1) {
		let r = v;
		for (let i = 0; i < STEPS_BEFORE_ROW1 + TILES_PER_ROW - 1; i++)
			r = nextState(r);

		// Check if this value matches the input for 11 tiles.
		let match = true;
		for (let i = 0; i < tiles.length; i++) {
			r = nextState(r);
			let tID = tileIDFromState(r);
			if (tID != tiles[i]) {
				match = false;
				break;
			}
		}
		if (match) {
			seedsRow2.push(...previousState(v));
		}
	}
	return seedsRow2;
}
findRow2Matches.workerName = 'fr2m';

export const getRow1 = (seed: number) => {
	let r = seed;
	for (let i = 0; i < STEPS_BEFORE_ROW1; i++)
		r = nextState(r);

	let tiles = '';
	for (let i = 0; i < 7; i++) {
		r = nextState(r);
		tiles += letters[tileIDFromState(r)];
	}

	return tiles;
}
export const getRow2 = (seed: number) => {
	let r = seed;
	for (let i = 0; i < STEPS_BEFORE_ROW1 + TILES_PER_ROW; i++)
		r = nextState(r);

	let tiles = '';
	for (let i = 0; i < 11; i++) {
		r = nextState(r);
		tiles += letters[tileIDFromState(r)];
	}

	return tiles;
}
