import { SearchParams, getAllPossibleRow1and2 } from "../functions/rng-params-search";

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

export type PatternMatchInfo = { ambiguous: boolean, match?: match, extraTiles?: number };

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
			let all = getAllPossibleRow1and2(sp);
			for (let p of all) {
				this.updateMatchMap(this.tree, {
					seed: p.seed,
					seconds: i,
					remainingTiles: p.pattern, // this should be upper case
				});
			}
		}
	}

	getPatternInfo(row1: string, row2: string = ''): PatternMatchInfo {
		if (row1.length > 7 || row2.length > 11) return { ambiguous: false };
		if (row1.length < 2) return { ambiguous: true };

		let tiles = row1.toUpperCase();
		if (row1.length == 7)
			tiles += row2.toUpperCase();
		else if (row2.length !== 0)
			// If we do not have a full row 1 but do have something in row 2, let's not try autocompleting.
			return { ambiguous: false };

		let tree = this.tree;
		for (let i = 0; i < tiles.length; i++) {
			let t = tiles[i];
			let next = tree[t];
			if (next === undefined)
				return { ambiguous: false };
			if (isMatch(next)) {
				// Only one pre-computed pattern matches up to this point. Does it match the rest of the given tiles?
				let remainingTiles = tiles.substring(i + 1);
				if (!(next as _match).remainingTiles.startsWith(remainingTiles))
					return { ambiguous: false };
				return {
					ambiguous: false,
					match: next as match,
					extraTiles: remainingTiles.length,
				}
			}
			tree = next as _tileTree;
		}

		// Any unambiguous matches would be returned from inside the loop.
		return { ambiguous: true };
	}
}
