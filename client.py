

import discord
from discord.ext import commands
from discord.embeds import Embed

import json
from urllib.parse import quote

import toofz, steam, category
import pandas as pd
import time



intents = discord.Intents.default()

desc = 'wip dont mind me'
bot = commands.Bot(command_prefix='.', case_insensitive=True, description=desc, intents=intents, help_command=None)


ecolor = 0x00AD96




def run_name(run, seeded):
	if seeded:
		return 'Seeded {}'.format(run)
	return run


def check_alt(args):
	if category.caseless_in('-alt', args):
		return True
	return False


@bot.event
async def on_ready():
    print(f'Logged in as {bot.user} (ID: {bot.user.id})')
    print('------')


@bot.command(aliases = ['ver'])
async def version(ctx):
    reply = Embed(colour=ecolor, title='Statsbot v2.1.1. Use `.help` for more info.')
    await ctx.send(embed=reply)

@bot.command()
async def help(ctx, *args):
    args = ' '.join(args)
    args = args.lower()

    embed = Embed(colour=ecolor)

    # clean that up, possibly use discord-py group feature instead of manual parse
    if args == '':
        embed.title = 'Statsbot is a bot which retrieves Crypt of the Necrodancer player stats'
        embed.description = '''\nAvailable commands:
                            \n`.search`, `.speed`, `.score`, `.deathless`, `.leaderboard`, `.stats`, `pb`.
                            \nUse `.help command` for more info about a particular command.
                            \n\n**Frequently asked questions:**
                            \n**Q: I can't find myself!**
                            \nA: Search using your steam name. If you recently changed it, there might be a while before the bot is updated.
                            \n**Q: Why does someone else with my nickname shows up?**
                            \nA: The player brought up is the one with the most records across all the leaderboards. Until it's you, use the command with your steam ID instead of nickname (`.speed #12345678`). If you don't know what your ID is, use `.search <your nick here>` to find out.
                            \n**Q: Is it possible to see my stats for iOS or switch versions?**
                            \nA: Unfortunately there's no way I'm aware of to access that info as of now, all the information presented by the bot comes from steam and Toofz (Mendayen's site).
                            \n**Q: Who are you?**
                            \nA: I'm Statsbot. beep boop. But nah I'm Naymin, feel free to PM me here or on twitter (@nayminyeah).'''
        await ctx.send(embed=embed)
        return
    if 'search' in args:
        embed.title = 'Search for players and their steam IDs'
        embed.description = 'Use `.search` `name` to see a list of results.'
        await ctx.send(embed=embed)
        return
    if 'speed' in args or 'score' in args or 'deathless' in args:
        embed.title = "Display player's personal bests in a specific category as featured in crypt.toofz.com"
        embed.description = '''Use `.speed`, `.score` or`.deathless` `name`.
                            \nAdd `seeded`, `classic`, `hardmode`, `mystery` etc to filter the results.
                            \nSteamID can be used instead of a name by prefixing it with a `#`.'''
        await ctx.send(embed=embed)
        return
    if args == 'leaderboard' or args == 'lb':
        embed.title = "Display an in-game leaderboard"
        embed.description = '''Use `.leaderboard` `character` `category`.
                            \nAdd `seeded`, `classic`, `hardmode`, `mystery` etc to filter the results.
                            \nAdjust entries offset by adding `-number`.'''
        await ctx.send(embed=embed)
        return
    if args == 'stats':
        embed.title = "Display misc statistics recorded by steam"
        embed.description = 'Use `.stats` `name`.'
        await ctx.send(embed=embed)
        return
    if args == 'pb' or args == 'pbs':
        embed.title = "Display player's personal best in default all-zones mode, fetched from warachia's site."
        embed.description = 'Use `.pb` `name`.'
        await ctx.send(embed=embed)
    # add default case lol


@bot.command()
async def search(ctx, *args):
    args = ' '.join(args)
    args = args.lower()

    embed = Embed(colour=ecolor)

    alt = check_alt(args)
    if alt:
        args = args.replace('-alt', '').strip()

    if args == '':
        embed.title = 'Please enter a name to search for.'
        await ctx.send(embed=embed)
        return

    results = toofz.search(quote(args))
    if results == '':
        embed.title = 'No players found called "{}".'.format(args)
        await ctx.send(embed=embed)
        return

    if alt:
        await ctx.send(('```Displaying top player results for "{}":\n\n{}```'.format(args, results)))
        return

    embed.title = 'Displaying top player results for "{}":'.format(args)
    embed.set_thumbnail(url='https://raw.githubusercontent.com/necrommunity/Statsbot/master/icons/search.png')
    embed.add_field(name='-', value='`{}`'.format(results))

    await ctx.send(embed=embed)


