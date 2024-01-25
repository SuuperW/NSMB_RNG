import { Component, inject } from '@angular/core';
import { StepComponent } from '../step';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { PopupDialogComponent } from '../../popup-dialog/popup-dialog.component';

let NintendoMacPrefixes = new Set([
	'0009BF', '001656', '0017AB', '00191D', '0019FD', '001AE9', '001B7A', '001BEA',
	'001CBE', '001DBC', '001E35', '001EA9', '001F32', '001FC5', '002147', '0021BD',
	'00224C', '0022AA', '0022D7', '002331', '0023CC', '00241E', '002444', '0024F3',
	'0025A0', '002659', '002709', '0403D6', '182A7B', '1C4586', '200BCF', '201C3A',
	'28CF51', '2C10C1', '342FBD', '34AF2C', '40D28A', '40F407', '483177', '48A5E7',
	'50236D', '582F40', '58B03E', '58BDA3', '5C0CE6', '5C521E', '601AC7', '606BFF',
	'64B5C6', '702C09', '7048F7', '70F088', '748469', '74F9CA', '7820A5', '78A2A0',
	'7CBB8A', '80D2E5', '8C56C5', '8CCDE8', '904528', '9458CB', '98415C', '98B6E9',
	'98E8FA', '9CE635', 'A438CC', 'A45C27', 'A4C0E1', 'B87826', 'B88AEC', 'B8AE6E',
	'BC744B', 'BC9EBB', 'BCCE25', 'CC5B31', 'CC9E00', 'CCFB65', 'D05509', 'D4F057',
	'D86BF7', 'DC68EB', 'DCCD18', 'E00C7F', 'E0E751', 'E0F6B5', 'E84ECE', 'E8A0CD',
	'E8DA20', 'ECC40D',
]);

@Component({
	selector: 'app-step2',
	standalone: true,
	templateUrl: './step2.component.html',
	styleUrls: ['./step2.component.css'],
	imports: [
		ReactiveFormsModule,
		MatDialogModule,
	],
})
export class Step2Component extends StepComponent {
	dialog: MatDialog = inject(MatDialog);

	lastValidation: string = '';
	form = new FormGroup({
		mac: new FormControl(localStorage.getItem('mac') ?? '00:09:bf:00:00:00', (c: AbstractControl<string>) => {
			let m = c.value.trim();
			if (m.match(/^([\dabcdef]{2}:){5}[\dabcdef]{2}$/i) || m.match(/^[\dabcdef]{12}$/i)) {
				if (m == this.lastValidation) return null;
				this.lastValidation = m;

				let noSep = m.length == 12 ? m : `${m.substring(0, 2)}${m.substring(3, 5)}${m.substring(6, 8)}`;
				let prefix = noSep.substring(0, 6).toUpperCase();
				if (!NintendoMacPrefixes.has(prefix)) {
					this.dialog.open(PopupDialogComponent, {
						data: {
							message: ['Your MAC address is valid but doesn\'t belong to a Nintendo device.',
								'Make sure you have entered it correctly.',
							],
						}
					});
				}
				localStorage.setItem('mac', m);
				return null;
			}
			else
				return { err: 'MAC address is invalid' };
		}),

	});
}
