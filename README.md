
# Creating a Discord Bot for Game Stats Tracking and Management System

A quick write up on how to create a stats system that interacts with Unity and Discord
In this detailed guide, we will walk you through creating a system that enables you to send game stats from Unity to a Discord channel via a webhook and then store and manage those stats using a custom Discord bot. The Unity side will be responsible for sending the game stats to Discord, and the Discord bot side will be in charge of processing and managing these stats.




## Unity Side: Sending Game Stats to Discord
1. Setting Up the Unity Script:
Open your Unity project and create a new C# script named DiscordStatsManager.cs.
Attach this script to a GameObject in your scene.

2. Implementing the Unity Script:
Use the code provided under the section "Unity Script". This script handles sending game stats to Discord through a webhook.

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

- Now you have a script that will send your stats to you Discord channel, however now we need to make a Discord bot that will process and manage these stats for us.

## Discord Bot Side: Processing and Managing Stats from Webhook

1. Setting up our Discord Bot:
Setup a new project for a Discord bot using discord.js and create a new application and bot on the Discord developer portal. I will not provide an entire walkthrough on this as there are already many tutorials on YouTube. Like this one from Under Ctrl for example.
- https://www.youtube.com/watch?v=KZ3tIGHU314&t=217s
- Once you have setup your Bot, make sure to copy your Bot Token, and paste it into the "" of botToken inside the Bot Scirpt.

2. Implementing the Bot code:
Use the code provided under the section "Discord Bot Script". This script will control our Discord Bot, which will process and manage our "Stats" data.

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
## Unity Script
This is the script that will handle sending your data to the webhook.

```c#
public class DiscordStatsManager : MonoBehaviour
{
    public bool doSend = true;
    public string webhookUrl;
    private float saveCooldown = 10f; // Cooldown time in seconds
    private float lastSaveTime = 0f;
    private bool isSending = false;

    private void Start()
    {
        lastSaveTime = Time.time - saveCooldown;
    }

    private IEnumerator PostToDiscordCoroutine(string message)
    {
        if (Time.time - lastSaveTime >= saveCooldown && !isSending)
        {
            using (WebClient client = new WebClient())
            {
                isSending = true;
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                string payload = $"content={UnityWebRequest.EscapeURL(message)}";

                // Use UploadStringAsync to send the payload asynchronously
                var uploadTask = client.UploadStringTaskAsync(webhookUrl, payload);

                // Wait for the upload to complete
                yield return new WaitUntil(() => uploadTask.IsCompleted);

                // Handle any errors here if needed
                if (uploadTask.IsFaulted)
                {
                    Debug.LogError("Error sending message to Discord: " + uploadTask.Exception);
                }
                isSending = false;
                lastSaveTime = Time.time;
            }
        }
    }
}
```
## Discord Bot Script
This is the script for our Discord Bot that will process and manage our "Stats".

