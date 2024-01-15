import { defer } from "rxjs";

export class MockHttpClient {
	private responses: {[key: string]: any[]} = {};

	get(url: string) {
		return defer(() => Promise.resolve(this.responses[url].shift()));
	}
	post(url: string) {
		return this.get(url);
	}

	respondWith(url: string, content: any) {
		if (!(url in this.responses)) {
			this.responses[url] = [];
		}

		this.responses[url].push(content);
	}
}
