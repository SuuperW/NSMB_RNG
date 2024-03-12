import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { TileDisplayComponent } from './tile-display.component';
import { PatternMatchInfo, PrecomputedPatterns } from '../guide/precomputed-patterns';
import { getRow1, getRow2 } from '../functions/tiles';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { SeedTileCalculatorService } from '../seeds-tile-calculator.service';
import { CommonModule } from '@angular/common';
import { SimpleEvent } from '../event';

@Component({
	selector: 'app-full-pattern-input',
	standalone: true,
	imports: [
		TileDisplayComponent,
		ReactiveFormsModule,
		CommonModule,
	],
	templateUrl: './full-pattern-input.component.html',
})
export class FullPatternInputComponent {
	private seedService: SeedTileCalculatorService = inject(SeedTileCalculatorService);

	public autocomplete: PrecomputedPatterns | undefined;
	public autocompleteRequireExtraTiles: number = 0;

	// I'm using the control validators as change handlers.
	// No other method of change handling that I found both worked and triggered immediately upon typing a character.
	protected rowsForm = new FormGroup({
		row1Input: new FormControl('', (c: AbstractControl<string>) => { this.row1Input = c.value; return null; }),
		row2Input: new FormControl('', (c: AbstractControl<string>) => { this.row2Input = c.value; return null; }),
	});
	// These represent the input to the tile display components. These might not ve valid.
	// We do not use the values from the form directly because we don't want auto-complete to change the text fields.
	protected row1Input: string = '';
	protected row2Input: string = '';
	// These represent the output of the tile display components, which must always be a valid pattern.
	private row1: string = '';
	private row2: string = '';
	protected bottomRows: string[] = [];
	public getPattern() { return [this.row1, this.row2]; }

	private _patternChanged: SimpleEvent<PatternMatchInfo | null> = new SimpleEvent<PatternMatchInfo | null>();
	public get patternChanged() { return this._patternChanged.expose(); }

	public seeds: number[] = [];

	protected patternIsInvalid: boolean = false;
	protected networkError: boolean = false;
	protected computingLastRow: boolean = false;

	private _changedByUser = true;
	private _row2SetByAutocomplete = false;

	constructor(private cdr: ChangeDetectorRef) {}

	public clear() {
		this.rowsForm.controls.row1Input.setValue('');
		this.rowsForm.controls.row2Input.setValue('');
		this.row1 = this.row2 = '';
	}

	public appendTile(tile: string) {
		let control = this.rowsForm.controls.row1Input;
		if ((this.rowsForm.value.row1Input?.length ?? 0) >= 7) {
			control = this.rowsForm.controls.row2Input;
		}
		control.setValue((control.value ?? '') + tile);
	}

	protected async row1Changed(tiles: string) {
		this.row1 = tiles;
		this.seeds = [];
		this.networkError = false;

		// Auto-complete
		if (this.autocomplete) {
			let precomputedResult = this.autocomplete.getPatternInfo(tiles);
			let enoughTilesEntered = precomputedResult.extraTiles! >= this.autocompleteRequireExtraTiles;
			if (!precomputedResult.ambiguous && precomputedResult.match && enoughTilesEntered) {
				this._row2SetByAutocomplete = true;
				let row2 = getRow2(precomputedResult.match.seed);
				// _changedByUser being false will short-circuit the event handler.
				// However, the event handler does not happen until later on so we cannot
				// un-set _changedByUser afterwards. The handler must do that for us.
				// Additionally, the event handler will only be triggered if the set value is different from the current value.
				if (row2 != this.row2Input) this._changedByUser = false;
				this.row2Input = row2;
				this.setSeeds([precomputedResult.match.seed]);
				this.row1Input = getRow1(precomputedResult.match.seed);
				this.cdr.detectChanges();
			} else if (this._row2SetByAutocomplete) {
				this.row2Input = '';
				this._row2SetByAutocomplete = false;
			}
			this._patternChanged.trigger(precomputedResult);
		} else
			this._patternChanged.trigger(null);

		if (tiles.length != 7) {
			return;
		}

		// We will need to process the second row if it is full and not auto-completed.
		let process = this.row2.length == 11 && !this._row2SetByAutocomplete;
		if (process)
			this.computingLastRow = true; // Displays the spinner; do this before getPossibleSeedsFor

		// We need to get the seeds before processing second row
		let result = await this.seedService.getPossibleSeedsFor(tiles);
		if (result.length == 0) {
			this.networkError = true;
			this.computingLastRow = false;
			return;
		}

		if (process)
			this.row2Changed(this.row2);
	}

	r2cc = 0;
	async row2Changed(tiles: string) {
		this.patternIsInvalid = false;
		this.row2 = tiles;
		let r2cc = ++this.r2cc;

		if (!this._changedByUser) {
			// Angular will not call this handler at the time the value is set. The handler will get called later.
			// Thus, the caller cannot handle setting _changedByUser back to true. We must do it here.
			this._changedByUser = true;
			return;
		}

		this.seeds = [];
		this.bottomRows = [''];
		this._row2SetByAutocomplete = false;

		if (tiles.length == 11 && this.row1.length == 7) {
			this.computingLastRow = true;
			let seeds = await this.seedService.getPossibleSeedsFor(this.row1, tiles) as number[];

			if (r2cc != this.r2cc)
				return; // User triggered this method again before getPossibleSeedsFor returned.

			if (seeds.length == 0)
				this.patternIsInvalid = true;
			this.setSeeds(seeds);
		}

		this.computingLastRow = false;
		this._patternChanged.trigger(null);
	}

	private setSeeds(seeds: number[]) {
		this.seeds = seeds;

		let rows: Set<string> = new Set();
		this.bottomRows = [];
		for (let seed of this.seeds)
			rows.add(this.seedService.getBottomRow(seed));
		for (let row of rows)
			this.bottomRows.push(row);
	}
}
