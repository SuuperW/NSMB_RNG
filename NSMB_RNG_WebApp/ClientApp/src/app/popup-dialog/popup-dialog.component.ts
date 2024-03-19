import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export type PopupDialogData = {
	message: string[],
	buttons?: string[],
	buttonHandler?: (text: string) => void,
};

@Component({
  selector: 'app-popup-dialog',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './popup-dialog.component.html',
  styleUrls: ['./popup-dialog.component.css']
})
export class PopupDialogComponent {
	protected buttons: string[];
	private externalHandler?: (text: string) => void;

	public constructor(
		public dialogRef: MatDialogRef<PopupDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public data: PopupDialogData) {
			this.buttons = data.buttons ?? ['OK'];
			this.externalHandler = data.buttonHandler;
		}

	protected onButtonClick(buttonText: string) {
		this.dialogRef.close();
		if (this.externalHandler) this.externalHandler(buttonText);
	}
}
