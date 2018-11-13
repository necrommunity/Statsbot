# Statsbot
Discord bot created for the CoNDOR server. Fetches and formats Crypt of the Necrodancer stats.


## Features
- Search - information regarding a player's personal bests using Mendayen's api (http://api.toofz.com/help).
- Leaderboard - supports all leaderboards as fetched from the steam api.
- Records - various misc stats such as playtime, deaths, characters clears count, etc that are saved on steam.
- Necrobot - returns results of Incnone's necrobot races from the community database.

### Synopsis

```.search "steamname"
.speed "steamname"
.speed #steamID
.leaderboard "character"
.stats "steamid"
.stats #steamID```


### Name search
Displays a list of players with a matching name, ordered by entry count.

```.search "steamname"
.s "steamname"```


### Player results
Displays the player's personal bests in the specified category.

```.speed "steamname"
.score #steamID
.[category] "steamname" [product] [mode]```

`steamname`: Name searched. The results displayed are of the player with the most entries.

`steamID`: Can be used instead of a name. An argument will be treated as an ID if it begins with '#'.

`product`: By default the results are Amplified only. Adding "classic" as a product will return classic results.

`mode`: Run's game mode, standard by default. (standard, hardmode. noreturn)


### Leaderboards
Displays the specified leaderboard.

```.leaderboard "character"
.lb "character"
.leaderboard [character] [product] [category] -[offset]```

`charcter`: The character type of the leaderboard. (All, Aria, Bard, Bolt, Cadence, Coda, Diamond, Dorian, Dove, Eli, Melody, Monk, Nocturna, Story)

`product`, `category` are the same as search.

`offset`: Changes the range of the displayed entries. The offset is 0 by default.


### Records
Displays various misc steam statistics. *Requires the user profile to be public.*

```.records "steamname"
.records #steamID```

`name/SteamID` is the same as search.


### Necrobot (no longer in use)
Displayed a user's past necrobot races.

```.necrobot "user"
.races "user"
.necrobot "user" -[offset]```

`name`: The discord user to search. By default the command will return the user's results.

`offset` is the same as leaderboards display.


### Help
Displays the help information and a list of commands.

```.help [command]```

`command`: The command to expand on.

### Version
Displays the bot's current version.

```.version```