```js
const { Client, IntentsBitField } = require('discord.js');
const path = require("path");

    const client = new Client({
       intents: [
           IntentsBitField.Flags.Guilds,
           IntentsBitField.Flags.GuildMembers,
           IntentsBitField.Flags.GuildMessages,
           IntentsBitField.Flags.MessageContent,
       ]
    });

const fs = require('fs');

client.once('ready', () => {

    if (fs.existsSync("ids.json"))
    {
        playerTotalValues = loadFromFile();
        playerStats = loadPlayersFromFile();
        userToDeviceMap = loadIdsFromFile();
    }

    console.log(`Logged in as ${client.user.tag}!`);
});

let userToDeviceMap = {}; // Object to store user-device mappings

let playerStats = {}; // An object to store stats for each player

let playerTotalValues  =  {
    TotalEarned: 0,
    TotalSpent: 0,
    ObjectsPlaced: 0,
    TimePlayed: 0,
    SeedsPlanted: 0,
    PlantsHarvested: 0,
    GramsPressed: 0,
    OzsSold: 0,
    PlantsKilled: 0,
    TotalPlayers: 0,
};

let globalNewValues = null;

let webhookUserID = "";
let statsChannelID = "";
let yourUserID = "";
let gameIDLength = 32;
let botToken = "";

client.on("messageCreate", async (msg) => {
    if (msg.channel.id === statsChannelID && msg.author.id === webhookUserID) {
        // Parse values from the webhook message content
        const newValues = {
            UniqueID: parseValue(msg.content, 'Game ID'),
            TotalEarned: parseInt(parseValue(msg.content, 'Total $ Earned'), 10),
            TotalSpent: parseInt(parseValue(msg.content, 'Total $ Spent'), 10),
            ObjectsPlaced: parseInt(parseValue(msg.content, 'Total Objects Placed'), 10),
            TimePlayed: parseInt(parseValue(msg.content, 'Total Time Played')),
            SeedsPlanted: parseInt(parseValue(msg.content, 'Total Seeds Planted'), 10),
            PlantsHarvested: parseInt(parseValue(msg.content, 'Total Plants Harvested'), 10),
            GramsPressed: parseInt(parseValue(msg.content, 'Total Grams Pressed'), 10),
            OzsSold: parseInt(parseValue(msg.content, 'Total Ozs Sold'), 10),
            PlantsKilled: parseInt(parseValue(msg.content, 'Total Plants Killed'), 10),
        };

        globalNewValues = newValues;

        // Check if the player's stats object exists in the playerStats object
        if (!playerStats[newValues.UniqueID]) {
            playerStats[newValues.UniqueID] = {
                TotalEarned: newValues.TotalEarned,
                TotalSpent: newValues.TotalSpent,
                ObjectsPlaced: newValues.ObjectsPlaced,
                TimePlayed: newValues.TimePlayed,
                SeedsPlanted: newValues.SeedsPlanted,
                PlantsHarvested: newValues.PlantsHarvested,
                GramsPressed: newValues.GramsPressed,
                OzsSold: newValues.OzsSold,
                PlantsKilled: newValues.PlantsKilled,
                // Add more properties for other values
            };

            // Update the player's stats object with new values
            playerTotalValues.TotalPlayers += 1;
            playerTotalValues.TotalEarned += newValues.TotalEarned;
            playerTotalValues.TotalSpent += newValues.TotalSpent;
            playerTotalValues.ObjectsPlaced += newValues.ObjectsPlaced;
            playerTotalValues.SeedsPlanted += newValues.SeedsPlanted;
            playerTotalValues.PlantsHarvested += newValues.PlantsHarvested;
            playerTotalValues.TimePlayed += newValues.TimePlayed;
            playerTotalValues.GramsPressed += newValues.GramsPressed;
            playerTotalValues.OzsSold += newValues.OzsSold;
            playerTotalValues.PlantsKilled += newValues.PlantsKilled;
            // Update other properties
        }
        else {
            const earned = newValues.TotalEarned - playerStats[newValues.UniqueID].TotalEarned;
            const spent = newValues.TotalSpent - playerStats[newValues.UniqueID].TotalSpent;
            const placed = newValues.ObjectsPlaced - playerStats[newValues.UniqueID].ObjectsPlaced;
            const time = newValues.TimePlayed - playerStats[newValues.UniqueID].TimePlayed;
            const seeds = newValues.SeedsPlanted - playerStats[newValues.UniqueID].SeedsPlanted;
            const plants = newValues.PlantsHarvested - playerStats[newValues.UniqueID].PlantsHarvested;
            const grams = newValues.GramsPressed - playerStats[newValues.UniqueID].GramsPressed;
            const ozs = newValues.OzsSold - playerStats[newValues.UniqueID].OzsSold;
            const killed = newValues.PlantsKilled - playerStats[newValues.UniqueID].PlantsKilled;

            playerStats[newValues.UniqueID].TotalEarned = newValues.TotalEarned;
            playerStats[newValues.UniqueID].TotalSpent = newValues.TotalSpent;
            playerStats[newValues.UniqueID].ObjectsPlaced = newValues.ObjectsPlaced;
            playerStats[newValues.UniqueID].TimePlayed = newValues.TimePlayed;
            playerStats[newValues.UniqueID].SeedsPlanted = newValues.SeedsPlanted;
            playerStats[newValues.UniqueID].PlantsHarvested = newValues.PlantsHarvested;
            playerStats[newValues.UniqueID].GramsPressed = newValues.GramsPressed;
            playerStats[newValues.UniqueID].OzsSold = newValues.OzsSold;
            playerStats[newValues.UniqueID].PlantsKilled = newValues.PlantsKilled;

            playerTotalValues.TotalEarned += earned;
            playerTotalValues.TotalSpent += spent;
            playerTotalValues.ObjectsPlaced += placed;
            playerTotalValues.SeedsPlanted += seeds;
            playerTotalValues.PlantsHarvested += plants;
            playerTotalValues.TimePlayed += time;
            playerTotalValues.GramsPressed += grams;
            playerTotalValues.OzsSold += ozs;
            playerTotalValues.PlantsKilled += killed;
        }

        try {
            // Delete the current message first
            await msg.delete();

            // Fetch messages in the channel
            const messages = await msg.channel.messages.fetch({ limit: 100 });

            // Filter all messages that contain the same Unique ID as the new message
            const messagesToDelete = messages.filter((message) => {
                const uniqueIdFromMessage = parseValue(message.content, 'Unique ID');
                return uniqueIdFromMessage === newValues.UniqueID;
            });

            // Delete each found message
            for (const message of messagesToDelete.values()) {
                await message.delete();
            }
        } catch (error) {
            console.error('Error deleting messages:', error);
        }

        const updatedMessage = createUpdatedMessage(playerStats[newValues.UniqueID], newValues.UniqueID);

    }

    if (msg.content.startsWith('!id') && msg.author.id === yourUserID) {
        const userID = msg.mentions.users.first() ? msg.mentions.users.first().id : msg.author.id;

        if (userToDeviceMap[userID]) {
            const deviceID = userToDeviceMap[userID];
            await msg.reply(`User's Game ID is: ${deviceID}`);
        } else {
            await msg.reply(`No ID found.`);
        }
    }

    // Check if the user sent the !unlink command
    if (msg.content === '!unlink') {
        const userID = msg.author.id;

        if (userToDeviceMap[userID]) {
            delete userToDeviceMap[userID];
            await msg.reply(`Game ID unlinked successfully!`);
        } else {
            await msg.reply(`You don't have a linked device ID.`);
        }
    }

    // Check if the user sent the !link command
    if (msg.content.startsWith('!link')) {
        // Extract the device ID from the command
        const commandParts = msg.content.split(' ');
        if (commandParts.length === 2) {
            const deviceID = commandParts[1];

            if (deviceID.length !== gameIDLength)
            {
                await msg.reply("Please provide a valid Game ID!");
            }
            else
            {
                const userID = msg.author.id;

                if (userToDeviceMap[userID]) {
                    await msg.reply(`You already have a device ID linked. Use !unlink to unlink your current device ID.`);
                } else {
                    // Store the device ID in the object
                    userToDeviceMap[userID] = deviceID;

                    await msg.reply(`Game ID linked successfully!`);
                }
            }
        } else {
            await msg.reply(`Invalid command format. Use !link GAME_ID`);
        }
        await msg.delete();
    }

    if (msg.content.startsWith("!stats"))
    {
        const userID = msg.mentions.users.first() ? msg.mentions.users.first().id : msg.author.id;

        if (userToDeviceMap[userID]) {
            const deviceID = userToDeviceMap[userID];
            if (playerStats[deviceID]) {
                await msg.reply(createUpdatedPublicMessage(playerStats[deviceID], deviceID));
            }
            else
            {
                await msg.reply(`No saved stats found for Game ID.`);
            }
        } else {
            await msg.reply(`Discord account does not have a linked game ID.`);
        }
    }

    if (msg.content.startsWith("!resetid") && msg.author.id === yourUserID)
    {
        const commandParts = msg.content.split(' ');
        const deviceID = commandParts[1];

        if (playerStats[deviceID]) {

            playerTotalValues.TotalEarned -= playerStats[deviceID].TotalEarned;
            playerTotalValues.TotalSpent -= playerStats[deviceID].TotalSpent;
            playerTotalValues.ObjectsPlaced -= playerStats[deviceID].ObjectsPlaced;
            playerTotalValues.SeedsPlanted -= playerStats[deviceID].SeedsPlanted;
            playerTotalValues.PlantsHarvested -= playerStats[deviceID].PlantsHarvested;
            playerTotalValues.TimePlayed -= playerStats[deviceID].TimePlayed;
            playerTotalValues.GramsPressed -= playerStats[deviceID].GramsPressed;
            playerTotalValues.OzsSold -= playerStats[deviceID].OzsSold;
            playerTotalValues.PlantsKilled -= playerStats[deviceID].PlantsKilled;

            playerStats[deviceID] = null;
            await msg.reply("Stats reset");
            await msg.delete();
        } else {
            await msg.reply(`ID does not have stats.`);
            await msg.delete();
        }
    }

    if (msg.content.startsWith("!resetstats") && msg.author.id === yourUserID)
    {
        const userID = msg.mentions.users.first() ? msg.mentions.users.first().id : msg.author.id;

        if (userToDeviceMap[userID]) {
            playerStats[userToDeviceMap[userID]] = null;
            await msg.reply("Stats reset");
        } else {
            await msg.reply(`Discord account does not have a linked game ID.`);
        }
    }

    if (msg.content === "!globalstats")
    {
        // Create and send updated message
        const updatedMessage = createUpdatedMessage(playerTotalValues, "global");
        await msg.reply(updatedMessage);
    }

    if (msg.content === "!save" && msg.author.id === yourUserID)
    {
        saveToFile(playerTotalValues);
        saveIdsToFile(userToDeviceMap);
        await msg.reply("File saved");
    }

    if (msg.content === "!load" && msg.author.id === yourUserID)
    {
        playerTotalValues = loadFromFile();
        userToDeviceMap = loadIdsFromFile();
        await msg.reply("File loaded");
    }

    if (msg.content === "!delete" && msg.author.id === yourUserID)
    {
        deleteSave();
        await msg.reply("File deleted");
    }
});

