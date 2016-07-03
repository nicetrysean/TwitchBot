# TwitchBot
Simple IRC Twitch Bot written in C#, backed with JSON.

# JSON Setup

- credentials.json
    - Username 
    - Password (http://www.twitchapps.com/tmi/)
    - Channel (your channel starting with #)

- commands.json
    - Name
    - Action (Text / Audio / Vote)
    - Command
    - Text (replies to general twitch chat)
    - Cooldown (in minutes)
    - MisfireText (Text is replied when command is in Cooldown)
    - Filename (name of audio source, Audio/Vote have the ability to play audio)
    
- messages.json
    You can reference other lists by calling #ListName#.
    All responses are chosen at psuedo random and theres no limit to how many references you can have.
    
    - Special Fields
        These lists are all given 'Name' and the Vote list is given 'Count'. These are required.
        - Vote
        - New Vote
        - Vote Succeed
