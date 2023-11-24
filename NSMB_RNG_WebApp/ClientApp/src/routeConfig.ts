import { Routes } from '@angular/router';
import { Error404Component } from './app/error404/error404.component';
import { HomeComponent } from './app/home/home.component';
import { SeedParamsFinderComponent } from './app/seed-params-finder/seed-params-finder.component';
import { GuideComponent } from './app/guide/guide.component';

const routeConfig: Routes = [
	{
		path: '',
		component: HomeComponent,
		title: 'Home',
		pathMatch: 'full',
	},
	{
		path: 'all',
		component: SeedParamsFinderComponent,
		title: 'Seed Params Finder',
	},
	{
		path: 'guide',
		component: GuideComponent,
		title: 'Guide',
	},
	{ // No route matches
		path: '**',
		component: Error404Component,
		title: 'Error 404',
	},
];
export default routeConfig;
