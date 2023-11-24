/*
*	Protractor support is deprecated in Angular.
*	Protractor is used in this example for compatibility with Angular documentation tools.
*/
import { bootstrapApplication, provideProtractorTestingSupport } from '@angular/platform-browser';
import { enableProdMode, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';

import { AppComponent } from './app/app.component';
import { environment } from './environments/environment';
import routeConfig from './routeConfig';
import { HttpClientModule } from '@angular/common/http';

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
		importProvidersFrom(HttpClientModule),
	],
})
.catch(err => console.error(err));
