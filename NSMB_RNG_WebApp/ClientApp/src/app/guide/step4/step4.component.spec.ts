import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClient } from '@angular/common/http';

import { Step4Component } from './step4.component';
import { assert } from '../../../test/assert';
import { MockHttpClient } from '../../../test/mockHttpClient';
import { nextState, previousState } from '../../functions/rng';
import { GuideComponent } from '../guide.component';

describe('Step4Component', () => {
	let component: Step4Component;
	let fixture: ComponentFixture<Step4Component>;
	let mockHttpClient: MockHttpClient;

	beforeEach(async () => {
		// We must set some localstorage for step4 to access.
		let mac = '40f407f7d421';
		let dtStr = '2023-11-24 01:00:15';
		localStorage.setItem('mac', mac);
		localStorage.setItem('datetime', dtStr);

		await TestBed.configureTestingModule({
			imports: [Step4Component],
			providers: [{ provide: GuideComponent, useFactory: () => { return g; } }]
		})
			.compileComponents();
		// Using the providers property in configureTestingModule does not work for some reason I don't understand.
		// It ends up providing the mock to the component's services, but not to the component itself.
		// overrideProvider will provide the mock to the compoenent as well.
		mockHttpClient = new MockHttpClient();
		TestBed.overrideProvider(HttpClient, { useValue: mockHttpClient });

		let g = TestBed.createComponent(GuideComponent).componentInstance;
		fixture = TestBed.createComponent(Step4Component);
		component = fixture.componentInstance;
		fixture.detectChanges();

		// These are the seeds we should get after the second row, plus a few from just the first row.
		mockHttpClient.respondWith('asp/seeds/ebeseee', [nextState(749423222), nextState(793632109), 204313, 298288, 358738, 578838, 618623, 645493, 788968, 1082073, 1126763, 1464388]);
		mockHttpClient.respondWith('asp/seeds/bsbcsep', [nextState(16632510), 695113, 857873, 887558, 1984193, 2071198, 2373313, 3360278, 3983158, 4557908, 4889353]);
		mockHttpClient.respondWith('asp/seeds/siebeib', [nextState(781790593), 1135733, 1220573, 2013633, 3205518, 3427573, 4142948, 4400183, 5319788, 5462678, 5565468]);
		mockHttpClient.respondWith('asp/seeds/ebeepbi', [nextState(365998418), 486313, 714973, 2393298, 2542503, 3380138, 3415408, 4315598, 5080493, 5345848, 5983538]);
		mockHttpClient.respondWith('asp/seeds/pbpiecb', [nextState(430237967), 489543, 706328, 1647918, 2235403, 4045058, 7342508, 8064998, 8230723, 8736488, 9636608]);

		mockHttpClient.respondWith('asp/submitResults', undefined);

		// Since we'll be submitting, the component will search for rng params.
		// A full search would take a long time; let's not do that.
		// This is also where we set the date, system, and MAC address for the tests.
		(component.resultManager as any).range = {
			buttons: 0,
			datetime: new Date(dtStr),
			is3DS: false,
			mac: mac,
			minTimer0: 1375,
			maxTimer0: 1390,
			minVCount: 35,
			maxVCount: 45,
			minVFrame: 5,
			maxVFrame: 5,
		};
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});

	it('can compute seeds', async () => {
		await component.row1Changed('ebeseee');
		await component.row2Changed('isbepebibbs'.toUpperCase());
		// The above pattern comes from known, hard-coded RNG params.
		// So, it will know the one specific seed that generated it.
		assert(component.seeds.length === 1, `Found ${component.seeds.length} seeds for pattern from known RNG params.`);

		// This pattern doesn't. So it'll have multiple possible seeds.
		await component.row1Changed('pbpiecb');
		await component.row2Changed('cbciesceibc'.toUpperCase());
		assert(component.seeds.length === 5, `Found ${component.seeds.length} seeds for valid pattern from unknown RNG params.`);

	});

	it('can progress with good patterns', async () => {
		await component.row1Changed('ebeseee');
		await component.row2Changed('isbepebibbs');
		await component.submit();

		await component.row1Changed('bsbcsep');
		await component.row2Changed('beebiicsspi');
		await component.submit();

		await component.row1Changed('ebeseee');
		await component.row2Changed('isbepebibbs');
		await component.submit();

		assert(component.errorStatus === undefined); // no error indicates user can proceed to next step
	});

	it('recognizes bad pattern', async () => {
		await component.row1Changed('pbpiecb');
		await component.row2Changed('cbciesceibc');
		await component.submit();

		assert(component.resultManager.submitCount == 1);
		assert(component.resultManager.totalMatchedPatterns == 0);
	}, 15000); // submit will do a full search, which may take longer than the default 5 second limit

	it('recognizes +1/-1 second on past patterns after finding params', async () => {
		// simulate submission of -1 second pattern:
		(component.resultManager as any).results = [{
			count: 1,
			result: [],
			row1: 'siebeib',
			row2: 'piibseceipe',
			seeds: [...previousState(nextState(781790593))],
		}];
		(component.resultManager as any).submitCount = 1;
		(component.resultManager as any).totalMatchedPatterns = 0;

		await component.row1Changed('ebeseee');
		await component.row2Changed('isbepebibbs');
		await component.submit();

		assert(component.resultManager.totalMatchedPatterns == 2);
	});
});
