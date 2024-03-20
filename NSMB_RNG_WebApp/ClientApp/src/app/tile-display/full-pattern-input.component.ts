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
	public rowsForm = new FormGroup({
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

	public backspace() {
		let control = this.rowsForm.controls.row2Input;
		if ((control.value?.length ?? 0) === 0) {
			control = this.rowsForm.controls.row1Input;
		}
		const oldValue = control.value ?? '';
		control.setValue(oldValue.substring(0, oldValue.length - 1));
	}

	private doAutocomplete() {
		if (this.autocomplete) {
			let precomputedResult = this.autocomplete.getPatternInfo(this.row1, this.row2);
			let enoughTilesEntered = precomputedResult.extraTiles! >= this.autocompleteRequireExtraTiles;
			if (!precomputedResult.ambiguous && precomputedResult.match && enoughTilesEntered) {
				this.setSeeds([precomputedResult.match.seed]);

				let row2 = getRow2(precomputedResult.match.seed);
				this._row2SetByAutocomplete = true;
				this._changedByUser = false; // short-circuit event handler
				if (row2 != this.row2Input) {
					this.row2Input = row2;
					this.cdr.detectChanges(); // triggers event handler
				}
				let row1 = getRow1(precomputedResult.match.seed);
				if (this.row1Input != row1) {
					this.row1Input = row1;
					this.cdr.detectChanges();
				}
				this._changedByUser = true;
				
			} else if (this._row2SetByAutocomplete) {
				this.row2Input = this.rowsForm.value.row2Input ?? '';
			}
			this._patternChanged.trigger(precomputedResult);
		} else
			this._patternChanged.trigger(null);
	}

	protected async row1Changed(tiles: string) {
		this.row1 = tiles;
		if (!this._changedByUser)
			return;

		this.seeds = [];
		this.networkError = false;

		this.doAutocomplete();

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
	protected async row2Changed(tiles: string) {
		this.patternIsInvalid = false;
		this.row2 = tiles;
		if (!this._changedByUser)
			return;

		this.seeds = [];
		this.bottomRows = [''];
		this._row2SetByAutocomplete = false;
		let r2cc = ++this.r2cc;

		// If row2Input is invalid, row2 will be a blank string. So autocomplete might mistakenly find something!
		if (tiles !== '' || this.row2Input.length === 0)
			this.doAutocomplete();
		else
			this._patternChanged.trigger(null);

		if (tiles.length == 11 && this.row1.length == 7 && !this._row2SetByAutocomplete) {
			this.computingLastRow = true;
			let seeds = await this.seedService.getPossibleSeedsFor(this.row1, tiles) as number[];

			if (r2cc != this.r2cc)
				return; // User triggered this method again before getPossibleSeedsFor returned.

			if (seeds.length == 0)
				this.patternIsInvalid = true;
			this.setSeeds(seeds);
		}

		this.computingLastRow = false;
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
