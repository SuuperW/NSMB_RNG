import { Routes } from '@angular/router';
import { FetchDataComponent } from './app/fetch-data/fetch-data.component';
import { CounterComponent } from './app/counter/counter.component';
import { Error404Component } from './app/error404/error404.component';
import { HomeComponent } from './app/home/home.component';

const routeConfig: Routes = [
	{
		path: '',
		component: HomeComponent,
		title: 'Home',
		pathMatch: 'full',
	},
	{
		path: 'weather',
		component: FetchDataComponent,
		title: 'Weather Data',
	},
	{
		path: 'counter',
		component: CounterComponent,
		title: 'Counter',
	},
	{ // No route matches
		path: '**',
		component: Error404Component,
		title: 'Error 404',
	},
];
export default routeConfig;
