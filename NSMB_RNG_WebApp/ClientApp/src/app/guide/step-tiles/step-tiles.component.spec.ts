import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClient } from '@angular/common/http';

import { StepTilesComponent } from './step-tiles.component';
import { assert } from '../../../test/assert';
import { MockHttpClient } from '../../../test/mockHttpClient';
import { nextState, previousState } from '../../functions/rng';
import { GuideComponent } from '../guide.component';

describe('StepTilesComponent', () => {
	let component: StepTilesComponent;
	let fixture: ComponentFixture<StepTilesComponent>;
	let mockHttpClient: MockHttpClient;
	let firstSearchDate = new Date('2024-03-10 1:45:15');

	let tiles014515 = ['sccpbep', 'beebpcsbbep'];
	let tiles014514 = ['scbeipe', 'pbbiesiepee'];
	let tiles014415 = ['ecepbbp', 'ibsebicebbb'];
	let tilesOtherTime = ['pbpiecb', 'cbciesceibc']

	beforeEach(async () => {
		// We must set some localstorage for StepTiles to access.
		let mac = '40f407f7d421';
		let seconds = '15';
		localStorage.setItem('mac', mac);
		localStorage.setItem('consoleType', 'DS');
		localStorage.setItem('date', firstSearchDate.toISOString().split('T')[0]);
		localStorage.setItem('seconds', seconds);

		await TestBed.configureTestingModule({
			imports: [StepTilesComponent],
			providers: [{ provide: GuideComponent, useFactory: () => { return g; } }]
		})
			.compileComponents();
		// Using the providers property in configureTestingModule does not work for some reason I don't understand.
		// It ends up providing the mock to the component's services, but not to the component itself.
		// overrideProvider will provide the mock to the compoenent as well.
		mockHttpClient = new MockHttpClient();
		TestBed.overrideProvider(HttpClient, { useValue: mockHttpClient });

		let g = TestBed.createComponent(GuideComponent).componentInstance;
		fixture = TestBed.createComponent(StepTilesComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();

		// These are the seeds we should get after the second row, plus a few from just the first row.
		mockHttpClient.respondWith(`asp/seeds/${tiles014515[0]}`, [1243987243, 598903, 669698, 2866843, 5290143, 12298323, 12877138, 14800728, 22149413, 28947498, 28974698, 33585738]);
		mockHttpClient.respondWith(`asp/seeds/${tiles014415[0]}`, [299666798, 182118, 804998, 1711193, 3554028, 3856533, 4681333, 6004258, 6748058, 6862918, 7293338, 8046558]);
		mockHttpClient.respondWith(`asp/seeds/${tilesOtherTime[0]}`, [nextState(430237967), 489543, 706328, 1647918, 2235403, 4045058, 7342508, 8064998, 8230723, 8736488, 9636608]);

		mockHttpClient.respondWith('asp/submitResults', undefined);
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});

	it('can compute seeds', async () => {
		await component.row1Changed(tiles014515[0]);
		await component.row2Changed(tiles014515[1].toUpperCase());
		// The above pattern comes from known, hard-coded RNG params.
		// So, it will know the one specific seed that generated it.
		assert(component.seeds.length === 1, `Found ${component.seeds.length} seeds for pattern from known RNG params.`);

		// This pattern doesn't. So it'll have multiple possible seeds.
		await component.row1Changed(tilesOtherTime[0]);
		await component.row2Changed(tilesOtherTime[1].toUpperCase());
		assert(component.seeds.length === 5, `Found ${component.seeds.length} seeds for valid pattern from unknown RNG params.`);

	});

	it('can progress with good patterns', async () => {
		await component.row1Changed(tiles014515[0]);
		await component.row2Changed(tiles014515[1]);
		await component.submit();

		await component.row1Changed(tiles014415[0]);
		await component.row2Changed(tiles014415[1]);
		await component.submit();

		assert(component.errorStatus === undefined); // no error indicates user can proceed to next step
	});

	it('recognizes bad pattern', async () => {
		await component.row1Changed(tilesOtherTime[0]);
		await component.row2Changed(tilesOtherTime[1]);
		await component.submit();

		assert(component.resultManager.submitCount == 1);
		assert(component.resultManager.distinctParamsCount == 0);
	}, 15000); // submit will do a full search, which may take longer than the default 5 second limit

	it('recognizes +1/-1 second on past patterns after finding params', async () => {
		// simulate submission of -1 second pattern:
		component.resultManager.emptySearches = [{
			row1: tiles014514[0],
			row2: tiles014514[1],
			seeds: [...previousState(nextState(1083732095))],
			time: firstSearchDate
		}];
		(component.resultManager as any).submitCount = 1;
		(component.resultManager as any).distinctParamsCount = 0;

		await component.row1Changed(tiles014515[0]);
		await component.row2Changed(tiles014515[1]);
		await component.submit();

		assert(component.resultManager.distinctParamsCount == 2, `Found ${component.resultManager.distinctParamsCount}, expected 2`);
	});
});
