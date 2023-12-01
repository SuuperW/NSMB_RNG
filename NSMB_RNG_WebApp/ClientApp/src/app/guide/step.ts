import { FormGroup } from "@angular/forms";
import { SimpleEvent } from "../event";

export abstract class StepComponent {
	abstract form: FormGroup;
	public errorStatus?: string;

	private inProgress: string[] = [];
	private _progress = new SimpleEvent<string[]>();
	public get progress() {
		return this._progress.expose();
	}

	protected addProgress(display: string) {
		this.inProgress.push(display);
		this._progress.trigger(this.inProgress);
	}
	protected removeProgress(display: string) {
		this.inProgress.splice(this.inProgress.indexOf(display), 1);
		this._progress.trigger(this.inProgress);
	}
}
