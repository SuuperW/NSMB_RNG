import { Component, inject } from '@angular/core';
import { StepComponent } from '../step';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { WorkerService } from '../../worker.service';
import { RngParams } from '../../functions/rng-params-search';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { PopupDialogComponent } from '../../popup-dialog/popup-dialog.component';

@Component({
	selector: 'app-step5',
	standalone: true,
	templateUrl: './step5.component.html',
	styleUrls: ['./step5.component.css'],
	imports: [
		ReactiveFormsModule,
		MatDialogModule,
	],
})
export class Step5Component extends StepComponent {
	worker: WorkerService = inject(WorkerService);
	dialog: MatDialog = inject(MatDialog);

	form = new FormGroup({
		route: new FormControl('', (c: AbstractControl<string>) => {
			if (c.value) {
				return null;
			} else {
				return { 'err': 'Select a route.' };
			}
		}),
	});
	errorStatus?= 'Please wait';

	done: boolean = false;

	private disableForm() {
		// Angular complains if I use [disabled], and I don't understand why.
		this.form.controls.route.disable();
	}
	private enableForm() {
		this.form.controls.route.enable();
	}

	protected async runSearch() {
		const normalSeeds = [
			0xaa99ad81, 0x2aa12d89, 0xa2a1a589, 0xaaa3ad8b, 0xaa21ad09, 0xcaa1cd89, 0xaca1af89, 0x11281410,
			0x4433471b, 0xc43ac722, 0x3c3b3f23, 0x43bb46a3, 0x443d4725, 0x643b6723, 0x463b4923, 0xaac1ada9,
			0xddcce0b4, 0xd5d4d8bc, 0x5dd460bc, 0xddd6e0be, 0xdd54e03c, 0xfdd500bc, 0xdfd4e2bc, 0x445b4743,
			0x77667a4e, 0xf76dfa55, 0x6f6e7256, 0x76ee79d6, 0x77707a58, 0x976e9a56, 0x796e7c56, 0xddf4e0dc,
			0x110013e8, 0x910793ef, 0x09080bf0, 0x130815f0, 0x310833f0, 0x10881370, 0x110a13f2, 0x778e7a76,
		];
		const miniSeeds = [
			0x449b4783, 0x2aa02d88, 0xa921ac09, 0x0aa20d8a, 0x92a19589, 0xb0a1b389, 0x4423470b, 0xaaa7ad8f,
			0xde34e11c, 0xc439c721, 0x42bb45a3, 0x2c3b2f23, 0x4a3b4d23, 0xa43ba723, 0xddbce0a4, 0x44414729,
			0x77ce7ab6, 0x5dd360bb, 0xe3d4e6bc, 0x3dd540bd, 0x77567a3e, 0xc5d4c8bc, 0xdc54df3c, 0xdddae0c2,
			0x11681450, 0xf76cfa54, 0x75ee78d6, 0x5f6e6256, 0x7d6e8056, 0x10f013d8, 0xd76eda56, 0x77747a5c,
			0xab01ade9, 0x910693ee, 0xf907fbef, 0x0f881270, 0x170819f0, 0xaa89ad71, 0x710873f0, 0x110e13f6,
		];

		this.disableForm();
		this.errorStatus = 'Please wait';
		const status = 'Finding date and time for RNG manip... this may take a while'
		this.addProgress(status);
		let params: RngParams = this.guide.expectedParams!;
		try {
			let result = await this.worker.searchForTime(this.form.value.route == 'normal' ? normalSeeds : miniSeeds, params);

			if (result) {
				this.guide.expectedParams!.datetime = result;
				this.guide.paramsRange!.datetime = result;
				localStorage.setItem('manipDatetime', `${result.toDateString()} ${result.toLocaleTimeString()}`);

				localStorage.setItem('route', this.form.value.route!);
				this.errorStatus = undefined;
				this.done = true;
			} else {
				this.dialog.open(PopupDialogComponent, {
					data: {
						message: ['It seems there aren\'t any datetimes that work.',
							'It\'s very unlikely anyone will see this message unless something is broken.',
						],
					}
				});
			}
		} catch (ex) {
			this.dialog.open(PopupDialogComponent, {
				data: {
					message: [`Error: ${ex}`, 'Please try again.'],
				}
			});
			this.form.controls.route.setValue(null);
		}

		this.enableForm();
		this.removeProgress(status);
	}
}
