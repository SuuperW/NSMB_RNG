import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClickableTilesComponent } from './clickable-tiles.component';

describe('ClickableTilesComponent', () => {
	let component: ClickableTilesComponent;
	let fixture: ComponentFixture<ClickableTilesComponent>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			imports: [ClickableTilesComponent]
		})
		.compileComponents();
		
		fixture = TestBed.createComponent(ClickableTilesComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
