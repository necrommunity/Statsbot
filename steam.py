import urllib.request, json
import category


with open('config.json') as f:
	content = json.loads(f.read())
	steamkey = content['steamkey']

with open('steam_stats.json') as f:
	statsnames = json.loads(f.read())

class User:
	def __init__(self, steam_id, name=None, avatar=None):
		self.steam_id = steam_id
		self.name = name
		self.avatar = avatar


def fill_user(steam_id):
	url = 'http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={}&steamids={}'
	response = urllib.request.urlopen(url.format(steamkey, steam_id)).read()
	cont = json.loads(response.decode('utf-8'))
	try:
		player = cont['response']['players'][0]
		steam_user = User(player['steamid'], player['personaname'], player['avatarmedium'])
		return steam_user
	except:
		return


def fetch_player_stats(steamuser:User):
	try:
		response = urllib.request.urlopen('http://api.steampowered.com/ISteamUserStats/GetUserStatsForGame/v0002/?key={}&appid=247080&steamid={}'.format(steamkey, steamuser.steam_id)).read()
	except:
		return
	
	content = json.loads(response.decode('utf-8'))
	raw_stats = content['playerstats']['stats']

	player_stats = {statsnames[s['name']]: int(s['value']) for s in raw_stats}
	for name in statsnames.values():
		if name not in player_stats:
			player_stats[name] = 0

	# playtime
	try:
		response = urllib.request.urlopen('http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={}&steamid={}'.format(steamkey, steamuser.steam_id)).read()
		content = json.loads(response.decode('utf-8'))
		for game in content['response']['games']:
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
		return

	response = {}
	if (p_stats['time_ever']):
		response['Playtime'] = '{} hours ({} recently)'.format(int(p_stats['time_ever']/60), round(p_stats['time_2weeks']/60, 3))
		response['Deaths'] = '{} ({} per hour)'.format(p_stats['Deaths'], round(p_stats['Deaths'] / (p_stats['time_ever']/60), 5))
	else:
		response['Deaths'] = str(p_stats['Deaths'])

	response['Green bats'] = str(p_stats['Green Bats'])
	
	table = ''
	for char in category.chars_human:
		if p_stats.get(char, 0) != 0:
			table += '{}{}\n'.format(char.ljust(13),str(p_stats[char]).rjust(5))
	response['Approximate clears count'] = '```' + table + '```'
	return response
