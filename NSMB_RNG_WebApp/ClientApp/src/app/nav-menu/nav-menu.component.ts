import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
	selector: 'app-nav-menu',
	standalone: true,
	templateUrl: './nav-menu.component.html',
	styleUrls: ['./nav-menu.component.css'],
	imports: [
		CommonModule,
		RouterModule,
	],
})
export class NavMenuComponent {
	isExpanded = false;

	collapse() {
		this.isExpanded = false;
	}

	toggle() {
		this.isExpanded = !this.isExpanded;
	}
}
