import urllib.request
import json
import category, steam
from datetime import time


def search(q):
	
	response = urllib.request.urlopen('https://api.toofz.com/players/?q={}'.format(q)).read()
	cont = json.loads(response.decode('utf-8'))
	string = ''
	i = 0
	for item in cont['players']:
		if i >= 5:
			break
		string += '{}\n\t#{}\n'.format(item['display_name'], item['id'])
		i += 1
	return string
	

def get_top(name):

	response = urllib.request.urlopen('https://api.toofz.com/players/?q={}'.format(name).replace(' ', '%20')).read()
	cont = json.loads(response.decode('utf-8'))
	try:
		best = cont['players'][0]
		user = steam.user(best['id'], best['display_name'], best['avatar'].replace('.jpg', '_medium.jpg'), best['updated_at'])
		return user
	except:
		return None


def fill_user(steam_id):

	response = urllib.request.urlopen('https://api.toofz.com/players/{}/entries'.format(steam_id)).read()
	cont = json.loads(response.decode('utf-8'))
	try:
		player = cont['player']
		user = steam.user(player['id'], player['display_name'], player['avatar'].replace('.jpg', '_medium.jpg'), player['updated_at'])
		return user
	except:
		return None


def entries(user, matches):

	response = urllib.request.urlopen('https://api.toofz.com/players/{}/entries'.format(user.steam_id)).read()
	cont = json.loads(response.decode('utf-8'))
	
	string = ''
	for item in cont['entries']:
		for m in matches:
			if item['leaderboard']['id'] == m.lbid:
				char = category.pad(m.char, 9)
				score = category.score_string(item['score'], m)
				rank = category.rank_string(item['rank'])
				string += "{}  {}   {}\n".format(char, score, rank)
	
	return (string)


def list_users(userList, runtype):
	for name in userList:
		steamid = get_top(name)
		if steamid != 0:
			response = urllib.request.urlopen('https://api.toofz.com/players/{}/entries'.format(steamid)).read()
			cont = json.loads(response.decode('utf-8'))
			for item in cont['entries']:
				if item['leaderboard']['id'] == 2047626:
					print(score_string(item['score'], 'Speed'))
					break
