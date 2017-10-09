import sys
sys.path.append('..')
import toofz, steam, category

from disco.bot import Plugin
from disco.types.message import MessageEmbed


class BasicPlugin(Plugin):

	def load(self, event):
		self.index = category.indexer()


	@Plugin.command('reload', oob=True)
	def on_reload(self, event):
		try:
			user = event.member.name
		except:
			user = event.author.username
		if user != 'Naymin':
			event.msg.reply('please stop')
			return
		self.reload()
		event.msg.reply('Reloaded!')


	@Plugin.command('version')
	def on_info(self, event):
		event.msg.reply('~~Role-bot v0.57~~ maybe statsbot.')


	@Plugin.command('search', '[args:str...]')
	def on_search(self, event, args=''):

		embed = MessageEmbed()
		embed.color = 0x00AD96

		if args == '':
			embed.title = 'Please enter a name to search for.'
			event.channel.send_message('', embed=embed)
			return

		results = toofz.search(args)
		if results == '':
			embed.title = 'No players found called "{}".'.format(args)
			event.channel.send_message('', embed=embed)
			return

		embed.title = 'Displaying top player results for "{}":'.format(args)
		embed.add_field(name='-', value=results)
		event.channel.send_message('', embed=embed)


	@Plugin.command('score', '[args:str...]')
	@Plugin.command('speed', '[args:str...]')
	@Plugin.command('deathless', '[args:str...]')
	def on_entries(self, event, args=''):

		embed = MessageEmbed()
		embed.color = 0x00AD96

		try:
			user = event.member.name
		except:
			user = event.author.username

		if args != '':
			user = args.split()[0]
			args = args[len(user)+1:]
		if user[0] == '#':
			user = user[1:]

		steam_user = toofz.get_top(user)
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
			embed.title = 'No results found for {} in the {} category ({}, {}).'.format(steam_user.name, category.run_name(runtype, matches[0].seeded), matches[0].ver, matches[0].extra)
			event.channel.send_message('', embed=embed)
			return
		
		embed.title = "{} #{}".format(steam_user.name, steam_user.steam_id)
		embed.timestamp = steam_user.updated
		embed.set_footer(text='Fetched from toofz | Last updated: ', icon_url='http://crypt.toofz.com/favicon-96x96.png')
		embed.set_thumbnail(url=steam_user.avatar)
		embed.set_author(name='Displaying {} results ({}, {})'.format(category.run_name(runtype, matches[0].seeded), matches[0].ver, matches[0].extra))
		embed.add_field(name='-', value=results)

		event.channel.send_message('', embed=embed)


	@Plugin.command('leaderboard', '[args:str...]')
	@Plugin.command('lb', '[args:str...]')
	def on_leaderboard(self, event, args =''):

		embed = MessageEmbed()
		embed.color = 0x00AD96

		# if args == '':
		# 	embed.title = 'Please enter a leaderboard to search for.'
		# 	event.channel.send_message('', embed=embed)
		# 	return

		target = self.index.get_certain_board(args)
		if not target:
			embed.title = 'No leaderboards found. Is this a valid category?'
			event.channel.send_message('', embed=embed)
			return

		offset = 1
		for arg in args:
			if arg[0] == '-':
				try:
					offset = int(arg[1:])
				except:
					embed.title = 'Please enter a valid offset.'
					event.channel.send_message('', embed=embed)
					return

		

	# @Plugin.listen('MessageCreate')
	# def on_message_create(self, event):
	# 	msg = event.message
