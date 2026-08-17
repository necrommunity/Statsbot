# Statsbot v3.0
Discord bot created for the CoNDOR server. Fetches and formats Crypt of the Necrodancer stats.


## Features
Uses slash commands now!
- Leaderboard - supports all in-game leaderboards.
- Stats - various miscellaneous stats tracked by steam such as playtime, deaths, characters clears count, and others.

## History
This is the third (give or take) rewrite of the project since the launch in 2017. Past versions include
- C# implementation based on Toofz API. Discord library stopped supporting unix-based compilation.
- Python rewrite using text-based commands, still based on Toofz API. Historically supported the most features (Toofz: username search and personal bests, Steam: leaderboards and miscellaneous stats, Necrobot (racing results): database access, Warachia: alternate source of personal bests). Changed discord library at some point after it was deprecated.
- Current version - a few years back, Toofz went offline and with it, the user search function and ability to fetch personal bests. Additionally, with the release of Necrodancer: Synchrony, the Steam leaderboards were deprecated in favour of a more sustainable service. The bot was on life support for a few years until Discord forced my hand by removing message content (text-based commands) privileges. With Necrodancer's developers' permission, service access (thanks Maru!), and an additional node client, username search and leaderboard functionalities are now available and can be used with slash commands.
