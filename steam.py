import urllib.request, math, json
import xml.etree.ElementTree as ET
import category


with open('key.conf') as f:
	steamkey = f.read()

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
		string += '`{}\t{}\t{}`\n'.format(rank, score, player)
	return string


def get_players(ids):
	request = 'http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={}&steamids='.format(steamkey)
	for i in ids:
		request += i + ','
	response = urllib.request.urlopen(request).read()
	cont = json.loads(response.decode('utf-8'))
	
	d = {}
	for item in cont['response']['players']:
		d[item['steamid']] = item['personaname']
	return d
	

class user:
	def __init__(self, steam_id, name=None, avatar=None, updated=None):
		self.steam_id = steam_id
		self.name = name
		self.avatar = avatar
		self.updated = updated
