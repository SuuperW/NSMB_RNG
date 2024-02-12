// Bad testing framework doesn't do this automatically.
// Tests should be repeatable and independent of eachother.
beforeEach(() => {
	localStorage.clear();
});
