[![C#](https://img.shields.io/badge/C%23-9.0-green)](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-9) [![C#](https://img.shields.io/badge/.NET-5.0-orange)](https://dotnet.microsoft.com/en-us/download/dotnet/5.0)

<a href="https://liftoff.publiczeus.com/images/git/logo.png">
  <img src="https://liftoff.publiczeus.com/images/git/logo.png" 
    data-canonical-src="https://liftoff.publiczeus.com" 
      height="120" />
</a>

# **LIFTOFF**
**An all-in-one Steam game launcher!**
LIFTOFF is a standalone x64 WPF application that launches steam games. It can auto-detect your steam games, join display available steam servers for those games, manage mods for those game and even allows your friends on Discord to join you on any game through the click of a button!

## SERVER EXPLORER
LIFTOFF allows users to easily search and display Steam Servers, you can even filter these result and if you cant decide what to play? Roll the dice and LIFTOFF will search and find you a server on an installed game that you can join.

<a href="https://liftoff.publiczeus.com/images/git/game-servers.png">
  <img src="https://liftoff.publiczeus.com/images/git/game-servers.png" 
    data-canonical-src="https://liftoff.publiczeus.com/images/git/game-servers.png" 
      height="200" />
</a>

Later variations of this "roll the dice" function could include features such as "mood" and other questions to better find a compatable server.

## FEATURED GAMES & SERVERS
LIFTOFF can feature and spotlight Game and Servers. Games featured work perticularly well with LIFTOFF, people can reach out to me if they want their games to be added to the featured list on LIFTOFF. This list already includes most Bohemia Interactive and Blazing Griffin titles but will hopefully grow in the future. Featured games are automatically added to LIFTOFF (if installed locally) and is pre-configured to work with all LIFTOFF functions.

Featured servers work quite the same as games, these will be recommeded servers highlighted to the users on LIFTOFF, featured servers can be disable in LIFTOFF settings.

Featured items are usually highlighted yellow and have a Heart icon

<a href="https://liftoff.publiczeus.com/images/git/featured.png">
  <img src="https://liftoff.publiczeus.com/images/git/featured.png" 
    data-canonical-src="https://liftoff.publiczeus.com/images/git/featured.png" 
      height="200" />
</a>

## STEAM GAME AUTO-DETECTION
Using Gameloop.Vdf we can read the local user's Steam Library setup and automatically read AppIDs, from this we can generate games in LIFTOFF and even find and list possible game file locations but ultimately to allow the system to be flexible; the user must explicitly point out the game executable before LIFTOFF can launch.

<a href="https://liftoff.publiczeus.com/images/git/steam-game-detection.png">
  <img src="https://liftoff.publiczeus.com/images/git/steam-game-detection.png" 
    data-canonical-src="https://liftoff.publiczeus.com/images/git/steam-game-detection.png" 
      height="200" />
</a>

## MOD SUPPORT {! WiP !}
Mod support will include the abbility to sync and manage your Steam Workshop mods directly from LIFTOFF, allowing you to enable and disable mods and Presets for your games. For compatable games LIFTOFF will also be able to auto-enable mods for servers on that game but as this has to be pre-configured by the game developer this cannot be as flexable. 

## DISCORD INTEGRATION
Using Discord RPC we can easily integrate joinable sessions through discord, advertising the connection details in the party token will allow users with LIFTOFF to click on their friends and join in on the fun.

<a href="https://liftoff.publiczeus.com/images/git/discord.png">
  <img src="https://liftoff.publiczeus.com/images/git/discord.png" 
    data-canonical-src="https://liftoff.publiczeus.com/images/git/discord.png" 
      height="200" />
</a>

Discord RPC also shows user activity including the server details: player count, map, gamemode and name but also the game details.


# LIFTOFF TOOLS
LIFTOFF is built along with two other programs, the ROCKET application and the LIFTOFF API. Both are used as auxiliary tools for this software.
## [ROCKET](https://github.com/Bruce-Devlin/LIFTOFF-ROCKET)
ROCKET is the "game launcher" used by LIFTOFF, it is a small program with functions to run, close and interact with the processes that LIFTOFF launches. It does this but launches it with parameters which ROCKET uses to execute functions on the appropriate processes.

## [LIFTOFF-API](https://github.com/Jack-Hartman/LIFTOFF-API)
LIFTOFF-API is a web interface for the LIFTOFF to receive steam information, it was generously provided to the project by the fantastic Jack Hartman.
