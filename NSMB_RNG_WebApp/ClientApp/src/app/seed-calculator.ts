const A = 0x0001;
const B = 0x0002;
const Select = 0x0004;
const Start = 0x0008;
const Right = 0x0010;
const Left = 0x0020;
const Up = 0x0040;
const Down = 0x0080;
const R = 0x0100;
const L = 0x0200;
const X = 0x0400;
const Y = 0x0800;
const Debug = 0x2000;
const BUTTON_MASK = 0x2fff;
export const buttons = {
	A: A, B: B, X: X, Y: Y,
	Start: Start, Select: Select,
	L: L, R: R,
	Up: Up, Down: Down, Left: Left, Right: Right,
};


export class SeedCalculator {
	private bytesBuffer: ArrayBuffer = new ArrayBuffer(32);
	private data = new DataView(this.bytesBuffer);

	private _is3DS: boolean = false;
	public get is3DS() {
		return this._is3DS;
	}
	public set is3DS(value: boolean) {
		this._is3DS = value;
		// Hour setter updates the byte value (it depends on is3DS)
		this.hour = this.hour;
	}

	public get timer0() { return this.data.getUint16(0, true); }
	public set timer0(value: number) { this.data.setUint16(0, value, true); }

	public get vCount() { return this.data.getUint16(2, true); }
	public set vCount(value: number) { this.data.setUint16(2, value, true); }

	private _mac: DataView = new DataView(new ArrayBuffer(6));
	public setMAC(value: string) {
		let s = value.length == 17 ? 3 : 2;
		for (let i = 0; i < 6; i++) {
			this._mac.setUint8(i, parseInt(value.substring(i * s, i * s + 2), 16));
		}

		let vframe = this.vFrame;
		this.data.setUint16(6, this._mac.getUint16(4, true), true);
		this.data.setUint32(8, this._mac.getUint32(0, true) ^ 0x06000000 ^ vframe, true);
	}

	public get vFrame() {
		return this.data.getUint32(8, true) ^ 0x06000000 ^ this._mac.getUint32(0, true);
	}
	public set vFrame(value: number) {
		this.data.setUint32(8, this._mac.getUint32(0, true) ^ 0x06000000 ^ value, true)
	}

	public get year() {
		return this.fromBCD(this.data.getUint8(12));
	}
	public set year(value: number) {
		this.data.setUint8(12, this.toBCD(value));
	}

	public get month() {
		return this.fromBCD(this.data.getUint8(13));
	}
	public set month(value: number) {
		this.data.setUint8(13, this.toBCD(value));
	}

	public get dayOfMonth() {
		return this.fromBCD(this.data.getUint8(14));
	}
	public set dayOfMonth(value: number) {
		this.data.setUint8(14, this.toBCD(value));
	}

	public get dayOfWeek() {
		return this.fromBCD(this.data.getUint8(15));
	}
	public set dayOfWeek(value: number) {
		this.data.setUint8(15, this.toBCD(value));
	}

	public get hour() {
		let v = this.data.getUint8(16) & 0xBF;
		return this.fromBCD(v);
	}
	public set hour(value: number) {
		let v = this.toBCD(value);
		if (!this._is3DS && value >= 12)
			v += 0x40;
		this.data.setUint8(16, v);
	}

	public get minute() {
		return this.fromBCD(this.data.getUint8(17));
	}
	public set minute(value: number) {
		this.data.setUint8(17, this.toBCD(value));
	}

	public get second() {
		return this.fromBCD(this.data.getUint8(18));
	}
	public set second(value: number) {
		this.data.setUint8(18, this.toBCD(value));
	}

	public get buttons() {
		return this.data.getUint32(28, true) ^ BUTTON_MASK;
	}
	public set buttons(value: number) {
		this.data.setUint32(28, value ^ BUTTON_MASK, true);
	}

	private toBCD(input: number) {
		let tens = (input / 10) | 0;
		return (tens * 16 + (input % 10));
	}
	private fromBCD(input: number) {
		let tens = input >> 4;
		return tens * 10 + (input & 0xf);
	}

	public constructor(macAddress: string, dateTime: Date, _3DS: boolean = false) {
		this.vFrame = 0; // we have set this before setting the MAC, because of the way the setters handle their overlap
		this.is3DS = _3DS;
		this.buttons = 0;

		this.setMAC(macAddress);
		this.setDateTime(dateTime);
	}

	public async getSeed() {
		let sha1 = await crypto.subtle.digest("SHA-1", this.data);
		let sha1Ints = new Uint32Array(sha1);
		return (sha1Ints[0] ^ sha1Ints[1] ^ sha1Ints[2] ^ sha1Ints[3] ^ sha1Ints[4]) >>> 0; // >>> converts to uint32, all other bitwise operators are signed
	}

	public setDateTime(dt: Date) {
		this.year = dt.getFullYear() - 2000;
		this.month = dt.getMonth() + 1;
		this.dayOfMonth = dt.getDate();
		this.dayOfWeek = dt.getDay();
		this.hour = dt.getHours();
		this.minute = dt.getMinutes();
		this.second = dt.getSeconds();
	}


	public toString() {
		let sb: string[] = [];
		for (let i = 0; i < 32; i += 4) {
			sb.push(this.data.getUint32(i, false).toString(16).padStart(8, '0'));
		}
		return sb.join('');
	}
}
