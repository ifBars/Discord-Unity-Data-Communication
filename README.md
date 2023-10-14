
# Creating a Discord Bot for Game Stats Tracking and Management System

A quick write up on how to create a stats system that interacts with Unity and Discord
In this detailed guide, we will walk you through creating a system that enables you to send game stats from Unity to a Discord channel via a webhook and then store and manage those stats using a custom Discord bot. The Unity side will be responsible for sending the game stats to Discord, and the Discord bot side will be in charge of processing and managing these stats.




## Unity Side: Sending Game Stats to Discord
1. Setting Up the Unity Script:
Open your Unity project and create a new C# script named DiscordStatsManager.cs.
Attach this script to a GameObject in your scene.

2. Implementing the Unity Script:
Copy and paste the [UnityScript.cs](UnityScript.cs), from the repo, into your new C# script. This script handles sending game stats to Discord through a webhook.

3. Create a Discord webhook.
- Open the Discord channel you want to receive stats in.
- From the channel menu, select Edit channel.
- Select Integrations.
- If there are no existing webhooks, select Create Webhook. 
- Otherwise, select View Webhooks then New Webhook.
- Enter the name of the bot to post the message.
- Optional. Edit the avatar.
- Copy the URL from the WEBHOOK URL field.
- Select Save.

4. Find you game object that contains your DiscordStatsManager class, and paste in your webhook link.

5. Creating your "Stats" class:
Create a class that will hold the data you are wanting to send to Discord. See example below.
```c#
// Class to hold our "Stats"
public class StatsHandler : MonoBehaviour
{
    // Our "Stats"
    public float gameID;
    public float currentBal;
    public float totalEarned;
    public float totalSpent;
    public float objectsPlaced;
    public float timePlayed;
    public float seedsPlanted;
    public float plantsHarvested;
    public float gramsPressed;
    public float ozsSold;
    public float plantsKilled;
}
```

6. Creating our send "Stats" to Discord method:
- Create a method to take in your "Stats" class and forms a message to send to Discord from the "Stats". See example below, this example would be pasted into your DiscordStatsManager.
```c#
// Our custom method to format our stats into a message to send to the Discord webhook
public void SendStatsToDiscord(StatsHandler sd)
    {
        // If "do send" option is enabled then we send the message (can change in editor in case you don't want stats to be sent temporarily)
        if (doSend)
        {
            // Formatting our message, \n represents a new line
            // Hey, I'm a new line :D
            string message = $"Unique ID: {sd.gameID}\nTotal $ Earned: {sd.totalEarned}\nTotal $ Spent: {sd.totalSpent}\nTotal Objects Placed: {sd.objectsPlaced}\nTotal Time Played: {sd.timePlayed}\nTotal Seeds Planted: {sd.seedsPlanted}\nTotal Plants Harvested: {sd.plantsHarvested}\nTotal Grams Pressed: {sd.gramsPressed}\nTotal Ozs Sold: {sd.ozsSold}\nTotal Plants Killed: {sd.plantsKilled}\nEnd";

            // Debug to console to let you know stats are being sent to Discord
            Debug.Log("Sending stats - " + message);

            // Calling our PostToDiscord method and passing in our formatted stats message
            StartCoroutine(PostToDiscordCoroutine(message));
        }
    }
```
- If you want this method somewhere other than your DiscordStatsManager class, you can pass the gameobject through the editor.
- Simply copy and paste "public DiscordStatsManager discordManager;" to the beginning of your class, and reference your DiscordStatsManager gameObject from the scene.
- Now you can use StartCoroutine(discordManager.PostToDiscordCoroutine(message));

7. Finally, call your method upon saving the game, or updating said stats. Keep in mind, the PostToDiscordCoroutine has a cooldown of 10 seconds. If you would like to change this cooldown, simply change the saveCooldown in your Unity script.

- Now you have a script that will send your stats to your Webhooked Discord channel, however now we need to make a Discord bot that will process and manage these stats for us.

## Discord Bot Side: Processing and Managing Stats from Webhook

1. Setting up our Discord Bot:
Setup a new project for a Discord bot using discord.js and create a new application and bot on the Discord developer portal. I will not provide an entire walkthrough on this as there are already many tutorials on YouTube. Like this one from Under Ctrl for example.
- https://www.youtube.com/watch?v=KZ3tIGHU314&t=217s

2. Implementing the Bot code:
Use the code provided [DiscordBot.js](DiscordBot.js). This script will control our Discord Bot, which will process and manage our "Stats" data.
- Once you have setup your Bot and implemented the Bot Script, make sure to copy your Bot Token, and paste it into the "" of botToken inside the Bot Script, at the bottom.

3. Implementing your Discord IDs:
Now we need to get both your Webhook UserID, your Discord UserID, and the ChannelID, in order for the bot to know what messages are from the Unity script, and to allow only you to be able to perform actions like, resetting stats, and what not.
- First, open the channel where you created your webhook, and copy the webhook url
- Then, head to https://discohook.org and paste in your webhook url at the top
- Optionally, change the "content", this does not matter as we will be deleting this message anyways.
- Hit send, then go to your Discord
- Open your settings, scroll down and press "Advanced"
- Make sure "Developer Mode" is enabled
- Now go back to the channel where your webhook sent the message, right click on the webhook "BOT" and press copy User ID
- Paste your Webhook's User ID inside the "" of webhookUserID in the Bot script
- Go back to the channel, right click the channel, and press copy channel ID
- Paste your channel id inside the "" of statsChannelID in the Bot script
- Then, go back to the Discord, right click on your profile, and hit copy User ID
- Finally, paste your UserID inside the "" of yourUserID in the Bot script

4. Change the gameIDLength to the length of your game ID, lets say your Game ID's are a random string of 32 characters, you would change gameIDLength to 32.

5. Now all you need to do is have some form of creating a "Game ID" in your Unity game unique to that player's PC, Save slot, or something like that. That way player's can use !link game_id, and the Bot will now know to link the stats of given game id with the discord account that linked with the game_id.
- For example, you could use SystemInfo.deviceUniqueIdentifier, while this definitely isnt the best form of creating a Game ID, it will provide an unique ID specfic to that player's PC.
- You could also do something like, just generate a random string of certain length upon players starting a new game, and save that to their PC locally for use later.

## Help
If you run into any issues or need some help, feel free to dm me on Discord: p.penguin
