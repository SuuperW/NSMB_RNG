import { Component } from '@angular/core';
import { EventEmitter, Output } from '@angular/core';

@Component({
	selector: 'app-clickable-tiles',
	standalone: true,
	imports: [],
	templateUrl: './clickable-tiles.component.html',
})
export class ClickableTilesComponent {
	@Output() protected tileClicked: EventEmitter<string> = new EventEmitter();

	tileClick(letter: string) {
		this.tileClicked.emit(letter);
	}
}
