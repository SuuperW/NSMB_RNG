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
	private dataAsUint32: number[] = [];

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
	public set timer0(value: number) {
		this.data.setUint16(0, value, true);
		this.dataAsUint32[0] = this.data.getUint32(0, false);
	}

	public get vCount() { return this.data.getUint16(2, true); }
	public set vCount(value: number) {
		this.data.setUint16(2, value, true);
		this.dataAsUint32[0] = this.data.getUint32(0, false);
	}

	private _mac: DataView = new DataView(new ArrayBuffer(6));
	public setMAC(value: string) {
		let s = value.length == 17 ? 3 : 2;
		for (let i = 0; i < 6; i++) {
			this._mac.setUint8(i, parseInt(value.substring(i * s, i * s + 2), 16));
		}

		let vframe = this.vFrame;
		this.data.setUint16(6, this._mac.getUint16(4, true), true);
		this.data.setUint32(8, this._mac.getUint32(0, true) ^ 0x06000000 ^ vframe, true);

		this.dataAsUint32[1] = this.data.getUint32(4, false);
		this.dataAsUint32[2] = this.data.getUint32(8, false);
	}

	public get vFrame() {
		return this.data.getUint32(8, true) ^ 0x06000000 ^ this._mac.getUint32(0, true);
	}
	public set vFrame(value: number) {
		this.data.setUint32(8, this._mac.getUint32(0, true) ^ 0x06000000 ^ value, true)
		this.dataAsUint32[2] = this.data.getUint32(8, false);
	}

	public get year() {
		return this.fromBCD(this.data.getUint8(12)) + 2000;
	}
	public set year(value: number) {
		this.data.setUint8(12, this.toBCD(value));
		this.dataAsUint32[3] = this.data.getUint32(12, false);
	}

	public get month() {
		return this.fromBCD(this.data.getUint8(13));
	}
	public set month(value: number) {
		this.data.setUint8(13, this.toBCD(value));
		this.dataAsUint32[3] = this.data.getUint32(12, false);
	}

	public get dayOfMonth() {
		return this.fromBCD(this.data.getUint8(14));
	}
	public set dayOfMonth(value: number) {
		this.data.setUint8(14, this.toBCD(value));
		this.dataAsUint32[3] = this.data.getUint32(12, false);
	}

	public get dayOfWeek() {
		return this.fromBCD(this.data.getUint8(15));
	}
	public set dayOfWeek(value: number) {
		this.data.setUint8(15, this.toBCD(value));
		this.dataAsUint32[3] = this.data.getUint32(12, false);
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
		this.dataAsUint32[4] = this.data.getUint32(16, false);
	}

	public get minute() {
		return this.fromBCD(this.data.getUint8(17));
	}
	public set minute(value: number) {
		this.data.setUint8(17, this.toBCD(value));
		this.dataAsUint32[4] = this.data.getUint32(16, false);
	}

	public get second() {
		return this.fromBCD(this.data.getUint8(18));
	}
	public set second(value: number) {
		this.data.setUint8(18, this.toBCD(value));
		this.dataAsUint32[4] = this.data.getUint32(16, false);
	}

	public get buttons() {
		return this.data.getUint32(28, true) ^ BUTTON_MASK;
	}
	public set buttons(value: number) {
		this.data.setUint16(28, value ^ BUTTON_MASK, true);
		this.dataAsUint32[7] = this.data.getUint32(28, false);
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
		for (let i = 0; i < 80; i++) {
			this.dataAsUint32.push(0);
		}
		this.dataAsUint32[8] = 0x80000000;
		this.dataAsUint32[15] = 0x00000100;

		this.vFrame = 0; // we have set this before setting the MAC, because of the way the setters handle their overlap
		this.is3DS = _3DS;
		this.buttons = 0;

		this.setMAC(macAddress);
		this.setDateTime(dateTime);
	}

	public getSeed() {
		// SHA-1
		// We aren't using crypto.sublte because it's horribly slow due to the overhead of async.
		let buffer = this.dataAsUint32;
		for (let i = 16; i < 80; i++) {
			let t = buffer[i - 3] ^ buffer[i - 8] ^ buffer[i - 14] ^ buffer[i - 16];
			buffer[i] = (t << 1) | (t >>> 31);
		}

		let a = 0x67452301;
		let b = 0xEFCDAB89;
		let c = 0x98BADCFE;
		let d = 0x10325476;
		let e = 0xC3D2E1F0;
		let f = 0;

		for (let i = 0; i < 18; /**/) {
			f = ((a << 5) | (a >>> 27)) + (d ^ (b & (c ^ d))) + e + 0x5A827999 + buffer[i++];
			b = (b << 30) | (b >>> 2);
			e = ((f << 5) | (f >>> 27)) + (c ^ (a & (b ^ c))) + d + 0x5A827999 + buffer[i++];
			a = (a << 30) | (a >>> 2);
			d = ((e << 5) | (e >>> 27)) + (b ^ (f & (a ^ b))) + c + 0x5A827999 + buffer[i++];
			f = (f << 30) | (f >>> 2);
			c = ((d << 5) | (d >>> 27)) + (a ^ (e & (f ^ a))) + b + 0x5A827999 + buffer[i++];
			e = (e << 30) | (e >>> 2);
			b = ((c << 5) | (c >>> 27)) + (f ^ (d & (e ^ f))) + a + 0x5A827999 + buffer[i++];
			d = (d << 30) | (d >>> 2);
			a = ((b << 5) | (b >>> 27)) + (e ^ (c & (d ^ e))) + f + 0x5A827999 + buffer[i++];
			c = (c << 30) | (c >>> 2);
		}
		f = ((a << 5) | (a >>> 27)) + (d ^ (b & (c ^ d))) + e + 0x5A827999 + buffer[18];
		b = (b << 30) | (b >>> 2);
		e = ((f << 5) | (f >>> 27)) + (c ^ (a & (b ^ c))) + d + 0x5A827999 + buffer[19];
		a = (a << 30) | (a >>> 2);

		for (let i = 20; i < 38; /**/) {
			d = ((e << 5) | (e >>> 27)) + (f ^ a ^ b) + c + 0x6ED9EBA1 + buffer[i++];
			f = (f << 30) | (f >>> 2);
			c = ((d << 5) | (d >>> 27)) + (e ^ f ^ a) + b + 0x6ED9EBA1 + buffer[i++];
			e = (e << 30) | (e >>> 2);
			b = ((c << 5) | (c >>> 27)) + (d ^ e ^ f) + a + 0x6ED9EBA1 + buffer[i++];
			d = (d << 30) | (d >>> 2);
			a = ((b << 5) | (b >>> 27)) + (c ^ d ^ e) + f + 0x6ED9EBA1 + buffer[i++];
			c = (c << 30) | (c >>> 2);
			f = ((a << 5) | (a >>> 27)) + (b ^ c ^ d) + e + 0x6ED9EBA1 + buffer[i++];
			b = (b << 30) | (b >>> 2);
			e = ((f << 5) | (f >>> 27)) + (a ^ b ^ c) + d + 0x6ED9EBA1 + buffer[i++];
			a = (a << 30) | (a >>> 2);
		}
		d = ((e << 5) | (e >>> 27)) + (f ^ a ^ b) + c + 0x6ED9EBA1 + buffer[38];
		f = (f << 30) | (f >>> 2);
		c = ((d << 5) | (d >>> 27)) + (e ^ f ^ a) + b + 0x6ED9EBA1 + buffer[39];
		e = (e << 30) | (e >>> 2);

		for (let i = 40; i < 58; /**/) {
			b = ((c << 5) | (c >>> 27)) + ((d & e) | ((d ^ e) & f)) + a + 0x8F1BBCDC + buffer[i++];
			d = (d << 30) | (d >>> 2);
			a = ((b << 5) | (b >>> 27)) + ((c & d) | ((c ^ d) & e)) + f + 0x8F1BBCDC + buffer[i++];
			c = (c << 30) | (c >>> 2);
			f = ((a << 5) | (a >>> 27)) + ((b & c) | ((b ^ c) & d)) + e + 0x8F1BBCDC + buffer[i++];
			b = (b << 30) | (b >>> 2);
			e = ((f << 5) | (f >>> 27)) + ((a & b) | ((a ^ b) & c)) + d + 0x8F1BBCDC + buffer[i++];
			a = (a << 30) | (a >>> 2);
			d = ((e << 5) | (e >>> 27)) + ((f & a) | ((f ^ a) & b)) + c + 0x8F1BBCDC + buffer[i++];
			f = (f << 30) | (f >>> 2);
			c = ((d << 5) | (d >>> 27)) + ((e & f) | ((e ^ f) & a)) + b + 0x8F1BBCDC + buffer[i++];
			e = (e << 30) | (e >>> 2);
		}
		b = ((c << 5) | (c >>> 27)) + ((d & e) | ((d ^ e) & f)) + a + 0x8F1BBCDC + buffer[58];
		d = (d << 30) | (d >>> 2);
		a = ((b << 5) | (b >>> 27)) + ((c & d) | ((c ^ d) & e)) + f + 0x8F1BBCDC + buffer[59];
		c = (c << 30) | (c >>> 2);

		for (let i = 60; i < 78; /**/) {
			f = ((a << 5) | (a >>> 27)) + (b ^ c ^ d) + e + 0xCA62C1D6 + buffer[i++];
			b = (b << 30) | (b >>> 2);
			e = ((f << 5) | (f >>> 27)) + (a ^ b ^ c) + d + 0xCA62C1D6 + buffer[i++];
			a = (a << 30) | (a >>> 2);
			d = ((e << 5) | (e >>> 27)) + (f ^ a ^ b) + c + 0xCA62C1D6 + buffer[i++];
			f = (f << 30) | (f >>> 2);
			c = ((d << 5) | (d >>> 27)) + (e ^ f ^ a) + b + 0xCA62C1D6 + buffer[i++];
			e = (e << 30) | (e >>> 2);
			b = ((c << 5) | (c >>> 27)) + (d ^ e ^ f) + a + 0xCA62C1D6 + buffer[i++];
			d = (d << 30) | (d >>> 2);
			a = ((b << 5) | (b >>> 27)) + (c ^ d ^ e) + f + 0xCA62C1D6 + buffer[i++];
			c = (c << 30) | (c >>> 2);
		}
		f = ((a << 5) | (a >>> 27)) + (b ^ c ^ d) + e + 0xCA62C1D6 + buffer[78];
		b = (b << 30) | (b >>> 2);
		e = ((f << 5) | (f >>> 27)) + (a ^ b ^ c) + d + 0xCA62C1D6 + buffer[79];
		a = (a << 30) | (a >>> 2);

		e += 0x67452301;
		f += 0xEFCDAB89;
		a += 0x98BADCFE;
		b += 0x10325476;
		c += 0xC3D2E1F0;

		// Seed is calculated by XOR-ing each 32-bit section of the hash
		let beSeed = e ^ f ^ a ^ b ^ c;
		return ((beSeed << 24) | ((beSeed & 0xff00) << 8) | ((beSeed & 0xff0000) >> 8) | (beSeed >>> 24)) >>> 0;
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
