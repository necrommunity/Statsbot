import urllib.request, math, json
import xml.etree.ElementTree as ET
import category


with open('key.conf') as f:
	steamkey = f.read()

statsnames = {
	"NumGreenBatKills": "GreenBats",
	"NumDeaths": "Deaths",
	"NumHardcoreCompletionsCadence": "Cadence",
	"NumHardcoreCompletionsAria": "Aria",
	"NumHardcoreCompletionsBard": "Bard",
	"NumHardcoreCompletionsBolt": "Bolt",
	"NumHardcoreCompletionsMonk": "Monk",
	"NumDailyChallengeCompletions": "Dailies",
	"NumSub8CadenceSpeedruns": "Cadence speed",
	"NumAriaLowPercentCompletions": "Aria low",
	"NumHardcoreCompletionsDove": "Dove",
	"NumHardcoreCompletionsEli": "Eli",
	"NumHardcoreCompletionsMelody": "Melody",	
	"NumHardcoreCompletionsDorian": "Dorian",
	"NumHardcoreCompletionsCoda": "Coda",
	"NumAllCharsCompletions": "All Chars",
	"NumAllCharsLowPercentCompletions": "All low",
	"NumHardcoreCompletionsNocturna": "Nocturna",
	"NumHardcoreCompletionsDiagonal": "Diamond",
	"NumHardcoreCompletionsReaper": "Mary",
	"NumHardcoreCompletionsTempo": "Tempo",
	"NumNoReturnCompletionsCadence": "NR",
	"NumHardModeCompletionsNocturna": "HM",
	"NumSub10NocturnaSpeedruns": "Nocturna speed",
	"NumStoryModeCompletions": "Story",
	"NumPhasingModeCompletions": "Phasing",
	"NumRandomizerModeCompletions": "Rando",
	"NumMysteryModeCompletions": "Mystery",
	"NumAllCharsDLCCompletions": "All Chars DLC",
}


def parse_index():
	response = urllib.request.urlopen('http://steamcommunity.com/stats/247080/leaderboards/?xml=1').read()
	root = ET.fromstring(response)
	d = {}
	for item in root.findall('leaderboard'):
		lbid = int(item.find('lbid').text)
		lb = category.leaderboard(item.find('display_name').text, 'Classic', lbid)
		d[lbid] = lb
	return(d)


def dig(i):
	if i == 0:
		return 1;
	return int(math.floor(math.log10(i) + 1))


def fetch_lb(lb, offset=1):
	response = urllib.request.urlopen('http://steamcommunity.com/stats/247080/leaderboards/{}/?xml=1&start={}&end={}'.format(lb.lbid, offset, offset+9)).read()
	root = ET.fromstring(response)
	root = root.find('entries')
	pad = dig(offset+9)

	ids = []
	for e in root.findall('entry'):
		ids.append(e.find('steamid').text)
	profiles = get_players(ids)


	string = ''
	for e in root.findall('entry'):
		rank = int(e.find('rank').text)
		rank = '0' * (pad-dig(rank)) + str(rank) +'.'
		score = category.score_string(int(e.find('score').text), lb)
		player = profiles[e.find('steamid').text]
		string += '{}  {}  {}\n'.format(rank, score, player)
	return string


def get_players(ids):
	joint = ','.join(ids)
	url = 'http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={}&steamids={}'.format(steamkey, joint)
	response = urllib.request.urlopen(url).read()
	cont = json.loads(response.decode('utf-8'))
	
	d = {}
	for item in cont['response']['players']:
		d[item['steamid']] = item['personaname']
	return d


def get_stats(user):
	try:
		response = urllib.request.urlopen('http://api.steampowered.com/ISteamUserStats/GetUserStatsForGame/v0002/?key={}&appid=247080&steamid={}'.format(steamkey, user.steam_id)).read()
	except:
		return 'Failed to retrieve game stats for {}.\n(profile is likely private, please refer to the pins).'.format(user.name)
	cont = json.loads(response.decode('utf-8'))
	stats = cont['playerstats']['stats']

	try:
		response = urllib.request.urlopen('http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={}&steamid={}'.format(steamkey, user.steam_id)).read()
		cont = json.loads(response.decode('utf-8'))
		for game in cont['response']['games']:
			if game['appid'] == 247080:
				time_ever = int(game['playtime_forever'])
				try:
					time_2weeks = int(game['playtime_2weeks'])
				except:
					time_2weeks = 0
	except:
		time_ever = 60
		time_2weeks = 60
	
	
	d = {}
	for s in stats:
		if statsnames.get(s['name']):
			d[statsnames[s['name']]] = int(s['value'])
	
	for s in statsnames.values():
		if not d.get(s):
			d[s] = 0

	string = ''
	string += 'Playtime: {} hours ({} recently)\n\n'.format(int(time_ever/60), round(time_2weeks/60, 3))
	string += 'Deaths: {} ({} per hour)\n'.format(d['Deaths'], round(int(d['Deaths']) / (time_ever/60), 5))
	string += 'Green bats: {} ({} per hour)\n\n'.format(d['GreenBats'], round(int(d['GreenBats']) / (time_ever/60), 5))
	string += 'Clears count\n'

	for char in category.characters:
		if d[char] != 0:
			extra = ''
			if char == 'All Chars':
				extra = ' ({} low%)'.format(d['All low'])
			if char == 'Aria':
				extra = ' ({} low%)'.format(d['Aria low'])
			if char == 'Cadence':
				extra = ' ({} sub-15, {} no-r)'.format(d['Cadence speed'], d['NR'])
			if char == 'Nocturna':
				extra += ' ({} sub-15, {} hard)'.format(d['Nocturna speed'], d['HM'])
			string += '   {}{}{}{}\n'.format(category.pad(char, 9), ' '*(5-dig(d[char])) ,d[char], extra)
	for e in ['Dailies', 'Phasing', 'Rando', 'Mystery']:
		if d[e] != 0:		
			string += '   {}{}{}\n'.format(category.pad(e, 9), ' '*(5-dig(d[e])) ,d[e])

	return string
	

class user:
	def __init__(self, steam_id, name=None, avatar=None, updated=None):
		self.steam_id = steam_id
		self.name = name
		self.avatar = avatar
		self.updated = updated


