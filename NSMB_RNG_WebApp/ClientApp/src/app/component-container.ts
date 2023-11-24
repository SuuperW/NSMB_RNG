import { Directive, Input, OnInit, Output, Type, ViewContainerRef } from '@angular/core';

@Directive({
	selector: '[container]',
	standalone: true,
})
export class ComponentContainer<T> implements OnInit {
	private _type: Type<T> | undefined;
	@Input() set type(value: Type<T>) {

		this._type = value;
		if (this.created) {
			this.viewContainerRef.clear();
			this.component = this.viewContainerRef.createComponent(value).instance;
		}
	}

	private created: boolean = false;

	@Output() component: T | null | undefined;

	constructor(public viewContainerRef: ViewContainerRef) { }

	ngOnInit() {
		if (this._type)
			this.component = this.viewContainerRef.createComponent(this._type).instance;
		this.created = true;
	}
}
