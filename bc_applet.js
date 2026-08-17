
const lib = require('./ids.js');
const fs = require('node:fs');

// Set up XMLHttpRequest.
XMLHttpRequest = require("xmlhttprequest").XMLHttpRequest;
window = {
	XMLHttpRequest: XMLHttpRequest
};
XMLHttpRequest.UNSENT = 0;
XMLHttpRequest.OPENED = 1;
XMLHttpRequest.HEADERS_RECEIVED = 2;
XMLHttpRequest.LOADING = 3;
XMLHttpRequest.DONE = 4;
// Set up WebSocket.
WebSocket = require('ws');
// Set up LocalStorage.
LocalStorage = require('node-localstorage/LocalStorage').LocalStorage;
os = require('os');
var configDir = os.homedir() + "/.bciot";
localStorage = new LocalStorage(configDir);


var bc = require("braincloud")

_bc = new bc.BrainCloudWrapper(); // optionally pass in a _wrapperName
_bc.initialize(lib.appId, lib.appSecret, "1.0.0");

function callBC(context, method, ...args) {
	return new Promise((resolve, reject) => {
		method.call(context, ...args, (response) => {
			if (response.status === 200) {
				resolve(response.data);
			} else {
				reject(response);
			}
		});
	});
}

async function searchUser(username) {
	var maxResults = 5;
	const res = await callBC(_bc, _bc.friend.findUsersByNameStartingWith, username, maxResults);
	return res.matches;
}

async function getSteamID(profId) {
	// const profile = result.matches[0];
	const res = await callBC(_bc, _bc.friend.getExternalIdForProfileId, profId, "Steam");
	return res;
}

async function fetchLeaderboard(leaderboardId, start=0) {
	const res = await callBC(_bc, _bc.leaderboard.getGlobalLeaderboardPage, leaderboardId, "HIGH_TO_LOW", start, start+9);
	// console.log(JSON.stringify(res, null, 2));
	return res;
}

async function listLeaderboards() {
	const res = await callBC(_bc, _bc.leaderboard.listAllLeaderboards);
	leaderboards = res.leaderboardList.filter((e) => e.expiry == null);
	leaderboardsNoWeekly = leaderboards.filter((e) => !e.leaderboardId.includes("WEEKLY"))
	
	lbIds = [];
	Object.entries(leaderboardsNoWeekly).forEach(([key, value]) => {
		lb_ids.push(value.leaderboardId);
	});
	const fileContent = lbIds.join('\n');
	try {
		fs.writeFileSync('leaderboards.txt', fileContent)
	} catch (err) {
		console.error(err);
	}
}


async function bcConnect(){
	console.log("Authenticating with brainCloud...");
	const authData = await callBC(_bc, _bc.authenticateAnonymous);
	console.log("Authenticated as Profile ID:", authData.profileId);
}



const express = require('express');

const app = express();
const SOCKET_PATH = '/tmp/express_ipc.sock';

// Middleware to parse incoming JSON bodies (sets limit for multi-KB payloads)
app.use(express.json({ limit: '10mb' }));

// 1. Remove existing socket file if left over from a previous run
if (fs.existsSync(SOCKET_PATH)) {
	fs.unlinkSync(SOCKET_PATH);
}

// 2. Define query endpoint
app.post('/query', (req, res) => {
	const { queryId, payload } = req.body;
	
	// Process query logic
	res.json({
		queryId,
		status: 'success',
		result: `Processed query for ${payload}`
	});
	// res.send('Hello World!');
});

app.post('/search', async (req, res) => {
	const { username } = req.body;
	response = await searchUser(username);
	res.json(response);
});

app.post('/steamid', async (req, res) => {
	const { uuid } = req.body;
	response = await getSteamID(uuid);
	res.json(response);
});

app.post('/getlb', async (req, res) => {
	const { lbid, index } = req.body;
	response = await fetchLeaderboard(lbid, index);
	res.json(response);
});

// 3. Listen on the Unix socket path instead of a port number
const server = app.listen(SOCKET_PATH, () => {
	console.log(`Express server listening on Unix socket: ${SOCKET_PATH}`);
	bcConnect();
});

// Clean up socket file on server shutdown
process.on('SIGINT', () => {
	server.close(() => {
		if (fs.existsSync(SOCKET_PATH)) fs.unlinkSync(SOCKET_PATH);
		process.exit(0);
	});
});