function parseValue(content, label) {
    const labelIndex = content.indexOf(label);
    if (labelIndex !== -1) {
        const startIndex = labelIndex + label.length + 2; // Adjusted to skip colon and space
        const endIndex = content.indexOf('\n', startIndex);
        if (endIndex !== -1) {
            return content.substring(startIndex, endIndex).trim();
        }
    }
    return "";
}

function formatTime(seconds) {
    if (seconds === 0) {
        return '0s'; // Return 0 seconds directly
    }

    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    const remainingSeconds = seconds % 60;

    const formattedTime = [];

    if (hours > 0) {
        formattedTime.push(`${hours}h`);
    }

    if (minutes > 0) {
        formattedTime.push(`${minutes}m`);
    }

    if (remainingSeconds > 0) {
        formattedTime.push(`${remainingSeconds}s`);
    }

    return formattedTime.join(' ');
}

function createUpdatedPublicMessage(values) {
    const formattedTimePlayed = formatTime(values.TimePlayed);

    return `Total Money Earned: ${values.TotalEarned}\nTotal $ Spent: ${values.TotalSpent}\nTotal Objects Placed: ${values.ObjectsPlaced}\nTotal Time Played: ${formattedTimePlayed}\nTotal Seeds Planted: ${values.SeedsPlanted}\nTotal Plants Harvested: ${values.PlantsHarvested}\nTotal Grams Pressed: ${values.GramsPressed}\nTotal Ozs Sold: ${values.OzsSold}\nTotal Plants Killed: ${values.PlantsKilled}`;
}

