import { SeedCalculator } from '../seed-calculator';
import { getRow1 } from './tiles';

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
export class SearchParams implements SearchParamsParams {
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

export const searchForTime = (seeds: Set<number>, params: RngParams, minYear: number, maxYearExclusive: number) => {
	let dt = new Date(minYear, 0, 1, 0, 0, params.datetime.getSeconds());
	let sc = new SeedCalculator(params.mac, dt, params.is3DS);
	sc.timer0 = params.timer0;
	sc.vCount = params.vCount;
	sc.vFrame = params.vFrame;
	sc.buttons = params.buttons;

	while (dt.getFullYear() < maxYearExclusive) {
		for (let hour = 0; hour < 24; hour++) {
			sc.hour = hour;
			for (let min = 0; min < 60; min++) {
				sc.minute = min;
				if (seeds.has(sc.getSeed())) {
					dt.setHours(hour);
					dt.setMinutes(min);
					return dt;
				}
			}
		}

		dt.setDate(dt.getDate() + 1);
		sc.setDateTime(dt);
	}

	return null;
}

export type SeedRow = { pattern: string, seed: number };
export const getAllPossibleRow1 = (options: SearchParams) => {
	let sc = new SeedCalculator(options.mac, options.datetime, options.is3DS);
	sc.buttons = options.buttons;
	let results: SeedRow[] = [];

	for (let timer0 = options.minTimer0; timer0 <= options.maxTimer0; timer0++) {
		sc.timer0 = timer0;
		for (let vCount = options.minVCount; vCount <= options.maxVCount; vCount++) {
			sc.vCount = vCount;
			for (let vFrame = options.minVFrame; vFrame <= options.maxVFrame; vFrame++) {
				sc.vFrame = vFrame;
				let seed = sc.getSeed();
				results.push({ pattern: getRow1(seed), seed: seed });
			}
		}
	}

	return results;
}
