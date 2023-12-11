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
type SearchParamsParams = {
	mac: string;
	datetime: Date;
	minTimer0: number;
	maxTimer0: number;
	minVCount?: number;
	maxVCount?: number;
	minVFrame?: number;
	maxVFrame?: number;
	is3DS?: boolean;
	buttons?: number;
}
export class SearchParams {
	mac: string;
	datetime: Date;
	minTimer0: number;
	maxTimer0: number;
	minVCount: number;
	maxVCount: number;
	minVFrame: number;
	maxVFrame: number;
	is3DS: boolean;
	buttons: number;

	public constructor({
		mac, datetime, minTimer0, maxTimer0,
		minVCount = 0,
		maxVCount = 263,
		minVFrame = 4,
		maxVFrame = 8,
		is3DS = false,
		buttons = 0,
	}: SearchParamsParams) {
		this.mac = mac;
		this.datetime = datetime;
		this.minTimer0 = minTimer0;
		this.maxTimer0 = maxTimer0;
		this.minVCount = minVCount;
		this.maxVCount = maxVCount;
		this.minVFrame = minVFrame;
		this.maxVFrame = maxVFrame;
		this.is3DS = is3DS;
		this.buttons = buttons;
	}
};

export const searchForSeeds = (seeds: number[], options: SearchParams) => {
	let sc = new SeedCalculator(options.mac, options.datetime, options.is3DS);
	sc.buttons = options.buttons;
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
						buttons: options.buttons,
					});
				}
			}
		}
	}

	return results;
}
