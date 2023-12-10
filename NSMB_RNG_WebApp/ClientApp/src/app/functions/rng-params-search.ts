import { SeedCalculator } from "../seed-calculator";

export type RngParams = {
	timer0: number,
	vCount: number,
	vFrame: number,
	mac: string,
	is3DS: boolean,
	datetime: Date,
	buttons: number,
};
export type SearchParams = {
	mac: string,
	datetime: Date,
	minTimer0: number,
	maxTimer0: number,
	minVCount?: number,
	maxVCount?: number,
	minVFrame?: number,
	maxVFrame?: number,
	is3DS?: boolean,
	buttons?: number,
};

export const searchForSeeds = (seeds: number[], options: SearchParams) => {
	options.minVCount ??= 0;
	options.maxVCount ??= 263;
	options.minVFrame ??= 4;
	options.maxVFrame ??= 8;
	options.is3DS ??= false;

	let sc = new SeedCalculator(options.mac, options.datetime, options.is3DS);
	let results: RngParams[] = [];
	for (let timer0 = options.minTimer0; timer0 <= options.maxTimer0; timer0++) {
		sc.timer0 = timer0;
		for (let vCount = options.minVCount; vCount <= options.maxVCount; vCount++) {
			sc.vCount = vCount;
			for (let vFrame = options.minVFrame; vFrame <= options.maxVFrame; vFrame++) {
				sc.vFrame = vFrame;

				if (seeds.indexOf(sc.getSeed()) != -1) {
					results.push({
						timer0: sc.timer0, vCount: sc.vCount, vFrame: sc.vFrame,
						mac: options.mac, datetime: options.datetime, is3DS: options.is3DS,
						buttons: 0
					});
				}
			}
		}
	}

	return results;
}