function createUpdatedMessage(values, playerID) {
    const formattedTimePlayed = formatTime(values.TimePlayed);

    if (playerID === "global")
    {
        return `Global Stats\nTotal Money Earned: ${values.TotalEarned}\nTotal $ Spent: ${values.TotalSpent}\nTotal Objects Placed: ${values.ObjectsPlaced}\nTotal Time Played: ${formattedTimePlayed}\nTotal Seeds Planted: ${values.SeedsPlanted}\nTotal Plants Harvested: ${values.PlantsHarvested}\nTotal Grams Pressed: ${values.GramsPressed}\nTotal Ozs Sold: ${values.OzsSold}\nTotal Plants Killed: ${values.PlantsKilled}`;
    }
    else
    {
        return `Unique ID: ${playerID}\nTotal Money Earned: ${values.TotalEarned}\nTotal $ Spent: ${values.TotalSpent}\nTotal Objects Placed: ${values.ObjectsPlaced}\nTotal Time Played: ${formattedTimePlayed}\nTotal Seeds Planted: ${values.SeedsPlanted}\nTotal Plants Harvested: ${values.PlantsHarvested}\nTotal Grams Pressed: ${values.GramsPressed}\nTotal Ozs Sold: ${values.OzsSold}\nTotal Plants Killed: ${values.PlantsKilled}`;
    }
}

