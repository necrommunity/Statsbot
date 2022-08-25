import urllib.request, math, json
import xml.etree.ElementTree as ET
import category


with open('key.conf') as f:
	steamkey = f.read()

with open('steam_stats.json') as f:
	statsnames = json.loads(f.read())


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


def fill_user(steam_id):
	url = 'http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={}&steamids={}'
	response = urllib.request.urlopen(url.format(steamkey, steam_id)).read()
	cont = json.loads(response.decode('utf-8'))
	try:
		player = cont['response']['players'][0]
		steam_user = user(player['steamid'], player['personaname'], player['avatarmedium'])
		return steam_user
	except:
		return None


def get_players(ids):
	joint = ','.join(ids)
	url = 'http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={}&steamids={}'.format(steamkey, joint)
	response = urllib.request.urlopen(url).read()
	cont = json.loads(response.decode('utf-8'))
	
	d = {}
	for item in cont['response']['players']:
		d[item['steamid']] = item['personaname']
	return d

def fetch_player_stats(user):
	try:
		response = urllib.request.urlopen('http://api.steampowered.com/ISteamUserStats/GetUserStatsForGame/v0002/?key={}&appid=247080&steamid={}'.format(steamkey, user.steam_id)).read()
	except:
		return False
	
	cont = json.loads(response.decode('utf-8'))
	raw_stats = cont['playerstats']['stats']

	player_stats = { statsnames[s['name']]: int(s['value']) for s in raw_stats}
	for name in statsnames.values():
		if name not in player_stats:
			player_stats[name] = 0


	# playtime
	try:
		response = urllib.request.urlopen('http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={}&steamid={}'.format(steamkey, user.steam_id)).read()
		cont = json.loads(response.decode('utf-8'))
		for game in cont['response']['games']:
			if game['appid'] == 247080:
				player_stats['time_ever'] = int(game['playtime_forever'])
				try:
					player_stats['time_2weeks'] = int(game['playtime_2weeks'])
				except:
					player_stats['time_2weeks'] = 0
	except:
		player_stats['time_ever'] = 0
		player_stats['time_2weeks'] = 0

	return player_stats

def get_stats(user):
	
	p_stats = fetch_player_stats(user)
	if not p_stats:
		return 'Failed to retrieve stats for {}. Please make sure "Game details" under steam profile privacy settings is set to "public"'.format(user.name)

	string = ''
	if (p_stats['time_ever']):
		string += 'Playtime: {} hours ({} recently)\n\n'.format(int(p_stats['time_ever']/60), round(p_stats['time_2weeks']/60, 3))
		string += 'Deaths: {} ({} per hour)\n'.format(p_stats['Deaths'], round(p_stats['Deaths'] / (p_stats['time_ever']/60), 5))
	else:
		string += 'Deaths: {}\n'.format(p_stats['Deaths'])

	string += 'Green bats: {}\n\n'.format(p_stats['Green Bats'])
	
	string += 'Clears count\n'
	for char in category.characters:
		if p_stats[char] != 0:
			string += '   {}{}{}\n'.format(category.pad(char, 10), ' '*(5-dig(p_stats[char])) ,p_stats[char])

	return string

def get_misc(user):
	
	player_stats = fetch_player_stats(user)
	if not user_stats:
		return 'Failed to retrieve stats for {}. Please make sure "Game details" under steam profile privacy settings is set to "public"'.format(user.name)
		
	string = ''
	if (time_ever):
		string += 'Playtime: {} hours ({} recently)\n\n'.format(int(time_ever/60), round(time_2weeks/60, 3))
		string += 'Deaths: {} ({} per hour)\n'.format(d['Deaths'], round(int(d['Deaths']) / (time_ever/60), 5))
	else:
		string += 'Deaths: {}\n'.format(d['Deaths'])

	string += 'Green bats: {}\n\n'.format(d['GreenBats'])
	
	string += 'Clears count\n'
	for char in category.characters:
		if d[char] != 0:
			string += '   {}{}{}\n'.format(category.pad(char, 10), ' '*(5-dig(d[char])) ,d[char])
	# for e in ['Dailies', 'Phasing', 'Rando', 'Mystery']:
	# 	if d[e] != 0:		
	# 		string += '   {}{}{}\n'.format(category.pad(e, 9), ' '*(5-dig(d[e])) ,d[e])

	return string
	

class user:
	def __init__(self, steam_id, name=None, avatar=None, updated=None):
		self.steam_id = steam_id
		self.name = name
		self.avatar = avatar
		self.updated = updated



