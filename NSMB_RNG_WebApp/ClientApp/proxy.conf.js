const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
	env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:9461';

const LOCAL_ASP = {
	target: target,
	secure: false,
	proxyTimeout: 20000,
};
const PROXY_CONFIG = {
	"/asp/**": LOCAL_ASP,
	"/favicon.ico": LOCAL_ASP,
};

module.exports = PROXY_CONFIG;
