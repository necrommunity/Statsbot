

import braincloud as bc
import category

from typing import Literal, Optional

import discord
from discord import app_commands
from discord.embeds import Embed

import steam
import json


ecolor = 0x00AD96

MY_GUILD = discord.Object(id=250565294274117643)

class Client(discord.Client):
    # Suppress error on the User attribute being None since it fills up later
    #user: discord.ClientUser

    def __init__(self, *, intents: discord.Intents):
        super().__init__(intents=intents)
        self.tree = app_commands.CommandTree(self)
    
    # In this basic example, we just synchronize the app commands to one guild.
    # Instead of specifying a guild to every command, we copy over our global commands instead.
    # By doing so, we don't have to wait up to an hour until they are shown to the end-user.
    async def setup_hook(self):
        # This copies the global commands over to your guild.
        self.tree.copy_global_to(guild=MY_GUILD)
        await self.tree.sync(guild=MY_GUILD)




intents = discord.Intents.default()
client = Client(intents=intents)


@client.event
async def on_ready():
    assert client.user is not None
    print(f'Logged in as {client.user} (ID: {client.user.id})')
    print('------')


@client.tree.command(description='Echos bot version.')
async def version(interaction: discord.Interaction):
    reply = Embed(colour=ecolor, title='This is Statsbot v3.0! We do slash commands now. Use `/help` for more info.')
    await interaction.response.send_message(embed=reply)


# @client.tree.command()
# @app_commands.describe(
#     first_value='The first value you want to add something to',
#     second_value='The value you want to add to the first value',
# )
# async def add(interaction: discord.Interaction, first_value: int, second_value: int):
#     """Adds two numbers together."""
#     await interaction.response.send_message(f'{first_value} + {second_value} = {first_value + second_value}')

@client.tree.command(description='Shows a selection of the user\'s Steam stats for Necrodancer.')
@app_commands.describe(name='The display name of the user. Defaults to the server nickname of the user.',
                       steamid='Alternatively, steamid of the user.')
async def stats(interaction: discord.Interaction, name: Optional[str], steamid: Optional[str]):
    reply = Embed(colour=ecolor)

    if not name:
        name = interaction.user.display_name
        
    if steamid:
        steam_user = steam.fill_user(steamid)
        if not steam_user:
            reply.title = 'Failed to retrieve profile for ID "{}".'.format(steamid)
            await interaction.response.send_message(embed=reply)
            return
    else:
        user = bc.search_users(name)
        if not user:
            reply.title = 'No players found called "{}".'.format(user)
            await interaction.response.send_message(embed=reply)
            return
        steam_id = bc.get_steamid(user['profileId'])
        steam_user = steam.User(steam_id, user['profileName'], avatar=user['pictureUrl'])
        if not steam_id:
            reply.title = 'No Steam profile found for user "{}".'.format(user['profileName'])
            await interaction.response.send_message(embed=reply)
            return

    results = steam.get_stats(steam_user)
    if not results:
        reply.add_field(name='Error', value='Failed to retrieve stats for {}. Please make sure "Game details" under steam profile privacy settings is set to "public"'.format(steam_user.name))
        await interaction.response.send_message(embed=reply)
        return

    reply.title = "{} #{}".format(steam_user.name, steam_user.steam_id)
    reply.set_footer(icon_url='https://raw.githubusercontent.com/necrommunity/Statsbot/master/icons/steam.png')
    reply.set_thumbnail(url=steam_user.avatar)
    # reply.add_field(name='Stats', value='`{}`'.format(results))
    for field in ['Playtime', 'Deaths', 'Green bats', 'Approximate clears count']:
        reply.add_field(name=field, value=results[field], inline=False)
    await interaction.response.send_message(embed=reply)


@client.tree.command(description='Fetches entries from the in-game leaderboards.')
@app_commands.describe(char='In-game character.',
                       ranking='Leaderboard type.',
                       index='Shows results starting at offset.',
                       dlc='Amplified DLC (defaults to True).',
                       sync='Sync DLC (defaults to True).',
                       seeded='Seeded runs (defaults to False).',
                       modes_input=f'Subset of {category.mode_strs}.',
                       players='Number of players, if applicable. Defaults to 1.')
@app_commands.rename(char='character', ranking='category', index='offset',
                     dlc='amp', players='number-of-players', modes_input='modes')
async def leaderboard(interaction: discord.Interaction,
                      char: category.char_names, 
                      ranking: Literal['speed', 'score', 'score duping', 'deathless'],
                      index: int = 0,
                      dlc: bool = True,
                      sync: bool = True,
                      seeded: bool = False,
                      modes_input: str = '',
                      players: app_commands.Range[int, 1, 8] = 1):
    reply = Embed(colour=ecolor)
    
    modes_input = modes_input.lower()
    modes_found = set()
    
    if players > 5:
        players = 8
    
    for i, m in enumerate([m.lower().replace('_', ' ') for m in category.mode_strs]):
        if m in modes_input:
            modes_found.add(category.mode_strs[i])
    found_lb = None
    for lb in all_lbs:
        if (lb['char'] == char and lb['ranking'] == ranking and lb['dlc'] == dlc
            and lb['sync'] == sync and lb['seeded'] == seeded and lb['players'] == players):
            if (set(lb['modes']) == modes_found):
                found_lb = category.Leaderboard(lb['lbid'])
    if not found_lb:
        reply.title = 'Did not find leaderboard. Is this a valid combination?'
        await interaction.response.send_message(embed=reply)
        return
    # print(found_lb.lbid)
    
    entries = bc.fetch_lb(found_lb.lbid, index=index)
    if not entries:
        reply.title = 'Failed to fetch entries.'
        await interaction.response.send_message(embed=reply)
        return
        
    rank_width = len(str(index+10))+1
    score_width = len(str(entries[0]['score']))+4
    lbstr = ''
    for e in entries:
        rank = f'{e["rank"]}.'.rjust(rank_width)
        score = category.score_string(e["score"], found_lb).ljust(score_width)
        player = e['name']
        lbstr += '{}  {}  {}\n'.format(rank, score, player)
    
    flags = {'Amp':dlc, 'Sync':sync, 'seeded':seeded}
    flags_str = []
    for k, b in flags.items():
        if b:
            flags_str.append(k)
    
    coop_str = ''
    if players > 1:
        coop_str = '{}-player '.format(players if players<8 else '5-8')
    
    reply.title = f'{ranking.capitalize()} leaderboard for {coop_str}{char}'
    if len(flags_str) > 0:
        reply.title += f' ({", ".join(flags_str)})'
    reply.set_thumbnail(url='https://raw.githubusercontent.com/necrommunity/Statsbot/master/icons/{}.png'.format(char).replace(' ','%20'))
    
    if len(modes_found) > 0:
        reply.add_field(name='Modes',
                    value=', '.join([m.lower().replace('_', ' ') for m in modes_found]).capitalize(), 
                    inline=False)
    
    reply.add_field(name='Results', value=f'```{lbstr}```')
    await interaction.response.send_message(embed=reply)

with open('config.json') as f:
	content = json.loads(f.read())
	token = content['token']

try:
    with open('leaderboards.json') as f:
	    all_lbs = json.loads(f.read())
except:
    print('Failed to read leaderboards.json.')

client.run(token)