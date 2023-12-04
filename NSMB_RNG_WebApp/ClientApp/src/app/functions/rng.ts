const STEPS_BEFORE_ROW1 = 1937;
const TILES_PER_ROW = 27;

export const findRow2Matches = (seedsRow1: number[], row2: string): number[] => {
	let letters = ['B', 'E', 'I', 'C', 'P', 'S'];
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

export const nextState = (v: number) => {
	// Number.MAX_SAFE_INTEGER > (0x0019660D * 0xffffffff + 0x3C6EF35F)  is true, so this should be safe.
	let a = 0x0019660D * v + 0x3C6EF35F;
	// Unfortunately, >> operator converts to 32-bit integer first so >> 32 is not helpful.
	//return a + (a >> 32);
	return (a + Math.floor(a / 0x1_0000_0000)) % 0x1_0000_0000;
}
// See NSMB_RNG.TilesFor12.reverseStep
export const previousState = (v: number) => {
	const m = 0x0019660D;
	const twoP32 = 0x1_0000_0000;

	const bigStep = Math.floor(twoP32 / m);
	const bigStepOffset = Math.floor(twoP32 - (bigStep * m));

	let tryMe = Math.floor((v - nextState(0)) / m);
	while (tryMe < 0x33333333) {
		let result = nextState(tryMe);
		// We added one so that we can easily check if r is within 1 of v.
		let diff = result + 1 - v;

		if (diff < 0) {
			// Underflow, result was less than v: Get back to >= v.
			diff += 0x1_0000_0000;
			tryMe += 1;
		}
		else if (diff <= 2)
			break;
		else {
			// Result was greater than v: Go to next value known to be before or at the next match.
			let bigStepsCount = Math.floor((diff + bigStepOffset - 1) / bigStepOffset);
			tryMe += bigStep * bigStepsCount;
		}

	}

	if (tryMe >= 0x33333333)
		return ([] as number[]);
	else {
		let allResults: number[] = [];
		let tryMeToo = tryMe;
		while (tryMeToo < 0x1_0000_0000) {
			if (nextState(tryMeToo) == v)
				allResults.push(tryMeToo);

			tryMeToo += 0x33333333;
		}
		return allResults;
	}
}

export const tileIDFromState = (v: number) => {
	return ((v >> 8) & 0x7) % 6;
}


