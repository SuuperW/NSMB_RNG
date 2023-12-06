// What's with unit testing frameworks trying to twist code into something resembling English sentences?
// There's nothing wrong with code as code. Code as English is horrible and introduces needless complexity and layers.
// Most tests come down to checking a boolean expression, so why not just do it directly?
export const assert = (v: boolean, failOutput: string = '') => { expect(v).toBeTruthy(); };
