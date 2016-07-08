# TwitchBot
Lean IRC/Twitch Bot written in C#, backed with YAML.

## Features
- Local audio playback for commands
- Commands for voting on subjects
- Cooldowns
- Text parsing with the ability to use ShuffleBag randomisation.
- Supports using `#Variable#` notation to call in different lists and variables.

## YAML Setup

### User:
```
  Username: username
  Password: (http://www.twitchapps.com/tmi/)
  Channel: "(your channel starting with #)"
```
### Commands:
```
  - Name: Command Name
  Trigger: '!command'
  Action: Text # Can be Text / Vote / Audio
  Text: 'woah, this had a #Random#% chance of firing, and did!'
  MisfireText: 'the stars didn't align'
  VotesRequired: 0
  FileName: audio.mp3
  Cooldown: 1 #measured in minutes
  Random: true
  Chance: 10 # 10% chance
```
### Messages:
```  
  Say:
  - Things
  - To
  - "#Say#" 
```

### Special Fields For Vote Events
    The following are required to be in the Messages!
    - `Vote`
    - `New Vote`
    - `Vote Succeed`

#### Variables Exposed
These variables are exposed as such to be used in Messages.

`#Username#`
> Who invoked the command

`#Name#`
> Command Name

`#Random#`
> When Random is set to true, this will return 0-100%

`#Cooldown#`
> How long the cooldown has left

`#Count#`
> Used for displaying the voting amounts
