import { Injectable } from "@angular/core";
import * as rng from "./functions/rng";

let workerSupported: boolean = !!window.Worker;
if (!workerSupported) {
	// Should we really be concerned about this? caniuse says 98% of users will have a browser that supports workers.
	alert('Your browser doesn\'t support web workers. The page will freeze during computations, which may make it seem like the web page has crashed.');
}

type dict = { [key: string]: any };

@Injectable({
	providedIn: 'root',
})
// A service that wraps a web worker, allowing outside code to call worker methods as async methods.
export class WorkerService {
	private worker: Worker | undefined;
	private resolvers: { [i: number]: ((value: dict | PromiseLike<dict>) => void) } = {};
	private lastId = 0;

	constructor() {
		if (workerSupported) {
			this.worker = new Worker(new URL('app.worker.ts', import.meta.url));
			// When the worker is done with its computation, it'll send a message.
			// We inspect the ID and resolve the corresponding Promise.
			this.worker.onmessage = ({ data }) => {
				if (this.resolvers[data.id]) {
					this.resolvers[data.id](data.data);
					delete this.resolvers[data.id];
				} else {
					throw 'Invalid id received from worker.';
				}
			};
		}
	}

	private async call(functionName: string, copyData: any) {
		this.lastId++;

		// Create a Promise that we can resolve in the worker's onmessage handler.
		let id = this.lastId;
		let this2 = this;
		let promise = new Promise<dict>(function (resolve, _) {
			this2.resolvers[id] = resolve;
		});

		// Send the message and await the response via our Promise.
		this.worker!.postMessage({
			...copyData,
			id: id,
			func: functionName,
		});

		return await promise;
	}

	async findRow2Matches(seedsRow1: number[], row2: string): Promise<number[]> {
		// Using the worker is noticeably slower than calling it directly.
		// According to articles online, it SHOULD be no more than a few milliseconds...
		// unless I'm transferring a lot of data. Then we should use a Transferrable.
		// Yet, making a Uint32Array and transferring the buffer seems no faster.
		if (this.worker)
			return await this.call(rng.findRow2Matches.name, { seeds: seedsRow1, row2: row2 }) as number[];
		else
			return rng.findRow2Matches(seedsRow1, row2);
	}
}
