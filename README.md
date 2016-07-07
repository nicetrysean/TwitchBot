# TwitchBot
Lean IRC/Twitch Bot written in C#, backed with YAML.

# YAML Setup

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
