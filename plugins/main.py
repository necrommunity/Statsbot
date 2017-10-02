import sys
sys.path.append('..')
import toofz

from disco.bot import Plugin


class BasicPlugin(Plugin):

	@Plugin.command('reload', oob=True)
	def on_reload(self, event):
		if event.msg.member.user.username != 'Naymin':
			event.msg.reply('Hyena stop pls')
			return
		self.reload()
		event.msg.reply('Reloaded!')

	@Plugin.command('info')
	def on_info(self, event):
		event.msg.reply('~~Role-bot v0.57~~ maybe statsbot.')

	@Plugin.command('fib', '<content:int>')
	def in_info(self, event, content):
		queue = fib.fib2(content)
		event.msg.reply(str(queue))

	@Plugin.listen('MessageCreate')
	def on_message_create(self, event):
    msg = event.message
