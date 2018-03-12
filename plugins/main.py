import toofz, steam, category

from urllib.parse import quote
from disco.bot import Plugin
from disco.types.message import MessageEmbed


ecolor = 0x00AD96


def run_name(run, seeded):
	if seeded:
		return 'Seeded {}'.format(run)
	return run


def check_alt(args):
	if category.caseless_in('-alt', args):
		return True
	return False


class MainPlugin(Plugin):

	def load(self, event):
		self.index = category.indexer()


	@Plugin.command('reload', oob=True)
	def on_reload(self, event):
		user = event.author.username
		if user != 'Naymin':
			return
		self.reload()
		event.msg.reply('Reloaded!')


	@Plugin.command('version')
	def on_version(self, event):
		embed = MessageEmbed()
		embed.color = ecolor
		embed.title = 'Statsbot v0.93. Use `.help` for more info.'
		event.channel.send_message('', embed=embed)


	@Plugin.command('help', '[args:str]')
	def on_help(self, event, args=''):

		args = args.lower()
		embed = MessageEmbed()
		embed.color = ecolor

		if args == '':
			embed.title = 'Statsbot is a bot which retrieves Crypt of the Necrodancer player stats'
			embed.description = '''\nAvailable commands:
								\n`.search`, `.speed`, `.score`, `.deathless`, `.leaderboard`, `.stats`.
								\nUse `.help command` for more info about a particular command.
								\n\n**Frequently asked questions:**
								\n**Q: I can't find myself!**
								\nA: Search using your steam name. If you recently changed it, there might be a while before the bot is updated.
								\n**Q: Why does someone else with my nickname shows up?**
								\nA: The player brought up is the one with the most records across all the leaderboards. Until it's you, use the command with your steam ID instead of nickname (`.speed #12345678`). If you don't know what your ID is, use `.search <your nick here>` to find out.
								\n**Q: Is it possible to see my stats for iOS or switch versions?**
								\nA: Unfortunately there's no way I'm aware of to access that info as of now, all the information presented by the bot comes from steam and Toofz (Mendayen's site).
								\n**Q: Who are you?**
								\nA: I'm Statsbot. beep boop. But nah I'm Naymin, feel free to PM me here or on twitter (naymin_nd).'''
			event.channel.send_message('', embed=embed)
			return
		if 'search' in args:
			embed.title = 'Search for players and their steam IDs'
			embed.description = 'Use `.search` `name` to see a list of results.'
			event.channel.send_message('', embed=embed)
			return
		if 'speed' in args or 'score' in args or 'deathless' in args:
			embed.title = "Display player's personal bests in a specific category"
			embed.description = '''Use `.speed`, `.score` or`.deathless` `name`.
								\nAdd `seeded`, `classic`, `hardmode`, `mystery` etc to filter the results.
								\nSteamID can be used instead of a name by prefixing it with a `#`.'''
			event.channel.send_message('', embed=embed)
			return
		if args == 'leaderboard':
			embed.title = "Display an in-game leaderboard"
			embed.description = '''Use `.leaderboard` `character` `category`.
								\nAdd `seeded`, `classic`, `hardmode`, `mystery` etc to filter the results.
								\nAdjust entries offset by adding `-number`.'''
			event.channel.send_message('', embed=embed)
			return
		if args == 'stats':
			embed.title = "Display misc statistics recorded by steam"
			embed.description = 'Use `.stats` `name`.'
			event.channel.send_message('', embed=embed)
			return
		


	@Plugin.command('search', '[args:str...]')
	def on_search(self, event, args=''):

		embed = MessageEmbed()
		embed.color = ecolor

		alt = check_alt(args)
		if alt:
			args = args.replace('-alt', '').strip()

		if args == '':
			embed.title = 'Please enter a name to search for.'
			event.channel.send_message('', embed=embed)
			return

		results = toofz.search(quote(args))
		if results == '':
			embed.title = 'No players found called "{}".'.format(args)
			event.channel.send_message('', embed=embed)
			return

		if alt:
			event.channel.send_message('```Displaying top player results for "{}":\n\n{}```'.format(args, results))
			return

		embed.title = 'Displaying top player results for "{}":'.format(args)
		embed.set_thumbnail(url='https://raw.githubusercontent.com/necrommunity/Statsbot/master/icons/search.png')
		embed.add_field(name='-', value='`{}`'.format(results))

		event.channel.send_message('', embed=embed)


	@Plugin.command('score', '[args:str...]')
	@Plugin.command('speed', '[args:str...]')
	@Plugin.command('deathless', '[args:str...]')
	def on_entries(self, event, args=''):

		embed = MessageEmbed()
		embed.color = ecolor

		alt = check_alt(args)
		if alt:
			args = args.replace('-alt', '').strip()

		try:
			user = event.member.name
		except:
			user = event.author.username

		if args != '':
			user = args.split()[0]
			args = args[len(user)+1:]

		if user[0] == '#':
			user = user[1:]
			steam_user = toofz.fill_user(user)
		else:
			steam_user = toofz.get_top(quote(user))

		if not steam_user:
			embed.title = 'No players found called "{}".'.format(user)
			event.channel.send_message('', embed=embed)
			return

		runtype = event.name.capitalize()
		matches = self.index.get_matching_boards(args, runtype)
		if len(matches) == 0:
			embed.title = 'No {} leaderboards found. Is this a valid category?'.format(runtype)
			event.channel.send_message('', embed=embed)
			return

		results = toofz.entries(steam_user, matches)
		if results == '':
			embed.title = 'No results found for {} in the {} category ({}, {}).'.format(steam_user.name, run_name(runtype, matches[0].seeded), matches[0].ver, matches[0].extra)
			event.channel.send_message('', embed=embed)
			return
		
		if alt:
			event.channel.send_message('```Displaying {} results ({}, {})\n{}\n\n{}```'.format(run_name(runtype, matches[0].seeded), matches[0].ver, matches[0].extra, steam_user.name, steam_user.steam_id, results))
			return

		embed.title = "{} #{}".format(steam_user.name, steam_user.steam_id)
		embed.timestamp = steam_user.updated
		embed.set_footer(text='Last updated', icon_url='https://raw.githubusercontent.com/necrommunity/Statsbot/master/icons/toofz.png')
		embed.set_thumbnail(url=steam_user.avatar)
		embed.set_author(name='Displaying {} results ({}, {})'.format(run_name(runtype, matches[0].seeded), matches[0].ver, matches[0].extra))
		embed.add_field(name='-', value='`{}`'.format(results))

		event.channel.send_message('', embed=embed)


	@Plugin.command('leaderboard', '[args:str...]')
	@Plugin.command('lb', '[args:str...]')
	def on_leaderboard(self, event, args =''):

		embed = MessageEmbed()
		embed.color = ecolor

		alt = check_alt(args)
		if alt:
			args = args.replace('-alt', '').strip()

		offset = 1
		for arg in args.split():
			if arg[0] == '-' or arg[0] == '&':
				try:
					offset = int(arg[1:])
					args = args.replace(arg, '')
				except:
					embed.title = 'Please enter a valid offset.'
					event.channel.send_message('', embed=embed)
					return

		target = self.index.get_certain_board(quote(args))
		if not target:
			embed.title = 'No leaderboards found. Is this a valid category?'
			event.channel.send_message('', embed=embed)
			return


		results = steam.fetch_lb(target, offset)
		if results == '':
			embed.title = 'No entries found for {} in the {} category ({}, {}).'.format(target.char, run_name(target.run, target.seeded), target.ver, target.extra)
			event.channel.send_message('', embed=embed)
			return

		if alt:
			event.channel.send_message('```Displaying {} leaderboard for {} ({}, {})\n\n{}```'.format(run_name(target.run, target.seeded), target.char, target.ver, target.extra, results))
			return

		embed.title = 'Displaying {} leaderboard for {} ({}, {})'.format(run_name(target.run, target.seeded), target.char, target.ver, target.extra)
		embed.set_footer(icon_url='https://raw.githubusercontent.com/necrommunity/Statsbot/master/icons/steam.png')
		embed.set_thumbnail(url='https://raw.githubusercontent.com/necrommunity/Statsbot/master/icons/{}.png'.format(target.char).replace(' ','%20'))
		embed.add_field(name='-', value='`{}`'.format(results))

		event.channel.send_message('', embed=embed)


	@Plugin.command('stats', '[args:str...]')
	def on_stats(self, event, args=''):

		embed = MessageEmbed()
		embed.color = ecolor

		alt = check_alt(args)
		if alt:
			args = args.replace('-alt', '').strip()
		try:
			user = event.member.name
		except:
			user = event.author.username

		if args != '':
			user = args.split()[0]
			args = args[len(user)+1:]

		if user[0] == '#':
			user = user[1:]
			steam_user = toofz.fill_user(user)
		else:
			steam_user = toofz.get_top(quote(user))

		if not steam_user:
			embed.title = 'No players found called "{}".'.format(user)
			event.channel.send_message('', embed=embed)
			return

		results = steam.get_stats(steam_user)
		if not results:
			embed.title = 'Failed to retrieve stats for {} (profile is likely private).'.format(steam_user.name)
			event.channel.send_message('', embed=embed)
			return
		
		if alt:
			event.channel.send_message('```{} #{}\n\n{}```'.format(steam_user.name, steam_user.steam_id, results))
			return

		embed.title = "{} #{}".format(steam_user.name, steam_user.steam_id)
		embed.set_footer(icon_url='https://raw.githubusercontent.com/necrommunity/Statsbot/master/icons/steam.png')
		embed.set_thumbnail(url=steam_user.avatar)
		embed.add_field(name='-', value='`{}`'.format(results))

		event.channel.send_message('', embed=embed)

	
	# @Plugin.listen('MessageCreate')
	# def on_message_create(self, event):
	# 	msg = event.message
