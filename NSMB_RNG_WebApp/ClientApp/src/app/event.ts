class SimpleEventExposed<T> {
	protected handlers: { (data: T): void }[] = [];

	add(handler: { (data: T): void }) {
		this.handlers.push(handler);
	}
	remove(handler: { (data: T): void }) {
		this.handlers.splice(this.handlers.indexOf(handler), 1);
	}
}

export class SimpleEvent<T> extends SimpleEventExposed<T> {
	trigger(data: T) {
		this.handlers.slice().forEach((h) => h(data));
	}

	expose() {
		return this as SimpleEventExposed<T>;
	}
}
