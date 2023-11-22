const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
	env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:9461';

const LOCAL_ASP = {
	target: target,
	secure: false,
	proxyTimeout: 10000,
};
const PROXY_CONFIG = {
	"/weatherforecast": LOCAL_ASP,
	"/asp/**": LOCAL_ASP,
};
/*
	{
		context: [
			"/weatherforecast",
			"/asp/**",
		 ],
		proxyTimeout: 10000,
		target: target,
		secure: false,
		headers: {
			Connection: 'Keep-Alive'
		}
	}
*/

module.exports = PROXY_CONFIG;
