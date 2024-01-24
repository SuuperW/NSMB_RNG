import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TilePreloaderService {
	constructor() {
		let tiles = ['B', 'C', 'E', 'I', 'P', 'S'];
		let nonTiles = ['none', 'tooLong', 'invalid'];
		for (let t of tiles) {
			let img = new Image();
			img.src = `assets/tile${t}.png`;
		}
		for (let t of nonTiles) {
			let img = new Image();
			img.src = `assets/${t}.png`;
		}
	}
}
