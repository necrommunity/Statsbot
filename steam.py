import urllib.request, math
import xml.etree.ElementTree as ET
import category


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
	string = ''
	for e in root.findall('entry'):
		rank = int(e.find('rank').text)
		rank = '0' * (pad-dig(rank)) + str(rank)
		string += '`{}  {}   {}`\n'.format(rank, category.score_string(e.find('score').text, lb), e.find('steamid').text)
	return string


class user:
	def __init__(self, steam_id, name, avatar, updated):
		self.steam_id = steam_id
		self.name = name
		self.avatar = avatar
		self.updated = updated
