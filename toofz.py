import urllib.request
import json
import category


def search(q):
	result = urllib.request.urlopen('https://api.toofz.com/players/?q={}'.format(q)).read()
	cont = json.loads(result.decode('utf-8'))
	string = ''
	i = 0
	for item in cont['players']:
		if i >= 5:
			break
		string += '{}\n\t{}\n'.format(item['display_name'], item['id'])
		i += 1
	if string == '':
		return 'No results found for "{}".'.format(q)
	return 'Displaying top player results for "{}"\n\n{}'.format(q, string)

def get_top(name):
	result = urllib.request.urlopen('https://api.toofz.com/players/?q={}'.format(name)).read()
	cont = json.loads(result.decode('utf-8'))
	steam_id = 0
	try:
		steam_id = int(cont['players'][0]['id'])
	except:
		steam_id = 0
	return steam_id


def entries(user, runtype, q):
	if q != '':
		user = q.split()[0]
		if user[0] == '#':
			user = user[1:]

	q = q[len(user)+1:]
	search = category.parse(q) #category = ['cadence', 'speed', 'amplified', 'standard', '']
	search[1] = runtype

	steam_id = get_top(user)
	if steam_id == 0:
		return 'No players found for "{}".'.format(user)
	result = urllib.request.urlopen('https://api.toofz.com/players/{}/entries'.format(steam_id)).read()
	cont = json.loads(result.decode('utf-8'))
	
	string = ''
	for item in cont['entries']:
		lb = item['leaderboard']
		if search[1] in lb['run'] and search[2] in lb['product'] and search[3] in lb['mode']:
			if (search[4] == '' and lb['run'].replace(search[1], '') == '') or (search[4] != '' and 'seeded' in lb['run']):
				string += '\t{}{}\t{}\n'.format(category.pad(lb['character'], 10, True), item['score'], item['rank'])
	search = [desc.capitalize() for desc in search]
	if string == '':
		return 'No results found for {} in the {} category ({}, {}).'.format(cont['player']['display_name'], search[1], search[2], search[3])
	return '{} #{}\n\nDisplaying {} results ({}, {})\n\n{}'.format(cont['player']['display_name'], cont['player']['id'], search[1], search[2], search[3], string)
	#cont['player']['avatar']

def list(userList, runtype):
	for name in userList:
		steamid = get_top(name)
		if steamid != 0:
			result = urllib.request.urlopen('https://api.toofz.com/players/{}/entries'.format(steamid)).read()
			cont = json.loads(result.decode('utf-8'))
			for item in cont['entries']:
				if item['leaderboard']['id'] == 2047626:
					print(name, ' ', 100000000 - int(item['score']))
					break
	
def score(a):
	print(a)
	time = int(a)
	time /= 1000
	minutes = int(time/60)
	seconds = time%minutes
	return('{} {}'.format(minutes, seconds))