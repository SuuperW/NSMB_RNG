/*
*	Protractor support is deprecated in Angular.
*	Protractor is used in this example for compatibility with Angular documentation tools.
*/
import { bootstrapApplication, provideProtractorTestingSupport } from '@angular/platform-browser';
import { enableProdMode } from '@angular/core';
import { provideRouter } from '@angular/router';

import { AppComponent } from './app/app.component';
import { environment } from './environments/environment';
import routeConfig from './routeConfig';

export function getBaseUrl() {
	return document.getElementsByTagName('base')[0].href;
}

const providers = [
	{ provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] }
];

if (environment.production) {
	enableProdMode();
}

bootstrapApplication(AppComponent, {
	providers: [
		provideProtractorTestingSupport(),
		provideRouter(routeConfig),
	],
})
.catch(err => console.error(err));
