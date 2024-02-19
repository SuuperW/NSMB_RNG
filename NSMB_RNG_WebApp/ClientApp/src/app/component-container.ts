import { Directive, Input, OnInit, Output, Type, ViewContainerRef } from '@angular/core';
import { SimpleEvent } from './event';

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
			this.makeComponent(value);
		}
	}

	private created: boolean = false;

	@Output() component: T | null | undefined;
	private _componentCreated = new SimpleEvent<T>();
	public get componentCreated() {
		return this._componentCreated.expose();
	}

	constructor(public viewContainerRef: ViewContainerRef) { }

	ngOnInit() {
		if (this._type)
			this.makeComponent(this._type);
		this.created = true;
	}

	private makeComponent(type: Type<T>) {
		this.component = this.viewContainerRef.createComponent(type).instance;
		this._componentCreated.trigger(this.component);
	}
}
