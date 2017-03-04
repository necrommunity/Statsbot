# Statsbot
Discord bot created for the CoNDOR server. Fetches and formats Crypt of the Necrodancer stats.


##Features
- Search - information regarding a player's personal bests using Mendayen's api (http://api.toofz.com/help).
- Leaderboard - supports all leaderboards as fetched from the steam api.
- Records - various misc stats such as playtime, deaths, characters clears count, etc that are saved on steam.
- Necrobot - returns results of Incnone's necrobot races from the community database.


##Command syntax
All commands in public servers must start with `.statsbot` or `.sb`.
This prefix can be dropped in private messages.

###Name search
Displays a list of players with a matching name, ordered by entry count.

.statsbot \<s/search/player/toofz> `<name>`

`name`: The name searched.

>.statsbot search bird

###Player results
Displays the player's personal bests in the specified category.

.statsbot \<s/search/player/toofz> `<name/steamID>` : `<product>`(optional) `<category>`

`name`: Name searched. The results displayed are of the player with the most entries.

`steamID`: Can be used instead of a name. An argument will be treated as an ID if it begins with '#'.

`product`: By default the results are with Amplified only. Adding "classic" as a product will return classic results.

`category`: Run type to return. (score, speed, deathless, seeded score, seeded speed)

>.statsbot search dog: speed

>.statsbot player cat : deathless

>.sb toofz bear : classic seeded score

###Leaderboards
Displays the specified leaderboard.

.statsbot \<leaderboard/lb> `<charcter>` : `<product>`(optional) `<category>` &`<offset>`(optional)

`charcter`: The character type of the leaderboard. (All, Aria, Bard, Bolt, Cadence, Coda, Dorian, Dove, Eli, Melody, Monk, Nocturna, Story)

`product`, `category` are the same as search.

`offset`: Changes the range of the displayed entries. The offset is 0 by default. '&' can be replaced by "offset=".

>.statsbot leaderboard nocturna: deathless

>.statsbot lb bard: seeded speed

>.sb leaderboard all: classic score

###Records
Displays various misc steam statistics.

.statsbot \<records/stats>  `<name/steamID>`

`name/SteamID` is the same as search.

>.statsbot records penguin

>.sb stats horse

###Necrobot
Displayed a user's past necrobot races.

.statsbot \<necrobot/races> `<name>`(optional) &`<offset>`(optional)

`name`: The discord user to search. By default the command will return the user's results.

`offset` is the same as leaderboards display.

>.statsbot necrobot

>.sb races mouse


###Help
Displays the help information and a list of commands.

.statsbot help `<command>`(optional)

`command`: The command to do expand on.

>.statsbot help

>.sb help search

###Version
Displays the bot's current version.

>.statsbot version
