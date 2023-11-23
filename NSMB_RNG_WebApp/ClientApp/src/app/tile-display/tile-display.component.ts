import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

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
	@Input() tileCount: number = 0;
	@Input() row1Input: string | null | undefined = '';

	getImageSrc(index: number) {
		if (!this.row1Input)
			return 'assets/none.png';

		let strIndex: number = -1;
		let charIndex: number = -1;
		while (charIndex != index) {
			strIndex++;
			if (strIndex == this.row1Input.length)
				return 'assets/none.png';
			if (this.row1Input[strIndex] != ' ')
				charIndex++;
		}

		if (index >= this.tileCount)
			return 'assets/tooLong.png';

		const validChars: string[] = ['B', 'C', 'E', 'I', 'P', 'S'];
		const tileChar = this.row1Input[strIndex].toUpperCase();
		if (validChars.indexOf(tileChar) != -1)
			return `assets/tile${tileChar}.png`;
		else
			return 'assets/invalid.png';
	}
}