@bot.command(aliases = ['lb', 'leaderboard'])
async def leaderboards(ctx, *args):
    args = ' '.join(args)
    args = args.lower()

    embed = Embed(colour=ecolor)

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
                await ctx.send(embed=embed)
                return

    target = index.get_certain_board(quote(args))
    if not target:
        embed.title = 'No such leaderboards found. Category might not be public.'
        await ctx.send(embed=embed)
        return


    results = steam.fetch_lb(target, offset)
    if results == '':
        embed.title = 'No entries found for {} in the {} category ({}, {}).'.format(target.char, run_name(target.run, target.seeded), target.ver, target.extra)
        await ctx.send(embed=embed)
        return

    if alt:
        await ctx.send('```Displaying {} leaderboard for {} ({}, {})\n\n{}```'.format(run_name(target.run, target.seeded), target.char, target.ver, target.extra, results))
        return

    embed.title = 'Displaying {} leaderboard for {} ({}, {})'.format(run_name(target.run, target.seeded), target.char, target.ver, target.extra)
    embed.set_footer(icon_url='https://raw.githubusercontent.com/necrommunity/Statsbot/master/icons/steam.png')
    embed.set_thumbnail(url='https://raw.githubusercontent.com/necrommunity/Statsbot/master/icons/{}.png'.format(target.char).replace(' ','%20'))
    embed.add_field(name='-', value='`{}`'.format(results))

    await ctx.send(embed=embed)


@bot.command(aliases = ['stat'])
async def stats(ctx, *args):
    args = ' '.join(args)
    args = args.lower()

    embed = Embed(colour=ecolor)

    alt = check_alt(args)
    if alt:
        args = args.replace('-alt', '').strip()

    user = ctx.author.display_name

    if args != '':
        user = args.split()[0]
        args = args[len(user)+1:]

    if user[0] == '#':
        user = user[1:]
        steam_user = steam.fill_user(user)
    else:
        steam_user = toofz.get_top(quote(user))

    if not steam_user:
        embed.title = 'No players found called "{}".'.format(user)
        await ctx.send(embed=embed)
        return

    results = steam.get_stats(steam_user)
    # if not results:
    #   embed.title = 'Failed to retrieve stats for {}. Please make sure "Game details" under steam profile privacy settings is set to "public"'.format(steam_user.name)
    #   event.channel.send_message('', embed=embed)
    #   return
    
    if alt:
        await ctx.send('```{} #{}\n\n{}```'.format(steam_user.name, steam_user.steam_id, results))
        return

    embed.title = "{} #{}".format(steam_user.name, steam_user.steam_id)
    embed.set_footer(icon_url='https://raw.githubusercontent.com/necrommunity/Statsbot/master/icons/steam.png')
    embed.set_thumbnail(url=steam_user.avatar)
    embed.add_field(name='Clear Count', value='`{}`'.format(results))

    await ctx.send(embed=embed)


@bot.command(aliases = ['pb'])
async def pbs(ctx, *args):
    args = ' '.join(args)

    embed = Embed(colour=ecolor)

    user = ctx.author.display_name
        
    if args != '':
        user = args.strip()
    try:
        df = pd.read_html('https://warachia2.github.io/NecroRankings/pbs/{}.html'.format(user), header=0, index_col=1)[0]
    except:
        embed.title = 'No page found for "{}". Make sure the name is capitalised correctly.'.format(user)
        await ctx.send(embed=embed)
        return
    
    df.dropna(axis=1, inplace=True)
    df.columns = ['Speed', '', 'Score', '']
    
    embed.title = "{} PBs".format(user)
    embed.set_footer(text='https://warachia2.github.io/NecroRankings', icon_url='https://avatars.githubusercontent.com/u/70665936?v=4')
    embed.add_field(name='-', value='`{}`'.format(df.to_string()))

    await ctx.send(embed=embed)


@bot.group()
async def cool(ctx):
    """Says if a user is cool.
    In reality this just checks if a subcommand is being invoked.
    """
    if ctx.invoked_subcommand is None:
        await ctx.send(f'No, {ctx.subcommand_passed} is not cool')


@cool.command(name='bot')
async def _bot(ctx):
    """Is the bot cool?"""
    await ctx.send('Yes, the bot is cool.')



with open('config.json') as f:
	content = json.loads(f.read())
	token = content['token']

index = category.indexer()

bot.run(token)
