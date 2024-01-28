import { SearchParams, getAllPossibleRow1 } from "../functions/rng-params-search";

type match = {
	seed: number,
	seconds: number,
}
type _match = match & {
	remainingTiles: string,
}
type _tileTree = {
	// key should be one character; a single tile
	[key: string]: _tileTree | _match
}
let isMatch = (o: any) => {
	return o.seed !== undefined;
}

export class PrecomputedPatterns {
	private tree: _tileTree = {};

	private updateMatchMap(m: _tileTree, match: _match) {
		// edge cases: no more tiles
		if (match.remainingTiles.length == 0)
			return;

		// Get map next level down.
		let nextTile = match.remainingTiles[0];
		match.remainingTiles = match.remainingTiles.substring(1);
		let next = m[nextTile];
		// Update it.
		if (next === undefined)
			m[nextTile] = match;
		else if (isMatch(next)) {
			let newMap = {}
			this.updateMatchMap(newMap, next as _match)
			this.updateMatchMap(newMap, match);
			m[nextTile] = newMap;
		} else {
			this.updateMatchMap(next as _tileTree, match);
		}
	}

	addParams(params: SearchParams, maxOffset: number = 1) {
		for (let i = -maxOffset; i <= maxOffset; i++) {
			// create searchParams
			let sp = new SearchParams(params);
			sp.datetime.setSeconds(sp.datetime.getSeconds() + i);
			// get seeds, row1s
			let all = getAllPossibleRow1(sp);
			for (let p of all) {
				this.updateMatchMap(this.tree, {
					seed: p.seed,
					seconds: i,
					remainingTiles: p.pattern, // this should be upper case
				});
			}
		}
	}

	getPatternInfo(row1: string): { ambiguous: boolean, match?: match } {
		if (row1.length > 7) return { ambiguous: false };
		if (row1.length < 2) return { ambiguous: true };

		row1 = row1.toUpperCase();
		let tree = this.tree;
		for (let i = 0; i < row1.length; i++) {
			let t = row1[i];
			let next = tree[t];
			if (next === undefined)
				return { ambiguous: false };
			if (isMatch(next)) {
				// Only one pre-computed pattern matches up to this point. Does it match the rest of the given row1?
				let remainingTiles = row1.substring(i + 1);
				if (!(next as _match).remainingTiles.startsWith(remainingTiles))
					return { ambiguous: false };
				return {
					ambiguous: false,
					match: next as match,
				}
			}
			tree = next as _tileTree;
		}

		// Any unambiguous matches would be returned from inside the loop.
		return { ambiguous: true };
	}
}
