# Statsbot
Discord bot created for the CoNDOR server. Fetches and formats Crypt of the Necrodancer stats.


## Features
- Search - information regarding a player's personal bests using Mendayen's api (http://api.toofz.com/help).
- Leaderboard - supports all leaderboards as fetched from the steam api.
- Records - various misc stats such as playtime, deaths, characters clears count, etc that are saved on steam.
- Necrobot - returns results of Incnone's necrobot races from the community database.


## Command syntax
All commands in public servers must start with a `.`
The only exception currently is CoNDOR S5 server (to prevent conflicts with inc's bot)

### Name search
Displays a list of players with a matching name, ordered by entry count.

\<.s/.search> `<name>`

`name`: The name searched.

>.search bird

### Player results
Displays the player's personal bests in the specified category.

\<.speed/.score/.deathless> `<name/steamID>` `<product>`(optional) `<mode>`(optional)

`name`: Name searched. The results displayed are of the player with the most entries.

`steamID`: Can be used instead of a name. An argument will be treated as an ID if it begins with '#'.

`product`: By default the results are Amplified only. Adding "classic" as a product will return classic results.

`mode`: Run's game mode, standard by default. (standard, hardmode. noreturn)

>.speed dog

>.score cat seeded classic

>.deathless bear noreturn

### Leaderboards
Displays the specified leaderboard.

\<.leaderboard/.lb> `<charcter>` `<product>`(optional) `<category>`(optional) &`<offset>`(optional)

`charcter`: The character type of the leaderboard. (All, Aria, Bard, Bolt, Cadence, Coda, Diamond, Dorian, Dove, Eli, Melody, Monk, Nocturna, Story)

`product`, `category` are the same as search.

`offset`: Changes the range of the displayed entries. The offset is 0 by default.

>.leaderboard nocturna deathless

>.lb bard seeded speed

>.leaderboard all classic score &15

### Records
Displays various misc steam statistics.

\<.records/.stats>  `<name/steamID>`

`name/SteamID` is the same as search.

>.s records penguin

>.stats horse

### Necrobot
Displayed a user's past necrobot races.

\<.necrobot/.races> `<name>`(optional) &`<offset>`(optional)

`name`: The discord user to search. By default the command will return the user's results.

`offset` is the same as leaderboards display.

>.necrobot

>.races mouse &20


### Help
Displays the help information and a list of commands.

.help `<command>`(optional)

`command`: The command to do expand on.

>.help

>.help search

### Version
Displays the bot's current version.

>.version
