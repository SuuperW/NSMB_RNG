import { Injectable } from "@angular/core";
import { WorkerWrapper } from './worker-wrapper';

let workerSupported: boolean = !!window.Worker;
if (!workerSupported) {
	// Should we really be concerned about this? caniuse says 98% of users will have a browser that supports workers.
	alert('Your browser doesn\'t support web workers. The page will freeze during computations, which may make it seem like the web page has crashed.');
}

// The service class is separate from _WorkerService so that workers can spawn workers without referencing a TS module that uses stuff not available to workers.
@Injectable({
	providedIn: 'root',
})
// A service that wraps a web worker, allowing outside code to call worker methods as async methods.
export class WorkerService extends WorkerWrapper { }