function saveToFile(values) {
    const data = JSON.stringify(values, null, 2); // Convert to JSON format with indentation
    fs.writeFileSync('playerTotalValues.json', data); // Write to a file named 'playerTotalValues.json'
    console.log('Player total values saved to file.');
}

function savePlayersToFile(values) {
    const data = JSON.stringify(values, null, 2); // Convert to JSON format with indentation
    fs.writeFileSync('playersValues.json', data); // Write to a file named 'playerTotalValues.json'
    console.log('Players values saved to file.');
}

function saveIdsToFile(values) {
    const data = JSON.stringify(values, null, 2); // Convert to JSON format with indentation
    fs.writeFileSync('ids.json', data); // Write to a file named 'playerTotalValues.json'
    console.log('Ids saved to file.');
}

function loadPlayersFromFile() {
    const filename = 'playersValues.json';
    if (fs.existsSync(filename)) {
        try {
            const data = fs.readFileSync(filename, 'utf8');
            const loadedValues = JSON.parse(data);
            console.log('Players values loaded from file.');
            return loadedValues;
        } catch (error) {
            console.error('Error loading Players values:', error);
            return null; // Return null or default values if loading fails
        }
    } else {
        console.log(`${filename} does not exist. Skipping loading.`);
        return null; // Return null or default values if file doesn't exist
    }
}

function loadIdsFromFile() {
    const filename = 'ids.json';
    if (fs.existsSync(filename)) {
        try {
            const data = fs.readFileSync(filename, 'utf8');
            const loadedValues = JSON.parse(data);
            console.log('Ids loaded from file.');
            return loadedValues;
        } catch (error) {
            console.error('Error loading Ids:', error);
            return null; // Return null or default values if loading fails
        }
    } else {
        console.log(`${filename} does not exist. Skipping loading.`);
        return null; // Return null or default values if file doesn't exist
    }
}

function loadFromFile() {
    const filename = 'playerTotalValues.json';
    if (fs.existsSync(filename)) {
        try {
            const data = fs.readFileSync(filename, 'utf8');
            const loadedValues = JSON.parse(data);
            console.log('Player total values loaded from file.');
            return loadedValues;
        } catch (error) {
            console.error('Error loading player total values:', error);
            return null; // Return null or default values if loading fails
        }
    } else {
        console.log(`${filename} does not exist. Skipping loading.`);
        return null; // Return null or default values if file doesn't exist
    }
}

function deleteSave() {
    const filePath = 'playerTotalValues.json';
    fs.unlink(filePath, (error) => {
        if (error) {
            console.error('Error deleting file:', error);
        } else {
            console.log('File deleted successfully.');
        }
    });
}

// Code to run on bot exit (shutdown)
process.on('SIGINT', () => {
    console.log('Bot is shutting down...');

    saveToFile(playerTotalValues);
    saveIdsToFile(userToDeviceMap);
    savePlayersToFile(playerStats);

    client.destroy(); // Close the bot connection gracefully
    process.exit(0); // Exit the process
});

client.login(
      botToken
    );
```
## Help
If you run into any issues or need some help, feel free to dm me on Discord: p.penguin
