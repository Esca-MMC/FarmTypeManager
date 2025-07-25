# Farm Type Manager (FTM)
Despite the name, Farm Type Manager (FTM) is a framework that allows other mods to spawn objects and monsters anywhere in the game. It can also customize objects and monsters in unique ways. Modders can configure or randomize the number of objects spawned per day, where and when they spawn, and more.

**🌐 In other languages: [zh (中文)](docs/readme_zh.md)**.

## Contents
* [Installation](#installation)
* [Translation](#translation)
	* [Available Languages](#available-languages)
* [Examples](#examples)
* [Commands](#commands)
	* [whereami](#whereami)
	* [list_monsters](#list_monsters)
* [Mod Settings](#mod-settings)
* [Spawn Settings](#spawn-settings)
    * [Basic Settings](#basic-settings)
    * [General Spawn Settings](#general-spawn-settings)
		* [Item Settings](#item-settings)
		* [Spawn Timing Settings](#spawn-timing-settings)
		* [Extra Conditions](#extra-conditions)
    * [Forage Spawn Settings](#forage-spawn-settings)
    * [Large Object Spawn Settings](#large-object-spawn-settings)
    * [Ore Spawn Settings](#ore-spawn-settings)
    * [Monster Spawn Settings](#monster-spawn-settings)
		* [Monster Type Settings](#monster-type-settings)
    * [Other Settings](#other-settings)
    * [File Conditions](#file-conditions)
* [Content Packs](#content-packs)
* [Additional Modding Features](#additional-modding-features)
    * [For SMAPI Mods](#for-smapi-mods)
		* [FTM API](#ftm-api)
		* [Custom Monster Classes](#custom-monster-classes)
    * [For Content Packs](#for-content-packs)
		* [Content Patcher Tokens](#content-patcher-tokens)
		* [Game State Queries](#game-state-queries)

## Installation
1. **Install the latest version of [SMAPI](https://smapi.io/).**
2. **Download FarmTypeManager** from [the Releases page on GitHub](https://github.com/Esca-MMC/FarmTypeManager/releases), [Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/3231/), or [ModDrop](https://www.moddrop.com/stardew-valley/mods/598755-farm-type-manager-ftm).
3. **Unzip FarmTypeManager** into your `Stardew Valley\Mods` folder.

If you use other mods that require FTM, then you’re all set!

## Translation
FTM supports translation of its Generic Mod Config Menu (GMCM) setting names and descriptions.

The mod will load a file from the `FarmTypeManager/i18n` folder that matches the current language code. If no matching translation exists, it will use [`default.json`](https://github.com/Esca-MMC/FarmTypeManager/blob/master/FarmTypeManager/i18n/default.json).

See the Stardew Valley Wiki's [Modding:Translations](https://stardewvalleywiki.com/Modding:Translations) page for more information. Please feel free to submit translation files through GitHub, Nexus Mods, ModDrop, or Discord.

### Available Languages
Language | File | Contributor(s)
---------|------|------------
English | [default.json](https://github.com/Esca-MMC/FarmTypeManager/blob/master/FarmTypeManager/i18n/default.json) | [Esca-MMC](https://github.com/Esca-MMC)
Chinese (Simplified) | [zh.json](https://github.com/Esca-MMC/FarmTypeManager/blob/master/FarmTypeManager/i18n/zh.json) | [SummerFleur](https://github.com/SummerFleur2997)
Dutch | [nl.json](https://github.com/Esca-MMC/FarmTypeManager/blob/master/FarmTypeManager/i18n/nl.json) | [UnrealThingTriesCode](https://github.com/UnrealThingTriesCode)
French | [fr.json](https://github.com/Esca-MMC/FarmTypeManager/blob/master/FarmTypeManager/i18n/fr.json) | [Fsecho7](https://next.nexusmods.com/profile/Fsecho7)
German | [de.json](https://github.com/Esca-MMC/FarmTypeManager/blob/master/FarmTypeManager/i18n/de.json) | [777PamPam777](https://next.nexusmods.com/profile/777PamPam777)
Portuguese (Brazilian) | [pt-BR.json](https://github.com/Esca-MMC/FarmTypeManager/blob/master/FarmTypeManager/i18n/pt-BR.json) | [roanrobersson](https://github.com/roanrobersson)
Turkish | [tr.json](https://github.com/Esca-MMC/FarmTypeManager/blob/master/FarmTypeManager/i18n/tr.json) | [Rupurudu](https://github.com/Rupurudu)
Ukrainian | [uk.json](https://github.com/Esca-MMC/FarmTypeManager/blob/master/FarmTypeManager/i18n/uk.json) | [burunduk](https://github.com/burunduk)

## Examples
Below are a few general examples of changes modders can make, spawning various objects on the farm or the other in-game maps.

### Spawn forage and respawn existing stumps on the farm
```
"ForageSpawnEnabled": true,
"LargeObjectSpawnEnabled": true,
```
![Your farm's stumps will respawn every day.](docs/images/ftm_example_1.png)

### Randomly spawn new logs and boulders on the farm
```
"LargeObjectSpawnEnabled": true,
```
```
"Large_Object_Spawn_Settings": {
    "Areas": [
      {
        "ObjectTypes": [
          "Log", "Boulder"
        ],
        "FindExistingObjectLocations": false,
        "MapName": "Farm",
        "MinimumSpawnsPerDay": 0,
        "MaximumSpawnsPerDay": 2,
        "IncludeTerrainTypes": ["Grass", "Dirt", "Diggable"],
```
![Most days, logs and/or boulders will spawn on the farm.](docs/images/ftm_example_2.png)

### Spawn ore in a specific area of Cindersap Forest
```
"OreSpawnEnabled": true,
```
```
"Ore_Spawn_Settings": {
    "Areas": [
      {
        "MapName": "Forest",
        "MinimumSpawnsPerDay": 1,
        "MaximumSpawnsPerDay": 5,
        "IncludeCoordinates": [ "65,22/74,27" ],
```
![Ore will spawn in a specific area of the Forest map.](docs/images/ftm_example_3.png)

### Spawn LOTS of forage on the farm, but not near the house
```
"ForageSpawnEnabled": true,
```
```
"Forage_Spawn_Settings": {
    "Areas": [
      {
        "MapName": "Farm",
        "MinimumSpawnsPerDay": 9999,
        "MaximumSpawnsPerDay": 9999,
        "IncludeTerrainTypes": [ "All" ],
        "ExcludeCoordinates": [ "69,17;57,10" ]
```
![Forage will spawn everywhere on the Farm map, except around the house.](docs/images/ftm_example_4.png)

### Spawn [modded plants](https://www.nexusmods.com/stardewvalley/mods/1598) on the [mod-enabled summit](https://www.nexusmods.com/stardewvalley/mods/2073), but only after rainy days, and only after year 1
```
"ForageSpawnEnabled": true,
```
```
  "Forage_Spawn_Settings": {
    "Areas": [
      {
        "SpringItemIndex": [ "Mint", "Juniper" ],
        "SummerItemIndex": [ "Mint", "Juniper" ],
        "FallItemIndex": [ "Mint", "Juniper" ],
        "WinterItemIndex": [],
        "MapName": "Summit",
        "MinimumSpawnsPerDay": 4,
        "MaximumSpawnsPerDay": 8,
        "IncludeTerrainTypes": [
          "All"
        ],
        "ExtraConditions": {
          "Years": [ "2+" ],
          "WeatherYesterday": [ "Rain", "Storm" ],
```
![Custom forage spawned on a mod-enabled map with specific time and weather conditions.](docs/images/ftm_example_5.png)

## Commands
This mod adds the following commands to SMAPI's console.

These commands require the Console Commands mod, which is installed automatically by SMAPI. They can also be disabled in FTM's **config.json** file if desired. See [Mod Settings](#mod-settings).

### whereami

Enter `whereami` in SMAPI's console to display information about the current map, including: 

* The map's name (e.g. "Farm" or "BusStop")
* Your current tile's X/Y coordinates
* The tile's terrain type (e.g. "Dirt" or "Stone")
* Whether the tile is "Diggable" with tools
* The tile's image index number (used to identify "Quarry" tiles or set up the "Custom" tile list)

### list_monsters

Enter `list_monsters` in SMAPI's console to display a list of available monster names to use with the MonsterName spawn setting.

The command will list the primary name of each monster from Stardew Valley itself, and then scan other mods for custom monster classes.

The full provided name should be used in the MonsterName setting. Example: `"MyModName.CustomMonster"`

### remove_items

Enter `remove_items` in SMAPI's console to delete items or objects from the game world. This may be useful for certain objects that players can't remove normally.

The command supports these formats:

Format | Description
-------|------------
`remove_items` | Removes an item directly in front of the player.
`remove_items <x> <y>` | Removes an item from the specified tile of the player's current location (a.k.a. map). Example: `remove_items 10 20`
`remove_items permanent` | Removes **all** items spawned by FTM with "CanBePickedUp" set to false, at the player's current location (a.k.a. map). Designed to clean up after FTM content pack bugs.


## Mod Settings
These settings are in the **config.json** file, which is in the mod's main folder: `Stardew Valley\Mods\FarmTypeManager`. They change the behavior of the mod itself, rather than a specific farm or content pack.

Name | Valid settings | Description
-----|----------------|------------
EnableConsoleCommands | **true**, false | Enables or disables this mod's commands in the SMAPI console. This may be helpful if other mods use similar command names.
EnableContentPacks | **true**, false | Enables or disables any installed content packs for FTM. If disabled, only "personal" content files in the `FarmTypeManager/data` folder will be used.
EnableTraceLogMessages | **true**, false | Enables or disables `[TRACE]`-level messages in the SMAPI error log.
EnableEPUDebugMessages | true, **false** | Enables or disables debug messages when Expanded Preconditions Utility (EPU) is used to check [preconditions](#extra-conditions).
MonsterLimitPerLocation | An integer (minimum 0), or **null** | If a single map already contains this number of monsters, the mod will skip spawning any additional monsters there. Use this setting if your content packs are spawning too many monsters, causing Stardew to run slowly.

## Spawn Settings
The settings below control how FTM spawns objects and monsters. The settings are found in each content pack's `content.json` file. Creating/loading a farm will also generate a "personal" content file for that farm in the `FarmTypeManager\data` folder. Personal content files are named after the farm's save data folder, such as **FarmerName_12345.json**.

Any text editor should be able to open these content files. However, you can also use the `ConfigEditor.html` file in the `FarmTypeManager` folder to edit these content files. The Config Editor is an editor that should work in any web browser and make it easier to understand each setting.

The mod will also generate a `default.json` file in the `FarmTypeManager\data` folder. Any newly generated files will copy its settings. This can be useful if you want to customize your own settings and create new farms frequently.

Deleting any file in the `FarmTypeManager\data` folder will allow it to be regenerated with default settings.

### Basic Settings
This section covers the simple on/off switches you can use to enable default configurations, which work similarly to "vanilla" Stardew Valley farm types.

Name | Valid settings | Description | Notes
-----|----------------|-------------|------
ForageSpawnEnabled | true, **false** | Enables or disables spawning forageable plants. | When set to **true** with other default settings, this will work similarly to the Forest Farm, randomly spawning forage items on the farm each day.
LargeObjectSpawnEnabled | true, **false** | Enables or disable spawning large objects (e.g. stumps). | When set to **true** with other default settings, this will find any existing large stumps on your farm and cause those to respawn each day.
OreSpawnEnabled | true, **false** | Enables or disables spawning ore. | When set to **true** with other default settings, this will work similarly to the Hill-top Farm, spawning various kinds of ore on any "Quarry" terrain your farm may have. (If you're not using the Hill-top Farm or a custom farm with similar-looking quarries, ore might not spawn. You'll need to change the **IncludeTerrainTypes** or **IncludeCoordinates** settings in the ore section.)
MonsterSpawnEnabled | true, **false** | Enables or disables spawning monsters. | When set to **true** with other default settings, this will work similarly to the Wilderness Farm, spawning various monsters on the farm at night when players are present. The monster types change at higher Combat skill levels.

The sections below cover the more advanced options for each spawn type. When the basic features above are enabled, the mod will use the Spawn Settings to determine **which** objects should be spawned, **how many** to spawn each day, and **where** to put them.

### General Spawn Settings
This section covers the general settings that appear in every spawner section.

Name | Valid settings | Description | Notes
-----|----------------|-------------|------
Areas | *(see Notes)* | A list of "spawn areas", which are groups of spawn settings for one "area" of a single map. | Each "area" describes a set of objects/monsters to spawn, exactly where they can spawn on a single map, and various optional spawning rules. To spawn things to multiple maps (or the same map with multiple "areas"), create copies of the entire bracketed section and separate them with commas: `"Areas": [ { Area 1 settings }, { Area 2 settings }, { Area 3 settings } ]`
UniqueAreaID | Any unique name, e.g. "Spawn area 1" | A unique nickname for this area. | This is used by the save system to record certain info for each area, and it appears in SMAPI's error log. If the same ID is used for multiple areas, the duplicates will be automatically renamed. Changing this later can cause minor problems with existing save data, e.g. it might reset LimitedNumberOfSpawns.
MapName | One or more map names, e.g. "Farm" | The name of the in-game location(s) where objects will spawn. | Go to an in-game location and use the `whereami` command in SMAPI's console to view its map name (see [Commands](#commands)). Multiple location names can be separated by commas, e.g. `"Farm, BusStop"`; a full set of spawns will appear in each location. Using a building name, e.g "Deluxe Barn", will target each copy of that building. Start a name with "prefix:", "suffix:", or "contains:" to match text in multiple location names; for example, "prefix:Farm" targets Farm, Farmhouse, FarmCave, etc.
MinimumSpawnsPerDay | An integer (less than or equal to the maximum) | The minimum number of objects to spawn each day. | If the spawn number is very high (e.g. 9999), objects will spawn until they run out of valid space. Numbers <= 0 are valid choices; negative numbers increase the chance of spawning 0 objects for the day.
MaximumSpawnsPerDay | An integer (greater than or equal to the minimum) | The maximum* number of objects to spawn each day. | If the spawn number is very high (e.g. 9999), objects will spawn until they run out of valid space. This maximum **can** be affected by other settings. e.g. "percent extra spawns per skill level" or "maximum simultaneous spawns". 
IncludeTerrainTypes | **"Diggable", "Grass"**, "Dirt", "Stone", "Wood", "All", "Quarry", "Custom" | A list of terrains where objects can spawn. | The "All" setting will let objects spawn on any open, valid tiles. Multiple terrain types can be included by separating them with commas: `"IncludeTerrainTypes": ["Grass", "Diggable"]`
ExcludeTerrainTypes | **"Diggable", "Grass"**, "Dirt", "Stone", "Wood", "All", "Quarry", "Custom" | A list of terrains where objects **cannot** spawn. |  See the notes for IncludeTerrainTypes to choose types. Any types covered by ExcludeTerrainTypes will **not** be used to spawn objects, **overriding** IncludeTerrainTypes and IncludeCoordinates.
IncludeCoordinates | `"x,y/x,y"` tile coordinates | A list of coordinates for areas where objects can spawn. | Use the `whereami` command (see [Commands](#commands)) to get a tile's coordinates. Any space between the two coordinates you use will be open for spawning. Separate multiple include areas with commas: `"IncludeCoordinates": ["0,0/100,100", "125,125/125,125"]`
ExcludeCoordinates | `"x,y/x,y"` tile coordinates | A list of coordinates for areas where objects **cannot** spawn. | See the notes for IncludeCoordinates to find coordinates. Any space covered by ExcludeCoordinates will **not** be used to spawn objects, **overriding** IncludeTerrainTypes and IncludeCoordinates.
StrictTileChecking | **"Maximum"**, "High", "Medium", "Low", "None" | How strictly the mod will validate included tiles. | Depending on the map's internal settings (especially in custom farms), Stardew might consider some tiles "invalid for object placement". If your other settings seem correct but nothing will spawn, try adjusting this setting. Note that "Low" and "None" might result in missing spawns: if a tile really *can't* have objects, the mod might still think it's valid. This might also cause spawning in water, cliffs, buildings, etc.
DaysUntilSpawnsExpire | **null** or an integer | The number of days spawns will exist before disappearing. | If set to null, spawned objects will behave normally. If set to a positive number, any spawns will only disappear after that number of days (or when removed by a player). If set to 0, spawns will never automatically disappear. Using this setting **will** protect forage from the game's weekly cleanup process.
CustomTileIndex | A list of integers | A list of index numbers from the game's tilesheet images, used by the "Custom" setting for IncludeTerrainTypes. | If the IncludeTerrainTypes setting above includes the "Custom" option, any tiles with these index numbers on the "Back" map layer will be valid spawn locations. You can find a tile's index number by standing on it and using the `whereami` command, or by using map/spritesheet modding tools.

#### Item Settings
This section describes the item formats used in the forage spawner's index lists (e.g. SpringItemIndex) and in MonsterTypes' optional "Loot" lists.

The lists can contain a mixture of object IDs (e.g. `206`), object names (e.g. `"pizza"`), or complex item definitions inside curly braces (e.g. `{"category": "object", "name": "pizza"}`).

For a more detailed description of the complex item settings, see the table below.

Name | Required | Valid settings | Description | Notes
-----|----------|----------------|-------------|------
Category | Yes | "Barrel", "Big Craftable", "Boots", "Breakable", "Buried", "Chest", "Crate", "DGA", "Fence", "Gate", "Furniture", "Hat", "Object", "Pants", "Ring", "Shirt", "Tool", "Weapon" | The category of the spawned item.| "Breakable" will randomly produce a barrel or crate. "Buried" will create an artifact spot with customizable "Contents".
Name | Yes | An item name or ID, e.g. `"Red Mushroom"` | The name or ID of the spawned item. | This setting is required **except** when the category is a container (e.g. "chest" or "breakable").
CanBePickedUp | No | **true**, false | When set to false, players will not be allowed to pick up this item. "True" has no effect. | Furniture and craftables set to false can be used, but not picked up. This setting has no effect on containers or monster loot. **Please use caution** with this setting. If necessary, players can use the `remove_items` command to override this.
Contents | No | A list of other items, e.g. `[16, "joja cola"]` | A list of items within this container. | This setting will be ignored by non-container item categories. It uses the same formatting as other item lists, so it can use complex item definitions as well.
PercentChanceToSpawn | No | An integer or decimal (minimum 0), e.g. `50` for a 50% chance | The percent chance of spawning this object. If the random chance fails, this item will not spawn. | This setting can be used for forage, loot, and the contents of containers.
Rotation | No | An integer (minimum/default 0) | The number of times to rotate a furniture item before spawning it. | This setting will be ignored by non-furniture items. The number of possible orientations is determined by each furniture item; 1, 2, and 4 are the most common.
SpawnWeight | No | An integer (minimum/default 1) | The weighted spawn chance of this forage type. | Increases the odds of spawning this forage type instead of others, similar to adding multiple copies of it to a forage list. Has no effect in "loot" or "contents" lists. Example: If this forage type's weight is 5 and another type's weight is 1, this type will spawn 5 times as often.
Stack | No | An integer (minimum 1) | The number of items to spawn as a single inventory "stack". | This setting should affect any categories capable of stacking. The maximum stack size varies for each item type; higher values will be reduced to the actual maximum.

Here is an example loot list that uses all three formats. It would cause a defeated monster to drop Wild Horseradish, Joja Cola, and a Galaxy Sword.
```
"Loot":
[
  16,
  "joja cola",
  {
    "category": "weapon",
    "name": "galaxy sword"
  }
],
```
Here is an example forage list that spawns a chest containing Wild Horseradish, Joja Cola, and a Galaxy Sword.
```
"SpringItemIndex":
[
  {
    "category": "chest",
    "contents":
    [
      16,
      "joja cola",
      {
        "category": "weapon",
        "name": "galaxy sword"
      }
    ]
  }
]
```
#### Spawn Timing Settings
This section is available for each spawn area and affects the time of day when objects will be spawned.

Name | Valid settings | Description | Notes
-----|----------------|-------------|------
StartTime | A Stardew time integer, e.g. **600** for 6:00AM or 2550 for 1:50AM | The earliest time objects can spawn. | Objects spawned by this area will be randomly assigned times between StartTime and EndTime.
EndTime | A Stardew time integer, e.g. **600** for 6:00AM or 2550 for 1:50AM | The latest time objects can spawn. | Objects spawned by this area will be randomly assigned times between StartTime and EndTime.
MinimumTimeBetweenSpawns | An integer (multiple of 10; minimum **10**) | The minimum number of in-game minutes between each spawn. | Randomly chosen spawn times will always be at least this many minutes apart. Example: If this value is `20` and some objects spawn at 6:10AM, nothing will spawn at 6:00 or 6:20.
MaximumSimultaneousSpawns | An integer (minimum 1), or **null** | The maximum number of objects this area will spawn at a specific time. | This **will** override the number of objects spawned per day if every spawn time has already reached this maximum.
OnlySpawnIfAPlayerIsPresent | true, **false** | Whether objects will spawn while no players are present at the in-game map. | If true and no players are present, any spawns assigned to the current time will be skipped; they will **not** be delayed until later.
SpawnSound | The name of a loaded sound, or blank: **""** | A Stardew sound effect that will play when this area spawns objects. | This setting is **case-sensitive** and uses the Sound Bank IDs available in [this modding spreadsheet](https://docs.google.com/spreadsheets/d/1CpDrw23peQiq-C7F2FjYOMePaYe0Rc9BwQsj3h6sjyo).

#### Extra Conditions
Name | Valid settings | Description | Notes
-----|----------------|-------------|------
ExtraConditions | *(see Notes)* | A set of optional conditions required to spawn objects in an area. | These can be used to restrict spawning to certain times or weather conditions. Each setting will be ignored if it is set to its default, i.e. `[]` or `null`.
Years | A list of integers, ranges, or "Any"/"All" | A list of years in which things are allowed to spawn. | Years should be inside the brackets with quotation marks, separated by commas if multiple are listed. They can be listed as single years, ranges, or with a **+** to include any following years. See these examples: `["1"]` `["2-4"]` `["1", "3+"]`
Seasons | A list of season names: "Spring", "Summer", "Fall", "Winter", or "Any"/"All" | A list of seasons in which things are allowed to spawn. | Seasons should be inside the brackets with quotation marks, separated by commas if multiple are listed. See these examples: `["Spring"]`, `["Summer", "Winter"]`
Days | A list of integers, ranges, or "Any"/"All" | A list of days on which things are allowed to spawn. | Days should be inside the brackets with quotation marks, separated by commas if multiple are listed. They can be listed as single days, ranges, or with a **+** to include any following days. See these examples: `["1"]` `["2-14"]` `["1", "8+"]`
WeatherYesterday | A list of weather names: "Sun", "Wind", "Rain", "Storm", "Snow", or "Any"/"All" | Things will be allowed to spawn if yesterday's weather matched a name from this list. | Weather names should be inside the brackets with quotation marks, separated by commas if multiple are listed. Note that windy days *do not* count as sunny, and storms *do not* count as rain; remember to include both if needed. See these examples: `["Snow"]`, `["Sun", "Wind"]`, `["Rain", "Storm", "Snow"]`
WeatherToday | A list of weather names: "Sun", "Wind", "Rain", "Storm", "Snow", or "Any"/"All" | Things will be allowed to spawn if today's weather matches a name from this list. | Weather names should be inside the brackets with quotation marks, separated by commas if multiple are listed. Note that windy days *do not* count as sunny, and storms *do not* count as rain; remember to include both if needed. See these examples: `["Snow"]`, `["Sun", "Wind"]`, `["Rain", "Storm", "Snow"]`
WeatherTomorrow | A list of weather names: "Sun", "Wind", "Rain", "Storm", "Snow", or "Any"/"All" | Things will be allowed to spawn if tomorrow's weather forecast matches a name from this list. | Weather names should be inside the brackets with quotation marks, separated by commas if multiple are listed. Note that windy days *do not* count as sunny, and storms *do not* count as rain; remember to include both if needed. See these examples: `["Snow"]`, `["Sun", "Wind"]`, `["Rain", "Storm", "Snow"]`
GameStateQueries | A list of [game state queries](https://stardewvalleywiki.com/Modding:Game_state_queries) (GSQs) | Things will be allowed to spawn if **any** of the listed strings are true at the start of the day. | See the wiki's article on [game state queries](https://stardewvalleywiki.com/Modding:Game_state_queries) for usage information. Format example: `"GameStateQueries": ["PLAYER_HAS_FLAG Any 1234", "PLAYER_HAS_FLAG Any 5678"]`
CPConditions | A set of [Content Patcher](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/README.md#for-mod-authors) "When" conditions | Things will be allowed to spawn if **all** of the provided conditions are true at the start of the day. | See [Content Patcher's mod author guides](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/README.md#for-mod-authors) for usage information. Format example: `"CPConditions": {"HasFlag": "beenToWoods", "FarmCave": "Mushrooms"}`
EPUPreconditions | A list of [EPU preconditions](https://github.com/ChroniclerCherry/stardew-valley-mods/tree/master/ExpandedPreconditionsUtility) | Things will be allowed to spawn if **any** of the listed strings are true at the start of the day. | This feature requires [Expanded Preconditions Utility](https://www.nexusmods.com/stardewvalley/mods/6529). See [EPU's readme](https://github.com/ChroniclerCherry/stardew-valley-mods/tree/master/ExpandedPreconditionsUtility) for usage information. In multiplayer, only the host's info will be checked. The conditions `t <mintime> <maxtime>` and `x <letter ID>` **should not be used** and are not supported.
LimitedNumberOfSpawns | An integer | The number of times this area will spawn things before stopping. | At the end of each day, if things spawned here without being prevented by other "extra conditions", this number will count down by 1 (record in the separate **.save** file). Once it reaches zero, the area will stop spawning things. Note that unlike other ExtraConditions settings, this does not need to be in brackets or quotations. Example: `1`


### Forage Spawn Settings
Name | Valid settings | Description | Notes
-----|----------------|-------------|------
SpringItemIndex (Area) | **null**, *(see Notes)* | The index numbers and/or item names for forage items to spawn in spring *in this area*. | This setting is per-area and will *override* the global SpringItemIndex setting below. Leave this set to *null* unless you want different spring forage items in a specific area. Fill this in by copying the format of the global version below.
SummerItemIndex (Area)| **null**, *(see Notes)* | The index numbers and/or item names for forage items to spawn in summer *in this area*. | This setting is per-area and will *override* the global SummerItemIndex setting below. Leave this set to *null* unless you want different summer forage items in a specific area. Fill this in by copying the format of the global version below.
FallItemIndex (Area) | **null**, *(see Notes)* | The index numbers and/or item names for forage items to spawn in fall *in this area*. | This setting is per-area and will *override* the global FallItemIndex setting below. Leave this set to *null* unless you want different fall forage items in a specific area. Fill this in by copying the format of the global version below.
WinterItemIndex (Area) | **null**, *(see Notes)* | The index numbers and/or item names for forage items to spawn in winter *in this area*. | This setting is per-area and will *override* the global WinterItemIndex setting below. Leave this set to *null* unless you want different winter forage items in a specific area. Fill this in by copying the format of the global version below.
PercentExtraSpawnsPerForagingLevel | Any integer (default **0**) | The % of extra forage spawned for each level of Foraging skill. | In multiplayer, this is based on the highest skill level among *all* players (even if they're offline). For example, setting this to 10 will spawn +10% forage items per level; if a farmhand has the best Foraging skill of level 8, there will be 80% more forage each day.
SpringItemIndex (Global) | A list of integers and/or item names, e.g. `[16, "Red Mushroom"]` | The index numbers and/or item names for forage items to spawn in spring. | By default, these are the forage items normally spawned on the Forest Farm during spring. For formatting information, see the [Item Settings](#item-settings) section.
SummerItemIndex (Global) | A list of integers and/or item names, e.g. `[16, "Red Mushroom"]` | The index numbers for forage items to spawn in summer. | By default, these are the forage items normally spawned on the Forest Farm during summer. For formatting information, see the [Item Settings](#item-settings) section.
FallItemIndex (Global) | A list of integers and/or item names, e.g. `[16, "Red Mushroom"]` | The index numbers for forage items to spawn in fall. | By default, these are the forage items normally spawned on the Forest Farm during fall. For formatting information, see the [Item Settings](#item-settings) section.
WinterItemIndex (Global) | A list of integers and/or item names, e.g. `[16, "Red Mushroom"]` | The index numbers for forage items to spawn in winter. | By default, this is empty because forage doesn't normally spawn during winter. To add winter forage, use the same format as the other ItemIndex settings above. For formatting information, see the [Item Settings](#item-settings) section.

### Large Object Spawn Settings
Name | Valid settings | Description | Notes
-----|----------------|-------------|------
ObjectTypes | "Stump", "Log", "Boulder", "Quarry Boulder", "Meteorite", "Mine Rock 1", "Mine Rock 2", "Mine Rock 3", "Mine Rock 4", "Weed 1", "Weed 2", a giant crop ID, or an Item Extensions clump ID | A list of object types to spawn. | Objects spawned in this area will be chosen randomly from this list. Adding the same object type multiple times will increase its chances. Separate multiple objects with commas: `"ObjectTypes: [ "Stump", "Log", "Meteorite" ]`
FindExistingObjectLocations | true, **false** | Finds any existing objects listed in ObjectTypes and adds them to the IncludeCoordinates list. | This can be used to automatically find existing objects' coordinates and respawn them each day. It will set itself to "false" in your settings file after completion.
PercentExtraSpawnsPerSkillLevel | Any integer (default **0**) | The % of extra objects spawned for each level of the RelatedSkill. | In multiplayer, this is based on the highest skill level among *all* players (even if they're offline). For example, setting this to 10 will spawn +10% items per skill level; if a farmhand has the best skill of level 8, there will be 80% more objects each day.
RelatedSkill | "Farming", "Fishing", "Foraging", "Mining", "Combat" | The skill used by PercentExtraSpawnsPerSkillLevel to spawn extra objects.

### Ore Spawn Settings
Name | Valid settings | Description | Notes
-----|----------------|-------------|------
MiningLevelRequired (Area) | **null**, *(see Notes)* | The minimum Mining skill level needed to spawn each ore type *in this area*. | This setting is per-area and will *override* the global MiningLevelRequired setting below. Leave this set to *null* unless you want different level requirements in a specific area. Fill this in by copying the format of the global version below.
StartingSpawnChance (Area) | **null**, *(see Notes)* | Each ore type’s chance of spawning with the minimum required Mining skill *in this area*. | This setting is per-area and will *override* the global StartingSpawnChance setting below. Leave this set to *null* unless you want different chances in a specific area. Fill this in by copying the format of the global version below.
LevelTenSpawnChance (Area) | **null**, *(see Notes)* | Each ore type’s chance of spawning with level 10 Mining skill *in this area*. | This setting is per-area and will *override* the global LevelTenSpawnChance setting below. Leave this set to *null* unless you want different chances in a specific area. Fill this in by copying the format of the global version below.
PercentExtraSpawnsPerMiningLevel | An integer (default **0**) | The % of extra ore spawned for each level of Mining skill. | In multiplayer, this is based on the highest skill level among *all* players (even if they're offline). For example, setting this to 10 will spawn +10% ore per Mining level; if a farmhand has the best Mining skill of level 8, there will be 80% more ore each day.
MiningLevelRequired (Global) | 0-10 | The minimum Mining skill level needed to spawn each ore type. | An ore type won't start spawning until *any* player (even offline farmhands) has the listed Mining skill. 
StartingSpawnChance (Global) | 0 or more | Each ore type's chance of spawning with the minimum required Mining skill. | These numbers are weighted chances; they don't need to add up to 100. The defaults are roughly based on the native game's spawn chances with slight increases.
LevelTenSpawnChance (Global) | 0 or more | Each ore type's chance of spawning with level 10 Mining skill. | Chances will drift gradually from StartingSpawnChance to LevelTenSpawnChance. For example, in the default settings, frozen geodes' chances of spawning from level 5 to 10 are `4, 4, 3, 3, 2, 2`.

### Monster Spawn Settings
Name | Valid settings | Description | Notes
-----|----------------|-------------|------
MonsterTypes | A list of "monster type" sections *(see Notes)* | A list of monster types to spawn, containing a name and list of optional settings. | Separate each monster type with commas: `"MonsterTypes": [ { "MonsterName": "bat", "Settings": {} }, { "MonsterName": "ghost", "Settings": {"HP": 1, "Damage": 0} } ]`
MonsterName | The name of an in-game monster, e.g. `"green slime"` | The "base" monster used by a Monster Type. | Spawned monsters use existing monster classes, but can be individually customized by the optional "Settings" list below. To find a monster name, use the [list_monsters](#list_monsters) command. 
Settings | A list of setting names and values, e.g. `"HP": 1` | A list of optional customization settings to apply to a Monster Type. | See the Monster Type Settings section below for more information about each setting. Separate each setting with commas: `"Settings": {"HP": 999, "Sprite":"Characters/Monsters/Skeleton"}`

#### Monster Type Settings
Name | Valid settings | Description | Notes
-----|----------------|-------------|------
SpawnWeight | An integer (minimum/default 1) | The weighted spawn chance of this monster type. | Increases the odds of spawning this monster type instead of others, similar to adding multiple copies of it to the list. Example: If this monster type's weight is 5 and another type's weight is 1, this type will spawn 5 times as often.
HP | An integer (minimum 1) | The monster's maximum health. | 
CurrentHP | An integer (minimum 1) | The monster's current (not maximum) health at spawn. | This is mainly useful for "themed" monsters to spawn with injuries, or monsters capable of healing themselves.
PersistentHP | true, **false** | Whether the monster will keep any HP damage overnight. | This only applies to monsters with `DaysUntilSpawnsExpire` settings. This is intended for unique monsters that don't respawn when defeated, and allows players to defeat them gradually over multiple days.
Damage | An integer (minimum 0) | The amount of damage the monster's attacks deal. | Some monster types/attacks ignore this setting and use hard-coded damage values, e.g. skeleton bone-throwing attacks.
Defense | An integer (minimum 0) | Attacks that hit the monster are reduced by this much damage. | 
DodgeChance | An integer (minimum 0) | The percent chance the monster will completely ignore each attack. | 
EXP | An integer (minimum 0) | Defeating the monster will give players this amount of Combat skill experience. | Even with this setting, Stardew does **not** give players experience for defeating monsters at the farm.
Loot | A list of integers and/or item names, e.g. `[16, "Red Mushroom"]` | A list of items the monster will drop when defeated. | By default, loot is **not** randomized; use multiple monster types or items with "PercentChanceToSpawn" settings to randomize loot. An empty list will cause the monster to drop nothing. For formatting information, see the [Item Settings](#item-settings) section.
ExtraLoot | **true**, false | If false, the monster will no longer drop certain type-specific items when defeated. | This can be used to disable loot that otherwise ignores the "Loot" setting. Examples include Slime color-specific items, Pepper Rex eggs and bones, etc.
SightRange | An integer | The tile distance (in a square pattern) at which this monster can see players and become aggressive. | Use -2 or lower to disable aggression completely. -1 might cause automatic aggression, similar to "SeesPlayersAtSpawn".
SeesPlayersAtSpawn | true, **false** | If true, the monster will always be aware of players, regardless of distance. | Slimes with this setting will have red eyes and behave aggressively.
RangedAttacks | **true**, false | If false, the monster will not use any ranged attacks. | This setting may not affect new monster types added by other mods.
InstantKillImmunity | true, **false** | If true, the monster will be immune to certain effects that instantly defeat monsters. | Currently, this prevents the monster being eaten by the Frog Egg trinket.
StunImmunity | true, **false** | If true, the monster will be immune to certain effects that stun monsters. | Currently, this prevents the monster being frozen by the Ice Rod trinket.
FacingDirection | "up", "down", "left", "right" | The direction the monster is facing when it spawns. | Spikers will attack in the chosen direction (or a single random direction if this setting is not used).
Segments | An integer (minimum 0) | The number of extra body parts this monster will have (if applicable). | Slimes will have this number of extra slimes "stacked" on top of them. Royal Serpents will have this number of extra tail segments.
Sprite | The "address" of a loaded asset | A loaded spritesheet to replace this monster's default sprite. | These can be default assests in Stardew or those loaded by a mod like Content Patcher. Examples: `"Characters/Monsters/Skeleton"` or `"Animals/horse"`
Color | A string of RGB or RGBA values, e.g. `"255 0 0"` | The monster's color and transparency level. | This setting overrides MinColor and Maxcolor. It currently only applies to slimes, big slimes, and metal heads. Values can range from 0 to 255 and optionally include alpha transparency, e.g.: `"0 0 0"` or `"0 0 0 127"`
MinColor | A string of RGB or RGBA values, e.g. `"0 0 0"` | The minimum color and transparency randomly applied to this monster. | This setting will be ignored unless MaxColor is also provided. See `Color` above for formatting.
MaxColor |  A string of RGB or RGBA values, e.g. `"255 255 255"` | The maximum color and transparency randomly applied to this monster. | This setting will be ignored unless MinColor is also provided. See `Color` above for formatting.
Gender | A string, e.g. "M" or "F" (random by default) | The monster's gender. | Only affects small slimes. Male slimes ("M") will spawn with an antenna. Female slimes ("F") will not.
RelatedSkill | "Farming", "Fishing", "Foraging", "Mining", "Combat" | The player skill that affects the "Skill Level" settings below. | If this setting isn't provided, the "Skill Level" settings below will be ignored.  In multiplayer, these settings check the highest skill level among **all** players.
MinimumSkillLevel | An integer (minimum 0) | The minimum skill level required to spawn this monster type. | This is based on the RelatedSkill setting.
MaximumSkillLevel | An integer (minimum 0) | The maximum skill level allowed to spawn this monster type. | This is based on the RelatedSkill setting.
PercentExtraHPPerSkillLevel | An integer | The monster's HP is increased by this percentage, once for each skill level. | This is based on the RelatedSkill setting. Negative values are valid and will decrease instead.
PercentExtraDamagePerSkillLevel | An integer | The monster's damage is increased by this percentage, once for each skill level. | This is based on the RelatedSkill setting. Negative values are valid and will decrease instead.
PercentExtraDefensePerSkillLevel | An integer | The monster's defense is increased by this percentage, once for each skill level. | This is based on the RelatedSkill setting. Negative values are valid and will decrease instead.
PercentExtraDodgeChancePerSkillLevel | An integer | The monster's dodge chance is increased by this percentage, once for each skill level. | This is based on the RelatedSkill setting. Negative values are valid and will decrease instead.
PercentExtraEXPPerSkillLevel | An integer | The monster's EXP is increased by this percentage, once for each skill level. | This is based on the RelatedSkill setting. Negative values are valid and will decrease instead.

### Other Settings
Name | Valid settings | Description | Notes
-----|----------------|-------------|------
QuarryTileIndex | Integer(s) | A list of index numbers for "quarry" tiles in the game's tilesheet images. | These have been manually chosen to match the "quarry" tiles in the game's Hill-top Farm and custom maps that use similar mining areas. They're provided here so that people familiar with editing Stardew maps can customize this mod's "Quarry" terrain setting.

### File Conditions
Name | Valid settings | Description | Notes
-----|----------------|-------------|------
File_Conditions | *(see below)* | A list of conditions required to use this content file with a specific farm. | These settings are primarily useful for content packs.
FarmTypes | A list of: "Standard", "Riverland", "Forest", "Hilltop", "Wilderness", "FourCorners", "Beach", and/or custom farm type IDs | A list of farm types for which this content file will be used. | This is mainly for content packs that are combined with custom farm maps. For example, if a custom farm replaces your Standard farm type, set this to `["Standard"]`. For custom farm types added to `Data/AdditionalFarms`, use the farm type's "ID" from that, e.g. `["MeadowlandsFarm"]`.
FarmerNames | A list of farmer names, e.g. `["Esca"]` | If one of this list's names matches the farmer's name, this content file will be used. | This setting only checks the main farmer's name; it won't be affected by multiplayer farmhands.
SaveFileNames | A list of save file names, e.g. `["Esca_1234567"]` | If one of these names matches the current farm's save file name, this content file will be used. | A niche setting for when other conditions aren't effective at choosing a farm. Note that this technically checks the save *folder* name, not the save file itself.
OtherMods | A list of mods' UniqueIDs and `true` or `false` (see Notes) | If all of the player's loaded mods match this list, this content file will be used. | This can be used to make a content pack or content file only activate with certain mod requirements. `true` means a mod *must* be installed, while `false` means it *can't* be installed. Example: `OtherMods: { "Esca.FarmTypeManager" : true, "FakeModID" : false }`

## Content Packs
Modders can use FTM by creating content packs. Note that these packs will be used *in addition* to any farm-specific files in the `FarmTypeManager\data` folder; they will not replace or override each other.

To create a content pack for FTM:

1. Create a new folder in the `Stardew Valley\Mods` folder. Its name should follow this format: `[FTM] Your Pack Name`
2. Create a text file in the new folder called **manifest.json**. (See the [Manifest](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Manifest) wiki article for more info.) Copy the format below and fill in the settings appropriately:
```
{
  "Name": "Your Pack Name",
  "Author": "Your Name",
  "Version": "1.0.0",
  "Description": "Your description here. Briefly explain what the content pack does.",
  "UniqueID": "YourName.YourPackName",
  "MinimumApiVersion": "4.0.0",
  "UpdateKeys": [],
  "ContentPackFor": {
    "UniqueID": "Esca.FarmTypeManager",
    "MinimumVersion": "1.23.0"
  }
}
```
3. Create or copy a FTM configuration file into the `[FTM] Your Pack Name` folder and name it **content.json**. The format is exactly the same as a farm content file from the `FarmTypeManager\data` folder.
4. If you want to combine this content pack with a custom farm map or similar mod, consider editing the **FarmTypes** and/or **OtherMods** settings under **File_Conditions** at the bottom of the content file. (See the [File Conditions](#file-conditions) section.)

## Additional Modding Features

### For SMAPI Mods

#### FTM API

FTM provides a small API for other SMAPI mods. To access it in your C# mod:

1. Copy the [IFtmApi.cs](https://github.com/Esca-MMC/FarmTypeManager/blob/master/FarmTypeManager/API/IFtmApi.cs) file into your mod.
2. After all mods are loaded, e.g. during a GameLaunched event, use SMAPI's Helper to get an API instance: `var api = Helper.ModRegistry.GetApi<FarmTypeManager.IFtmApi>("Esca.FarmTypeManager");`

Feel free to request additional features, but note that there are currently no plans for on-demand spawning through the API.

#### Custom Monster Classes

The MonsterName setting in FTM's monster spawn settings can use custom monster classes created by other mods. This process requires some knowledge of C# and SMAPI; you may also need to decompile and explore Stardew Valley's monster code.

Creating a custom class works as follows:

1. Create a basic SMAPI mod. See the wiki's [Modder Guide](https://stardewvalleywiki.com/Modding:Modder_Guide) for more information. The mod doesn't need to perform any specific actions; it just needs to exist and be loaded by SMAPI.
2. Within that mod (in any namespace or class), create a subclass of the StardewValley.Monster class or one of its existing subclasses (e.g. Ghost).
3. Create a default constructor (no parameters) and a constructor with only a Vector2 parameter. The default is required for some of Stardew's internal behavior, while the Vector2 constructor is used by FTM.
4. Customize the monster as needed. Override virtual methods to change the monster's "base" behavior, and if the monster needs any new properties, remember to add them to the `NetFields` list and use ISerializable types.
5. Once the mod is complete, you should be able to use the [list_monsters](#list_monsters) command to find your custom monster's full name. Use that name with FTM's MonsterName setting to spawn your monster.

An example project will likely be added in a future update. For now, please see FTM's [custom monster classes](https://github.com/Esca-MMC/FarmTypeManager/tree/master/FarmTypeManager/Classes/In-Game/Monsters) for examples of formatting, and possibly a few bugfixes for certain classes.

### For Content Packs

#### Content Patcher Tokens

FTM adds the following custom tokens to Content Patcher, which can be used by content packs for that mod.

To enable them in Content Patcher packs, you must do at least one of the following:

A) Add FTM as a dependency in your pack's "manifest.json" file: `"Dependencies": [{"UniqueID": "Esca.FarmTypeManager"}]`

B) Whenever you use a token from FTM, add this "When" condition: `"HasMod": "Esca.FarmTypeManager"`

Format | Description
-------|------------
Esca.FarmTypeManager/NumberOfMonsters [location] | This token outputs the current number of monsters at a location. If no location name is provided, it will use the local player's current location. Use the `whereami` console command to see your current location's name.<br/><br/>If a location does not exist, is not loaded, or currently isn't being shared with the local player in multiplayer, this token will output `-1` instead.<br/><br/>Note: This token should NOT be used in FTM's CPConditions field; it cannot detect FTM monsters at the start of the day, which is when that field is used. Future versions of FTM are planned to add more support for this.

#### Game State Queries

FTM adds the following [game state queries](https://stardewvalleywiki.com/Modding:Game_state_queries) (GSQs) to the GSQ system.

They can be used in FTM's GameStateQueries field, in the "Condition" fields of various Stardew data assets, or by other mods.

Format | Description
-------|------------
Esca.FarmTypeManager_LOCATION_EXISTS &lt;location&gt; | This query is true if the named location is currently loaded. This should include custom locations, all mine levels visited by any player today, etc.
Esca.FarmTypeManager_LOCATION_IS_ACTIVE &lt;location&gt; | This query is true if the named location is currently loaded and being shared with the local player. For the host player, this include all loaded locations; for multiplayer farmhands, this only includes the local player's current location and a few that are always active.
Esca.FarmTypeManager_NUMBER_OF_MONSTERS &lt;location&gt; &lt;minimum&gt; [maximum] | This query is true if the named location currently has the specified number of monsters (from the minimum to the optional maximum, inclusive). It also supports the "Here" and "Target" [location arguments](https://stardewvalleywiki.com/Modding:Game_state_queries#Target_location).<br/><br/>Note: This will throw an error if the location is not loaded, or indicate "-1" monsters if the location is not active. It's strongly recommended to use the "location is active" query before this one.<br/><br/>This token should NOT be used in FTM's GameStateQueries field; it cannot detect FTM monsters at the start of the day, which is when that field is used. Future versions of FTM are planned to add more support for this.