import { Component, inject } from '@angular/core';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
	selector: 'app-fetch-data',
	standalone: true,
	templateUrl: './fetch-data.component.html',
	imports: [
		CommonModule,
		HttpClientModule,
	],
})
export class FetchDataComponent {
	public forecasts: WeatherForecast[] = [];
	http: HttpClient = inject(HttpClient);

	constructor() {
		this.http.get<WeatherForecast[]>(window.location.origin + '/weather').subscribe({
			next: (result) => this.forecasts = result,
			error: (error) => console.error(error),
		});
	}
}

interface WeatherForecast {
	date: string;
	temperatureC: number;
	temperatureF: number;
	summary: string;
}
