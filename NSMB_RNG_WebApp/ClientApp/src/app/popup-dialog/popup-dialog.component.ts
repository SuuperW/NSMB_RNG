import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export type PopupDialogData = {
	message: string[],
};

@Component({
  selector: 'app-popup-dialog',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './popup-dialog.component.html',
  styleUrls: ['./popup-dialog.component.css']
})
export class PopupDialogComponent {
	public constructor(
		public dialogRef: MatDialogRef<PopupDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public data: PopupDialogData) { }

	public onOkClick() {
		this.dialogRef.close();
	}
}
