import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { TilePreloaderService } from '../tile-preloader.service';

const noTile = 'assets/none.png';

@Component({
	selector: 'app-tile-display',
	standalone: true,
	imports: [
		CommonModule,
	],
	templateUrl: './tile-display.component.html',
	styleUrls: ['./tile-display.component.css'],
})
export class TileDisplayComponent {
	private _preloader: TilePreloaderService = inject(TilePreloaderService)

	private _tileCount: number = 0;
	@Input() set tileCount(value: number) {
		this._tileCount = value;
		this.imageSrc = new Array(value + 1);
		this.tiles = this._tiles;
	}
	get tileCount() {
		return this._tileCount;
	}

	_tiles: string = '';
	@Input() set tiles(value: string | null | undefined) {
		if (!value)
			value = '';

		const validChars: string[] = ['B', 'C', 'E', 'I', 'P', 'S'];
		let strIndex: number = 0;
		let charIndex: number = -1;
		let outTiles = '';
		let valid = true;
		while (charIndex < this._tileCount && strIndex < value.length) {
			if (value[strIndex] != ' ') {
				charIndex++;
				const tileChar = value[strIndex].toUpperCase();
				outTiles = outTiles + tileChar;
				if (validChars.indexOf(tileChar) != -1)
					this.imageSrc[charIndex] = `assets/tile${tileChar}.png`;
				else {
					this.imageSrc[charIndex] = 'assets/invalid.png';
					valid = false;
				}
			}
			strIndex++;
		}

		if (charIndex == this._tileCount) {
			valid = false;
			this.imageSrc[this._tileCount] = 'assets/tooLong.png';
		}
		else {
			while (charIndex < this._tileCount) {
				charIndex++;
				this.imageSrc[charIndex] = noTile;
			}
		}

		if (valid) {
			this.tilesOut.emit(outTiles);
			this._tiles = outTiles;
		}
		else
			this.tilesOut.emit('');
	}

	private imageSrc: string[] = [noTile];


	@Output() tilesOut = new EventEmitter<string>();

	getImageSrc(index: number) {
		if (index >= 0 && index < this.imageSrc.length)
			return this.imageSrc[index];
		else
			return noTile;
	}
}
