# Farm Type Manager (FTM)
顾名思义，Farm Type Manager（FTM）是一个允许其他模组在游戏中的任何地方生成各种物品和怪物的前置框架模组。它还可以以独特的方式自定义物品和怪物。可以方便地让模组作者配置或随机化生成的物品数量、生成地点和生成时间等规则。

> **注**：下文内容中提到的所有“数据包”，均指代其他依赖 FTM 的模组。

## 目录
* [安装](#安装)
* [示例](#示例)
* [指令](#指令)
    * [whereami](#whereami)
    * [list_monsters](#list_monsters)
    * [remove_items](#remove_items)
* [模组配置](#模组配置)
* [生成设置](#生成设置)
    * [基础设置](#基础设置)
    * [通用生成设置](#通用生成设置)
        * [物品设置](#物品设置)
        * [生成时间设置](#生成时间设置)
        * [额外条件设置](#额外条件设置)
    * [采集品生成设置](#采集品生成设置)
    * [大型地物生成设置](#大型地物生成设置)
    * [采矿点生成设置](#采矿点生成设置)
    * [怪物生成设置](#怪物生成设置)
        * [怪物类型设置](#怪物类型设置)
    * [其它设置](#其它设置)
    * [文件条件设置](#文件条件设置)
* [数据包](#数据包)
* [其它模组功能](#其它模组功能)
    * [面向 SMAPI 模组](#面向-smapi-模组)
        * [FTM API](#ftm-api)
        * [自定义怪物类](#自定义怪物类)
    * [面向数据包](#面向数据包)
        * [Content Patcher Tokens](#content-patcher-tokens)
        * [Game State Queries](#game-state-queries)

## 安装
1. **安装最新版本的 [SMAPI](https://smapi.io/)；**
2. 从 [GitHub Release 页面](https://github.com/Esca-MMC/FarmTypeManager/releases)、[Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/3231/)或[ModDrop](https://www.moddrop.com/stardew-valley/mods/598755-farm-type-manager-ftm)处**下载 FarmTypeManager**；
3. 将 **FarmTypeManager 解压至**您的 `Stardew Valley\Mods` 文件夹内。

如果您正在使用需要 FTM 作为前置的其他模组，那么恭喜您完成了全部的步骤！

## 示例
以下内容主要面向数据包作者，此处提供了一些基础的更改示例，例如在农场或其他游戏地图上生成各种物品。

### 在农场中生成采集品和大树桩
```js
"ForageSpawnEnabled": true,         // 启用采集品生成
"LargeObjectSpawnEnabled": true,    // 启用大型地物生成
```
![Your farm's stumps will respawn every day.](images/ftm_example_1.png)

如此设置以后，您的农场上将每天生成出新的大树桩和采集品。

### 在农场随机生成新的大圆木和圆石
```js
"LargeObjectSpawnEnabled": true,    // 启用大型地物生成
```
```js
"Large_Object_Spawn_Settings": {
  "Areas": [
    {
      "ObjectTypes": [
        "Log", "Boulder"          // 设定生成物为大圆木和圆石
      ],
      "FindExistingObjectLocations": false,
      "MapName": "Farm",          // 设置目标地图名为“Farm”（农场）
      "MinimumSpawnsPerDay": 0,   // 每日生成数量下限
      "MaximumSpawnsPerDay": 2,   // 每日生成数量上限
      "IncludeTerrainTypes": ["Grass", "Dirt", "Diggable"],
      ...
    }
  ]
}
```
![Most days, logs and/or boulders will spawn on the farm.](images/ftm_example_2.png)
如此设置以后，每天您的农场上都有较高概率生成出新的大树桩和圆石。

### 在煤矿森林的指定区域内生成采矿点
```js
"OreSpawnEnabled": true,    // 启用采矿点生成
```
```js
"Ore_Spawn_Settings": {
  "Areas": [
    {
      "MapName": "Forest",        // 设置目标地图名为“Forest”（煤矿森林）
      "MinimumSpawnsPerDay": 1,   // 每日生成数量下限
      "MaximumSpawnsPerDay": 5,   // 每日生成数量上限
      "IncludeCoordinates": 
          [ "65,22/74,27" ],      // 在此坐标范围生成
      ...
    }
  ]
}
```
![Ore will spawn in a specific area of the Forest map.](images/ftm_example_3.png)
如此设置以后，煤矿森林里您指定的区域内将每天生成采矿点。

### 在农场上生成大量采集品，且不在农舍附近生成
```js
"ForageSpawnEnabled": true, // 启用采集品生成
```
```js
"Forage_Spawn_Settings": {
  "Areas": [
    {
      "MapName": "Farm",                  // 设置目标地图名为“Farm”（农场）
      "MinimumSpawnsPerDay": 9999,        // 每日生成数量下限
      "MaximumSpawnsPerDay": 9999,        // 每日生成数量上限
      "IncludeTerrainTypes": [ "All" ],
      "ExcludeCoordinates": 
          [ "69,17;57,10" ]               // 不在此坐标范围内生成
      ...
    }
  ]
}
```
![Forage will spawn everywhere on the Farm map, except around the house.](images/ftm_example_4.png)
如此设置以后，您的农场上将会长满采集品，且不会在农舍附近生成。

### 在由[模组支持的顶峰地图上](https://www.nexusmods.com/stardewvalley/mods/2073)生成由[模组添加的植物](https://www.nexusmods.com/stardewvalley/mods/1598)，并设置条件为仅在第二年开始的雨天后生成。
```
"ForageSpawnEnabled": true,     // 启用采集品生成
```
```js
"Forage_Spawn_Settings": {
  "Areas": [
    {
      "SpringItemIndex": [ "Mint", "Juniper" ],   // 春季时生成的物品名称
      "SummerItemIndex": [ "Mint", "Juniper" ],   // 夏季时生成的物品名称
      "FallItemIndex": [ "Mint", "Juniper" ],     // 秋季时生成的物品名称
      "WinterItemIndex": [],
      "MapName": "Summit",
      "MinimumSpawnsPerDay": 4,
      "MaximumSpawnsPerDay": 8,
      "IncludeTerrainTypes": [
        "All"
      ],
      // 额外条件
      "ExtraConditions": {
        "Years": [ "2+" ],                        // 仅从第二年起生成
        "WeatherYesterday": [ "Rain", "Storm" ],  // 仅在雨天或雷暴天气的后一天生成
              ...
      }
    }
  ]
}
```
![Custom forage spawned on a mod-enabled map with specific time and weather conditions.](images/ftm_example_5.png)
如此设置以后，在模组支持的顶峰地图上将会在特定的时间和天气条件下生成特定的物品。

## 指令
此模组向 SMAPI 的控制台添加了以下命令。

调用这些命令需要 Console Commands 模组，该模组由 SMAPI 自动安装。如果需要，它们也可以在 FTM 的 config.json 文件中禁用。参见[模组配置]章节(#模组配置)。

### whereami

在 SMAPI 控制台中输入 `whereami` 即可显示当前地图的信息，包括：

* 地图的名称（例如：“Farm”或“BusStop”）
* 您当前所在地块的 X/Y 坐标
* 地块的的地形类型（例如：“Dirt”或“Stone”）
* 该地块是否可用工具“挖掘”
* 该地块的图像索引编号（用于识别“Quarry”地块或设置自定义地块列表）

### list_monsters

在 SMAPI 控制台中输入 `list_monsters` 即可显示所有可用怪物名称列表，用于 MonsterName 生成设置。

该命令将首先列出游戏内每种怪物的主要名称，然后扫描其他模组以查找自定义的怪物类。

MonsterName 设置中应使用完整的名称。例如：`"MyModName.CustomMonster"`

### remove_items

在 SMAPI 控制台中输入 `remove_items` 以从游戏世界中删除物品或对象。这对于某些无法通过正常手段移除的物品非常有用。

该命令支持以下格式：

格式 | 描述
-------|------------
`remove_items` | 移除玩家面朝着的物品。
`remove_items <x> <y>` | 移除玩家当前所在位置（即当前地图）指定坐标下物品。例如：`remove_items 10 20`。
`remove_items permanent` | 移除玩家当前所在位置（即当前地图）下所有由 FTM 生成且“CanBePickedUp”字段为 false 的物品。该命令设计为用于清理其它使用 FTM 的数据包所产生的 bug。

## 模组配置

这些设置位于模组主文件夹中的 **config.json** 文件中，路径为 `Stardew Valley\Mods\FarmTypeManager`。这些选项仅改变 FTM 模组本身的行为，不会影响其他任何数据包。

名称  | 有效值 | 描述
-----|----------------|------------
启用控制台命令 | **true**、false | 是否在 SMAPI 控制台中启用由此模组添加的命令。若其他模组使用相同的命令时，这或许会有所帮助。
启用数据包加载 | **true**、false | 是否加载其它依赖 FTM 的模组。若禁用该项，那么模组将仅使用 FarmTypeManager/data 文件下的缓存数据。
启用日志跟踪信息 | **true**、false | 是否启用 SMAPI 错误日志中的 `[TRACE]` 级别消息。
启用 EPU 调试信息| true、**false** | 是否显示扩展前置条件工具（EPU）检查[额外条件](#额外条件)时生成的调试消息。
怪物生成上限 | 一个整数（最小值为 0）或 **null** | 若某张地图上的怪物数量达到了特定值后，本模组将停止在该地图上生成怪物。当您的数据包生成了过多怪物，导致游戏卡顿、运行缓慢时，请使用此设置。

## 生成设置
以下设置控制了 FTM 如何生成物品和怪物（下统称为“对象”）。这些设置可以在每个数据包中的 `content.json` 文件中找到。当创建或加载存档时，也会在本模组目录下的 `FarmTypeManager\data` 文件夹中为该存档生成一个数据缓存文件。数据缓存文件的命名方式与农场的存档数据文件夹相同，例如 `农场名称_12345.json`。

您可以使用任何文本编辑器来打开这些数据文件。当然，您也可以使用位于 FarmTypeManager 文件夹中的 [ConfigEditor.html](../FarmTypeManager/ConfigEditor.html) 文件来编辑这些数据文件。ConfigEditor 是一个可以使用任意的浏览器打开的编辑器，它能够使您更好地阅读并设定各配置项。

本模组还会在 `FarmTypeManager\data`文件夹中生成一个 `default.json` 文件。任何新生成的文件都会复制其配置。这在您想要自定义自己的设置并频繁地创建新存档时非常有用。

删除 `FarmTypeManager\data` 文件夹中的任意文件将会令其使用默认配置重新生成数据。

### 基础设置
本部分介绍了四种基础生成规则的配置选项，启用对应的配置后，将模拟游戏原版农场的对象生成机制来生成相应的对象。

名称 | 有效值 | 描述 | 备注
----|-------|------|---- 
ForageSpawnEnables | true, **false** | 是否启用采集品生成。 | 当设置为 **true** 并使用其他默认设置时，将会像类似森林农场那样，每天随机地在农场上生成采集品。
LargeObjectSpawnEnabled | true, **false** | 是否启用大型地物生成（例如大树桩）。 | 当设置为 **true** 并使用其他默认设置时，将会寻找您农场现有所有大树桩，并使其每天重新生成。
OreSpawnEnabled | true, **false** | 是否启用采矿点生成。 | 当设置为 **true** 并使用其他默认设置时，将会像类似山顶农场那样，在您农场的“采石场”地形上生成各种采矿点。（如果您没有使用山顶农场或类似的自定义农场，那么采矿点可能不会生成。在这种情况下，您需要更改 **IncludeTerrainTypes** 或 **IncludeCoordinates** 设置。）
MonsterSpawnEnabled | true, **false** | 是否启用怪物生成。 | 当设置为 **true** 并使用其他默认设置时，将会像类似荒野农场那样，当夜晚来临且玩家位于当前地图时生成各种怪物。怪物类型也会随着战斗技能等级的提高而变化。

下文将介绍每种生成规则的高级配置。当启用上述基本功能时，本模组将使用这些生成设置来确定生成**哪些**对象、每天生成**多少**以及对象生成在**哪里**。

### 通用生成设置
This section covers the general settings that appear in every spawner section.
本章节涵盖了每个生成器的通用配置。

名称 | 有效值 | 描述 | 备注
----|-------|------|---- 
Areas |（参见备注） | 包含若干“spawn areas”的列表，列表内的每个项都代表着某地图上某个“区域”内的详细生成配置。 | 每个“spawn areas”描述了一组要生成的对象及其生成配置，包括在地图上生成的确切位置，以及各种可选的生成规则。若要为多个地图（或同一地图的多个“区域”）配置生成规则，请使用花括号包裹每个区域的生成配置，并用逗号分隔它们：`"Areas": [ { spawn areas 1 设置 }, { spawn areas 2 设置 }, { spawn areas 3 设置 } ]`
UniqueAreaID | 任意唯一的标识名称，例如 "Spawn area 1" | 此区域的唯一标识名称。 | 用于保存系统记录每个区域的某些信息，并出现在 SMAPI 的错误日志中。该名称用于在存档系统中记录每个区域的某些信息，并且会出现在 SMAPI 的错误日志中。如果多个区域使用相同的 ID，则重复项将被自动重命名。后期更改此设置可能会导致现有存档数据出现小问题，例如可能会重置 LimitedNumberOfSpawns。
MapName | 一个或多个地点名，例如 "Farm" | 在游戏中生成对象的地点名称。 | 前往游戏中的某个地点并在 SMAPI 控制台中使用 `whereami` 命令即可查看该地点名称（参见[指令](#指令)）。多个地点名可以用逗号分隔，例如 `"Farm, BusStop"`，且每个地点都会出生成完整的生成集。使用建筑物名称（例如 "Deluxe Barn"）将会针对该建筑物的每个副本进行设置。以 "prefix:"、"suffix:" 或 "contains:" 开头的值可以根据文本匹配多个地点名称；例如，"prefix:Farm" 将匹配所有使用 Farm 作为前缀的地点名，如 Farm、Farmhouse、FarmCave 等。
MinimumSpawnsPerDay | 一个整数（应小于或等于 MaximumSpawnsPerDay） | 每日生成数量下限。 | 若设置为一个非常大的值（例如 9999），则会不断生成对象，直至没有有效空间为止。小于或等于 0 的数字也是合法的数值，且当设置为负数时，会增加当天不生成任何对象的几率。
MaximumSpawnsPerDay | 一个整数（应大于或等于 MinimumSpawnsPerDay） | 每日生成数量上限。 | 若设置为一个非常大的值（例如 9999），则会不断生成对象，直至没有有效空间为止。此上限**可以**被其他设置影响，例如“每个技能等级额外生成百分比”或“最大同时生成数量”。
IncludeTerrainTypes | **"Diggable"、"Grass"**、"Dirt"、"Stone"、"Wood"、"All"、"Quarry"、"Custom" | 一组地形类型，表示对象可以在哪些地形上生成。 | 设置为“All”将允许对象在任何有效地块上生成。可以通过逗号分隔来包含多个地形类型，例如：`"IncludeTerrainTypes": ["Grass", "Diggable"]`。注意：如果您想要在“采石场”地形上生成对象，请使用“Quarry”选项。
ExcludeTerrainTypes | **"Diggable"、"Grass"**、"Dirt"、"Stone"、"Wood"、"All"、"Quarry"、"Custom" | 一组地形类型，表示对象**不能**在哪些地形上生成。 | 参见 IncludeTerrainTypes 的说明以选择类型。被 ExcludeTerrainTypes 覆盖的任何类型都**不会**用于生成对象，并且会**覆盖** IncludeTerrainTypes 和 IncludeCoordinates 配置。
IncludeCoordinates | 形如 `"x1,y1/x2,y2"` 格式的坐标 | 一组坐标，表示对象可以在哪些坐标范围内生成。 | 使用 `whereami` 命令（参见[指令](#指令)）可获取地块的坐标。对象可以生成在这两个坐标之间的任何有效位置内。多个区域可以用逗号分隔，例如：`"IncludeCoordinates": ["0,0/100,100", "125,125/125,125"]`
ExcludeCoordinates | 形如 `"x1,y1/x2,y2"` 格式的坐标 | 一组坐标，表示对象**不能**在哪些坐标范围内生成。 | 参见 IncludeCoordinates 的说明以选择坐标。被 ExcludeCoordinates 覆盖的任何区域都**不会**用于生成对象，并且会**覆盖** IncludeTerrainTypes 和 IncludeCoordinates 配置。
StrictTileChecking | **"Maximum"**、"High"、"Medium"、"Low"、"None" | 本模组验证有效地块的严格程度。 | 根据地图的内部设置（尤其是自定义农场），游戏本体可能会将某些地块视为“无效的物品生成地块”。如果您觉得您的的其他设置是正确的，但没有任何物品生成，请尝试调整此设置。请注意，设置为“Low”和“None”可能会导致物品不生成：如果某个地块确实**不能**放置物品，模组仍然可能认为它是一个有效生成地块，这有可能导致对象在水面、悬崖或建筑物上生成。
DaysUntilSpawnsExpire | **null** 或一个整数 | 生成的对象在消失前留存的天数。 | 如果设置为 null，则生成的对象将会遵循原版游戏内的消失规则。如果设置为正整数，则任何未被玩家采集或击杀的对象将在指定天数后消失。如果设置为 0，则生成的对象将永远不会自动消失。使用此设置将避免采集品在每周六或季节结束时被游戏自动清理。
CustomTileIndex | 一个整数列表 | 图块集中的索引列表，用于 IncludeTerrainTypes 的“Custom”设置。 | 如果上面的 IncludeTerrainTypes 设置包含了“Custom”选项，则任何在地图文件的“Back”图层上具有这些索引编号的地块都将是有效的生成位置。您可以通过站在某个地块上使用 `whereami` 命令来查找该地块的索引编号，或者使用地图/精灵图修改工具。

#### 物品设置
本章节描述了在采集品生成器索引列表（例如“SpringItemIndex”）与自定义怪物掉落物列表中所使用的物品格式。

这些列表接受对象 ID（例如 `206`）、对象名称（例如 `"pizza"`）或使用花括号包裹的复杂物品定义（例如 `{"category": "object", "name": "pizza"}`）。

关于复杂物品设置的更详细描述，请参见下表。

名称 | 必需项 | 有效值 | 描述 | 备注
----|-------|-------|------|---- 
Category | 是 | "Barrel"、"Big Craftable"、"Boots"、"Breakable"、"Buried"、"Chest"、"Crate"、"DGA"、"Fence"、"Gate"、"Furniture"、"Hat"、"Object"、"Pants"、"Ring"、"Shirt"、"Tool"、"Weapon" | 生成物品的类别。| “Breakable”将随机生成一个木桶或箱子（如矿井内可破坏的木桶和箱子那样）。“Buried”将创建一个包含自定义内容物的远古斑点（参见“Contents”字段）。
Name | 是 | 物品名称或 ID，例如 `"Red Mushroom"` | 生成物品的名称或 ID。| 除非 Category 为容器（例如“chest”或“breakable”），否则此设置是必需的。
CanBePickedUp | 否 | **true**, false | 设置为 false 时，玩家将无法拾取此物品。设置为 true 则无影响。| 当家具和可制作物品设置为 false 时可以使用，但不能被拾取。此设置对容器或怪物掉落物不生效。**请务必谨慎使用此设置**。必要时，玩家可以使用 `remove_items` 命令来覆盖此设置。
Contents | 否 | 一个物品列表，例如 `[16, "joja cola"]` | 容器内的物品列表。| 若当前物品不是容器，则此项将被忽略。使用与其他物品列表相同的格式，因此也可以使用复杂的物品定义。
PercentChanceToSpawn | 否 | 一个整数或小数（最小值为 0），例如 `50` 表示 50% 的几率 | 生成此物品的几率。若概率判定失败，该物品将不会生成。| 此设置可用于采集品、怪物掉落物和容器内容物。
Rotation | 否 | 一个整数（最小值/默认值为 0） | 生成家具物品前旋转的次数。| 此设置对非家具物品无效。每个家具物品的可能方向数量由其自身决定；1、2 和 4 是最常见的。
SpawnWeight | 否 | 一个整数（最小值/默认值为 1） | 此采集品类型的生成权重。| 提高某采集品生成的几率，类似于在有效采集品列表中添加多个副本。对“怪物掉落物”或“容器内容物”列表无效。例如：如果采集品 A 的生成权重为 5、采集品 B 的生成权重为 1，则 A 的生成机会将会是 B 的 5 倍。
Stack | 否 | 一个整数（最小值为 1） | 生成物品的堆叠数量。| 此设置应适用于任何可以堆叠的物品。每种物品类型的最大堆叠数量不同，过高的值将被减少到实际的最大值。

以下是一个包含三种格式的怪物掉落物列表示例。击败该怪物后将掉落野山葵、Joja 可乐和银河剑。
```js
"Loot":         // 怪物掉落物列表
[
  16,           // 野山葵的 ID
  "joja cola",  // Joja 可乐的名称
  {
    "category": "weapon",   // 分类为武器
    "name": "galaxy sword"  // 银河剑的名称
  }
],
```
以下是一个容器生成列表示例，它将在春季生成一个包含野山葵、Joja 可乐和银河剑的箱子。
```js
"SpringItemIndex":  // 春季生成物列表
[
  {
    "category": "chest",  // 生成物分类为箱子
    "contents":           // 包含的物品列表
    [
      16,                 // 野山葵的 ID
      "joja cola",        // Joja 可乐的名称
      {
        "category": "weapon",   // 分类为武器
        "name": "galaxy sword"  // 银河剑的名称
      }
    ]
  }
]
```
#### 生成时间设置
本章节描述了每个 spawn area 中对象生成的时间设置。

名称 | 有效值 | 描述 | 备注
----|-------|------|---- 
StartTime | 一个游戏内时间整数，例如 **600** 表示上午 6:00、2550 表示凌晨 1:50 | 对象可以生成的最早时间。 | 此区域生成的对象将会在 StartTime 和 EndTime 之间随机分配生成时间。
EndTime | 一个游戏内时间整数，例如 **600** 表示上午 6:00、2550 表示凌晨 1:50 | 对象可以生成的最晚时间。 | 此区域生成的对象将会在 StartTime 和 EndTime 之间随机分配生成时间。
MinimumTimeBetweenSpawns | 一个整数（10 的倍数，最小值为 **10**） | 每次生成之间的最小游戏内分钟数。 | 每次随机生成时至少间隔的分钟数。例如：如果此值为 `20` 且某对象在上午 6:10 生成，则其下次生成将会在上午 6:30 及之后。
MaximumSimultaneousSpawns | 一个整数（最小值为 1）或 **null** | 特定时间内此区域生成对象的最大数量。 | 如果每个生成时间都已达到此上限，则此设置**将覆盖**每日生成的对象数量。
OnlySpawnIfAPlayerIsPresent | **true**, false | 是否在没有任何玩家位于该地图上时生成对象。 | 如果设置为 true 且当前地图上没有任何玩家，则当前时间下的生成将会被直接跳过，且**不会**延迟到稍后触发。
SpawnSound | 已加载的声音名称或空字符串：**""** | 生成对象时播放的游戏内音效。 | 此设置**区分大小写**，可参考[游戏内可用音效列表](https://wiki.biligame.com/stardewvalley/%E6%A8%A1%E7%BB%84:%E9%9F%B3%E9%A2%91)。

#### 额外条件设置
名称 | 有效值 | 描述 | 备注
----|-------|------|---- 
ExtraConditions |（参见备注） | 一组可选的额外条件，用于为某个区域内生成对象设置额外的生成条件。 | 这些设置可以用于限制生成的时间或天气条件。如果设置为默认值（即 `[]` 或 `null`），则该设置将被忽略。
Years | 一组整数、范围或 "Any"/"All" | 对象生成的年份限制。 | 年份应放在方括号内，并用引号括起来，如果有多个年份，则用逗号分隔。支持单个年份、年份范围或使用 **+** 来表示后续的年份。例如：`["1"]` `["2-4"]` `["1", "3+"]`
Seasons | 一组季节名称列表，列表内元素可以是："Spring"、"Summer"、"Fall"、"Winter" 或 "Any"/"All" | 对象生成的季节限制。 | 季节应放在方括号内，并用引号括起来，如果有多个季节，则用逗号分隔。示例：`["Spring"]`、`["Summer", "Winter"]`
Days | 一组整数、范围或 "Any"/"All" | 对象生成的日期限制。 | 日期应放在方括号内，并用引号包裹，如果有多个日期，则用逗号分隔。支持单个日期、日期范围或使用 **+** 来表示后续的日期。例如：`["1"]` `["2-14"]` `["1", "8+"]`
WeatherYesterday | 一组天气名称列表，列表内元素可以是："Sun"、"Wind"、"Rain"、"Storm"、"Snow" 或 "Any"/"All" | 对象生成前一天的天气限制。 | 天气名称应放在方括号内，并用引号括起来，如果有多个天气，则用逗号分隔。注意，大风天气**不算作晴天**，雷暴**不算作雨天**，如果需要，应当同时包含二者。示例：`["Snow"]`、`["Sun", "Wind"]`、`["Rain", "Storm", "Snow"]`
WeatherToday | 一组天气名称列表，列表内元素可以是："Sun"、"Wind"、"Rain"、"Storm"、"Snow" 或 "Any"/"All" | 对象生成当天的天气限制。 | 天气名称应放在方括号内，并用引号括起来，如果有多个天气，则用逗号分隔。注意，大风天气**不算作晴天**，雷暴**不算作雨天**，如果需要，应当同时包含二者。示例：`["Snow"]`、`["Sun", "Wind"]`、`["Rain", "Storm", "Snow"]`
WeatherTomorrow | 一组天气名称列表，列表内元素可以是："Sun"、"Wind"、"Rain"、"Storm"、"Snow" 或 "Any"/"All" | 对象生成后一天的天气限制。 | 天气名称应放在方括号内，并用引号括起来，如果有多个天气，则用逗号分隔。注意，大风天气**不算作晴天**，雷暴**不算作雨天**，如果需要，应当同时包含二者。示例：`["Snow"]`、`["Sun", "Wind"]`、`["Rain", "Storm", "Snow"]`
GameStateQueries | 一组 [游戏状态查询](https://wiki.biligame.com/stardewvalley/%E6%A8%A1%E7%BB%84:%E6%B8%B8%E6%88%8F%E7%8A%B6%E6%80%81%E6%9F%A5%E8%AF%A2) (GSQs) | 对象生成当天，若列表内有任意一个条件判断为真，则能够生成对象。 | 请参阅 Wiki 上的 [游戏状态查询](https://wiki.biligame.com/stardewvalley/%E6%A8%A1%E7%BB%84:%E6%B8%B8%E6%88%8F%E7%8A%B6%E6%80%81%E6%9F%A5%E8%AF%A2) 以获取用法信息。格式示例：`"GameStateQueries": ["PLAYER_HAS_FLAG Any 1234", "PLAYER_HAS_FLAG Any 5678"]`
CPConditions | 一组 [Content Patcher](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/README.md#for-mod-authors) 的 “When” 条件 | 对象生成当天，若列表内的所有条件判断为真，则能够生成对象。 | 请参阅 [Content Patcher 的模组作者指南](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/README.md#for-mod-authors) 以获取用法信息。格式示例：`"CPConditions": {"HasFlag": "beenToWoods", "FarmCave": "Mushrooms"}`
EPUPreconditions | 一组 [EPU preconditions](https://github.com/ChroniclerCherry/stardew-valley-mods/tree/master/ExpandedPreconditionsUtility) | 对象生成当天，若列表内有任意一个条件判断为真，则能够生成对象。 | 此功能需要 [Expanded Preconditions Utility](https://www.nexusmods.com/stardewvalley/mods/6529) 模组。请参阅 [EPU's README](https://github.com/ChroniclerCherry/stardew-valley-mods/tree/master/ExpandedPreconditionsUtility) 以获取用法信息。在多人游戏中，仅会检查主机的信息。条件 `t <mintime> <maxtime>` 和 `x <letter ID>` **不应使用**，且不受支持。
LimitedNumberOfSpawns | 一个整数 | 此区域生成对象的最大次数限制。 | 在每天结束时，如果此区域生成了对象且没有被其他“额外条件”阻止，则此数字将减少 1（记录在单独的 **.save** 文件中）。一旦计数器归零，此区域将停止生成该对象。请注意，与其他 ExtraConditions 设置不同，此设置不需要使用方括号或引号。例如：`1`

### 采集品生成设置
名称 | 有效值 | 描述 | 备注
----|-------|------|---- 
SpringItemIndex (Area) | **null**，（参见备注） | 春季时**在此区域内**生成的物品 ID 或名称。 | 此设置为每个区域单独设置，并将**覆盖**下面的 SpringItemIndex(Global) 设置。除非您想要在特定区域中生成不同的采集品，否则请将其设置为 *null*。请按照下面 global 字段的格式填写。
SummerItemIndex (Area) | **null**，（参见备注） | 夏季时**在此区域内**生成的物品 ID 或名称。 | 此设置为每个区域单独设置，并将**覆盖**下面的 SummerItemIndex(Global) 设置。除非您想要在特定区域中生成不同的采集品，否则请将其设置为 *null*。请按照下面 global 字段的格式填写。
FallItemIndex (Area)  | **null**，（参见备注） | 秋季时**在此区域内**生成的物品 ID 或名称。 | 此设置为每个区域单独设置，并将**覆盖**下面的 SummerItemIndex(Global) 设置。除非您想要在特定区域中生成不同的采集品，否则请将其设置为 *null*。请按照下面 global 字段的格式填写。
WinterItemIndex (Area) | **null**，（参见备注） | 冬季时**在此区域内**生成的物品 ID 或名称。 | 此设置为每个区域单独设置，并将**覆盖**下面的 SummerItemIndex(Global) 设置。除非您想要在特定区域中生成不同的采集品，否则请将其设置为 *null*。请按照下面 global 字段的格式填写。
PercentExtraSpawnsPerForagingLevel | 一个整数（默认值为 **0**） | 每级采集技能提供的额外采集品生成百分比。 | 在多人游戏中，此设置基于所有玩家（即使是离线的玩家）中最高的技能等级。例如，将此设置为 10，则每拥有 1 级采集技能将额外生成 10% 的采集品，这意味着若某个玩家的采集技能等级为 8，则每天将会额外生成 80% 的采集品。
SpringItemIndex (Global) | 一个整数列表或物品名称列表，例如 `[16, "Red Mushroom"]` | 春季时生成的物品 ID 或名称。 | 默认情况下，该值将会是森林农场在春季时生成的采集品。关于格式信息，请参见 [物品设置](#物品设置) 部分。
SummerItemIndex (Global) | 一个整数列表或物品名称列表，例如 `[16, "Red Mushroom"]` | 夏季时生成的物品 ID 或名称。 | 默认情况下，该值将会是森林农场在春夏季时生成的采集品。关于格式信息，请参见 [物品设置](#物品设置) 部分。
FallItemIndex (Global) | 一个整数列表或物品名称列表，例如 `[16, "Red Mushroom"]` | 秋季时生成的物品 ID 或名称。 | 默认情况下，该值将会是森林农场在秋季时生成的采集品。关于格式信息，请参见 [物品设置](#物品设置) 部分。
WinterItemIndex (Global) | 一个整数列表或物品名称列表，例如 `[16, "Red Mushroom"]` | 冬季时生成的物品 ID 或名称。 | 默认情况下，该值将会是森林农场在冬季时生成的采集品。关于格式信息，请参见 [物品设置](#物品设置) 部分。

### 大型地物生成设置
名称 | 有效值 | 描述 | 备注
----|-------|------|---- 
ObjectTypes | "Stump"、"Log"、"Boulder"、"Quarry Boulder"、"Meteorite"、"Mine Rock 1"、"Mine Rock 2"、"Mine Rock 3"、"Mine Rock 4"、"Weed 1"、"Weed 2"、巨型作物 ID 或 Item Extensions clump ID | 要生成的大型地物类型列表。 | 在此区域生成的对象将从此列表中随机选择。多次添加同一对象类型将增加其生成几率。多个对象用逗号分隔。示例：`"ObjectTypes: [ "Stump", "Log", "Meteorite" ]`
FindExistingObjectLocations | true, **false** | 查找当前地图上包含在 ObjectTypes 列表中的大型地物，并将其添加到 IncludeCoordinates 列表中。 | 该配置项用于自动查找现有大型地物的坐标，并每天重新生成它们。完成一次查找后，它会将自身设置为“false”。
PercentExtraSpawnsPerSkillLevel | 一个整数（默认值为 **0**） | 每级相关技能等级提供的额外对象生成百分比。 | 在多人游戏中，此设置基于所有玩家（即使是离线的玩家）中最高的技能等级。例如，将此设置为 10，则每拥有 1 级技能将额外生成 10% 的对象，这意味着若某个玩家的技能等级为 8，则每天将会额外生成 80% 的对象。
RelatedSkill | "Farming"、"Fishing"、"Foraging"、"Mining"、"Combat" | 由 PercentExtraSpawnsPerSkillLevel 设置使用的相关技能。| 该设置用于确定每个技能等级额外生成的对象数量。

### 采矿点生成设置
名称 | 有效值 | 描述 | 备注
----|-------|------|---- 
MiningLevelRequired (Area) | **null**，（参见备注） | 每种采矿点**在此区域内**生成所需的最低采矿技能等级。 | 此设置为每个区域单独设置，并将**覆盖**下面的 MiningLevelRequired(Global) 设置。除非您想要在特定区域中使用不同的等级要求，否则请将其设置为 *null*。请按照下面 global 字段的格式填写。
StartingSpawnChance (Area) | **null**，（参见备注） | 在玩家的采矿技能等级达到最低要求时，每种采矿点**在此区域内**的生成几率。 | 此设置为每个区域单独设置，并将**覆盖**下面的 StartingSpawnChance(Global) 设置。除非您想要在特定区域中使用不同的生成几率，否则请将其设置为 *null*。请按照下面 global 字段的格式填写。
LevelTenSpawnChance (Area) | **null**，（参见备注） | 在玩家的采矿技能达到 10 级时，每种采矿点**在此区域内**的生成几率。 | 此设置为每个区域单独设置，并将**覆盖**下面的 LevelTenSpawnChance(Global) 设置。除非您想要在特定区域中使用不同的生成几率，否则请将其设置为 *null*。请按照下面 global 字段的格式填写。
PercentExtraSpawnsPerMiningLevel | 一个整数（默认值为 **0**） | 每级采矿技能提供的额外采矿点生成百分比。 | 在多人游戏中，此设置基于所有玩家（即使是离线的玩家）中最高的技能等级。例如，将此设置为 10，则每拥有 1 级采矿技能将额外生成 10% 的采矿点，这意味着若某个玩家的采矿技能等级为 8，则每天将会额外生成 80% 的采矿点。
MiningLevelRequired (Global) | 0-10 |  每种采矿点生成所需的最低采矿技能等级。 | 只有当**任意**玩家（即使是离线的玩家）达到要求的采矿技能等级时，才会开始生成该采矿点。
StartingSpawnChance (Global) | 0 或更高 | 在玩家的采矿技能等级达到最低要求时，每种采矿点的生成几率。 | 这些数字是加权几率，它们不需要加起来等于 100。默认值大致基于原版游戏的生成几率，并略有增加。
LevelTenSpawnChance (Global) | 0 或更高 | 在玩家的采矿技能达到 10 级时，每种采矿点的生成几率。 | 几率将从 StartingSpawnChance 渐渐过渡到 LevelTenSpawnChance。例如，在默认设置中，冰封晶球矿在 5 到 10 级之间的生成几率为 `4, 4, 3, 3, 2, 2`。

### 怪物生成设置
名称 | 有效值 | 描述 | 备注
----|-------|------|---- 
MonsterTypes | 一个“怪物类型”列表（参见备注） | 要生成的怪物类型列表，包含名称和其它可选设置。 | 每个怪物类型之间用逗号分隔：`"MonsterTypes": [ { "MonsterName": "bat", "Settings": {} }, { "MonsterName": "ghost", "Settings": {"HP": 1, "Damage": 0} } ]`
MonsterName | 游戏内的怪物名称，例如 `"green slime"` | `"MonsterTypes"` 中使用的“基础”怪物。 | 使用游戏内现有的怪物类型生成怪物实例，但仍可以通过下面的“可选设置”列表修改怪物的部分属性。要查找可用的怪物名称，请使用 [list_monsters](#list_monsters) 命令。
Settings | A list of setting names and values, e.g. `"HP": 1` | A list of optional customization settings to apply to a Monster Type. | See the Monster Type Settings section below for more information about each setting. Separate each setting with commas: `"Settings": {"HP": 999, "Sprite":"Characters/Monsters/Skeleton"}`
Settings | 一个字典，其键值对形如 `"设置项": 设置值` | 一组可选的自定义设置，用于修改怪物类型的属性。 | 有关具体的设置项信息，请参见下面的“怪物类型设置”部分。每个设置之间用逗号分隔：`"Settings": {"HP": 999, "Sprite":"Characters/Monsters/Skeleton"}`

#### 怪物类型设置
名称 | 有效值 | 描述 | 备注
----|-------|------|---- 
SpawnWeight | 一个整数（最小值/默认值为 1） | 此怪物类型的生成权重。| 提高某怪物生成的几率，类似于在有效怪物列表中添加多个副本。例如：如果怪物 A 的生成权重为 5、怪物 B 的生成权重为 1，则 A 的生成机会将会是 B 的 5 倍。
HP | 一个整数（最小值为 1） | 怪物的最大生命值。| 
CurrentHP | 一个整数（最小值为 1） | 怪物生成时的生命值（非最大生命值）。| 这主要用于模拟怪物以受伤的状态生成，或是适用于可以治疗自身的怪物。
PersistentHP | true, **false** | Whether the monster will keep any HP damage overnight. | This only applies to monsters with `DaysUntilSpawnsExpire` settings.
PersistentHP | true, **false** | 怪物是否会在过夜后保留任何生命值伤害。| 仅适用于设置了 `DaysUntilSpawnsExpire` 的怪物。
Damage | 一个整数（最小值为 0） | 怪物的攻击值。| 某些怪物类型或某些怪物的特定攻击手段会忽略此设置并使用硬编码的伤害值，例如骷髅投掷骨头造成的伤害。
Defense | 一个整数（最小值为 0） | 怪物的防御值 | 每次受到攻击时，怪物受到的伤害会减少此数值。|
DodgeChance | 一个整数（最小值为 0） | 怪物的闪避几率 | 每次受到攻击时，怪物闪避此次攻击的几率。|
EXP | 一个整数（最小值为 0） | 击败怪物后给予玩家的战斗经验值。| 即使设置了此项，击杀农场上生成的怪物时，游戏也**不会**给予玩家任何经验值。
Loot | 一个整数和/或物品名称的列表，例如 `[16, "Red Mushroom"]` | 击败怪物后掉落的战利品列表。| 默认情况下，掉落物**不会**随机化；要实现掉落物的随机化，请使用多个怪物类型或带有“PercentChanceToSpawn”设置的物品。空列表将导致怪物不掉落任何物品。关于格式信息，请参见 [物品设置](#物品设置) 部分。
ExtraLoot | **true**, false | 如果设置为 false，则怪物将不再掉落某些特定类型的物品。| 这可以用于禁用那些通常会忽略“Loot”设置的掉落物。例如，基于史莱姆颜色掉落的特定的物品、霸王喷火龙掉落的恐龙蛋和骨头等。
SightRange | 一个整数 | 怪物察觉到玩家并开始攻击玩家时的距离（以方形模式计算的图块距离）。| 使用 -2 或更低的值可以完全禁用怪物的攻击性。-1 可能会导致怪物的自动攻击，类似于“SeesPlayersAtSpawn”。
SeesPlayersAtSpawn | true, **false** | 如果设置为 true，则怪物将始终察觉到玩家，无论玩家距离多远。| 带有此设置的史莱姆的眼睛将会变成红色，并表现出强烈攻击性。
RangedAttacks | **true**, false | 如果设置为 false，则怪物将不会使用任何远程攻击。| 此设置可能不会影响其他由模组添加的新怪物类型。
InstantKillImmunity | true, **false** | 如果设置为 true，则怪物将免疫某些“秒杀”怪物的效果。| 目前，该选项用于防止怪物被青蛙蛋饰品直接吃掉。
StunImmunity | true, **false** | 如果设置为 true，则怪物将免疫某些“眩晕”效果。| 目前，该选项用于防止怪物被冰霜法杖饰品冻住。
FacingDirection | "up"、"down"、"left"、"right" | 怪物生成时面朝着的方向。| Spikers 将会朝该方向进行攻击。如果未使用此设置，则将随机选择一个方向攻击。
Segments | 一个整数（最小值为 0） | 怪物的额外“体节”数量（如果适用）。| 史莱姆将会在其上方堆叠此数量的额外史莱姆。皇家蛇将会有此数量的尾巴段。
Sprite | 一个已加载素材的“地址” | 用于替换此怪物默认精灵图的已加载精灵图。| 这些可以是游戏中的默认资源，也可以是由 Content Patcher 等模组加载的资源。例如：`"Characters/Monsters/Skeleton"` 或 `"Animals/horse"`
Color | 一个 RGB 或 RGBA 值的字符串，例如 `"255 0 0"` | 怪物的颜色和透明度级别。| 此设置将覆盖 MinColor 和 MaxColor。目前仅适用于史莱姆、巨型史莱姆和金属大头。值范围从 0 到 255，并且可以选择包含 Alpha 通道的透明度，例如：`"0 0 0"` 或 `"0 0 0 127"`
MinColor | 一个 RGB 或 RGBA 值的字符串，例如 `"0 0 0"` | 应用于此怪物的最小颜色和透明度。| 除非同时提供了 MaxColor，否则此设置将被忽略。相关格式请参见上面的 `Color` 设置。
MaxColor | 一个 RGB 或 RGBA 值的字符串，例如 `"255 255 255"` | 应用于此怪物的最大颜色和透明度。| 除非同时提供了 MinColor，否则此设置将被忽略。相关格式请参见上面的 `Color` 设置。
Gender | 一个字符串，例如 "M" 或 "F"（默认随机） | 怪物的性别。| 仅影响小型史莱姆。雄性史莱姆（"M"）将会生成天线，雌性史莱姆（"F"）则不会。
RelatedSkill | "Farming"、"Fishing"、"Foraging"、"Mining"、"Combat" | 由以下 `"Min/MaximumSkillLevel"` 设置所使用的相关技能。| 如果未提供此设置，则下面关于“技能等级”设置将被忽略。在多人游戏中，此设置基于所有玩家（即使是离线的玩家）中最高的技能等级。
MinimumSkillLevel | 一个整数（最小值为 0） | 生成此怪物类型所需的最低技能等级。| 相关技能基于 RelatedSkill 设置。
MaximumSkillLevel | 一个整数（最小值为 0） | 生成此怪物类型所允许的最高技能等级。| 相关技能基于 RelatedSkill 设置。
PercentExtraHPPerSkillLevel | 一个整数 | 怪物的生命值会根据每个技能等级增加此**百分比**。| 相关技能基于 RelatedSkill 设置。接受负值，且负值将减少生命值。
PercentExtraDamagePerSkillLevel | 一个整数 | 怪物造成的伤害值会根据每个技能等级增加此**百分比**。| 相关技能基于 RelatedSkill 设置。接受负值，且负值将减少伤害值。
PercentExtraDefensePerSkillLevel | 一个整数 | 怪物的防御值会根据每个技能等级增加此**百分比**。| 相关技能基于 RelatedSkill 设置。接受负值，且负值将减少防御值。
PercentExtraDodgeChancePerSkillLevel | 一个整数 | 怪物的闪避几率会根据每个技能等级增加此**百分比**。| 相关技能基于 RelatedSkill 设置。接受负值，且负值将减少闪避几率。
PercentExtraEXPPerSkillLevel | 一个整数 | 击败怪物后给予玩家的经验值会根据每个技能等级增加此**百分比**。| 相关技能基于 RelatedSkill 设置。接受负值，且负值将减少经验值。

### 其它设置
名称 | 有效值 | 描述 | 备注
----|-------|------|---- 
QuarryTileIndex | 整数列表 | 游戏图块集中“采石场”图块的图像索引编号列表。| 这些索引是手动选择的，以匹配游戏中山顶农场以及使用类似的采矿区域的自定义地图中的“采石场”图块。提供这些索引能够让那些熟悉游戏地图编辑的用户可以自定义此模组的“采石场”地形设置。

### 文件条件设置
Name | Valid settings | Description | Notes
-----|----------------|-------------|------
File_Conditions | （参见下文） | 一组条件，用于判断是否启用某数据包。 | 这些设置主要用于数据包。
FarmTypes | 一组农场类型列表，列表内元素可以是："Standard"、"Riverland"、"Forest"、"Hilltop"、"Wilderness"、"FourCorners"、"Beach"，或自定义农场类型的 ID | 此数据包将应用于哪些农场类型。 | 这主要用于与自定义农场地图结合的数据包。例如，如果您的自定义农场替换了标准农场，那么请将其设置为 `["Standard"]`。对于添加到 `Data/AdditionalFarms` 的自定义农场类型，请使用该农场类型的 ID，例如 `["MeadowlandsFarm"]`。
FarmerNames | 一组玩家名称列表，例如 `["Esca"]` | 如果当前玩家的名称存在于该列表内，则使用此数据包。 | 多人游戏中此设置仅检查房主的名称，不会受到房客的影响。
SaveFileNames | 一组存档文件名称列表，例如 `["Esca_1234567"]` | 如果当前农场的存档文件名称存在于该列表中，则使用此数据包。 | 这是在其他条件无法有效选择农场时的一个特殊设置。请注意，实际上检查的是存档**文件夹**的名称，而不是存档文件本身。
OtherMods | 一个字典，其键值对形如 `"模组的 UniqueID": true/false`（参见备注） | 如果玩家加载的模组均满足条件时，则使用此数据包。 | 这可以用于使数据包或数据文件仅在满足特定模组要求时激活。`true` 表示必须安装该模组，而 `false` 表示不能安装该模组。例如：`OtherMods: { "Esca.FarmTypeManager" : true, "FakeModID" : false }`，这种情况要求必须安装了 FTM 模组，但不能安装 FakeModID 模组。

## 数据包
开发者可以通过创建数据包来使用 FTM。请注意，在使用 FTM 时，本模组将使用所有关联数据包的数据，并在 `FarmTypeManager\data` 下为每一个存档创建一个副本数据，它们不会互相替换或覆盖。

要创建一个数据包，请按照以下步骤操作：

1. 在 `Stardew Valley\Mods` 文件夹中创建一个空文件夹，并将其命名为 `[FTM] Your Pack Name`。（请将 `Your Pack Name` 替换为您的数据包名称）；
2. 创建一个新的 `manifest.json` 文件（请参阅 Wiki 上关于 [manifest 的说明](https://wiki.biligame.com/stardewvalley/模组:制作指南/APIs/Manifest)），将以下内容复制到其中并将相应的设置填写完整：
  ```js
  {
    "Name": "您的模组名称",
    "Author": "您的名字",
    "Version": "1.0.0",
    "Description": "在这里写上您的描述。简要说明数据包的功能。",
    "UniqueID": "YourName.YourPackName",    // 您的模组的 Unique ID，请使用英文，并确保唯一性
    "MinimumApiVersion": "4.0.0",
    "UpdateKeys": [],
    "ContentPackFor": {
      "UniqueID": "Esca.FarmTypeManager",   // FTM 的 Unique ID
      "MinimumVersion": "1.23.0"
    }
  }
  ```
3. 在 `[FTM] Your Pack Name` 文件夹中创建或复制一个 FTM 配置文件，并将其命名为 **content.json**。其格式与 `FarmTypeManager\data` 文件夹中的农场数据文件完全相同。
4. 如果您希望此数据包仅在特定农场类型上生效，请在 **content.json** 文件的底部添加 `File_Conditions` 字段，并设置 `FarmTypes`、`FarmerNames`、`SaveFileNames`、`OtherMods` 等条件。请参阅上面的 [File Conditions](#file-conditions) 部分以获取更多信息。

## 其它模组功能

### 面向 SMAPI 模组

#### FTM API

FTM 提供了一个小小的 API 供其他 SMAPI 模组使用。如果您想在您的 C# 模组中使用它，您应该这么做：

1. 将 [IFtmApi.cs](https://github.com/Esca-MMC/FarmTypeManager/blob/master/FarmTypeManager/API/IFtmApi.cs) 文件复制到您的模组中。
2. 在所有模组加载完成后，例如在 `GameLaunched` 事件中，使用 SMAPI 的 Helper 获取 API 实例：`var api = Helper.ModRegistry.GetApi<FarmTypeManager.IFtmApi>("Esca.FarmTypeManager");`

欢迎提出额外功能的需求，但请注意，目前没有计划通过 API 实现按需生成。

#### 自定义怪物类

FTM 的怪物生成设置中的 MonsterName 设置可以使用其他模组创建的自定义怪物类。这个过程需要一些 C#和 SMAPI 的知识，您可能还需要反编译并探索 Stardew Valley 中关于怪物的源代码。

创建自定义怪物类的步骤如下：

1. 创建一个基本的 SMAPI 模组。有关更多信息，请参阅 Wiki 上的 [模组开发者指南](https://wiki.biligame.com/stardewvalley/模组:创建_SMAPI_模组)。这个模组不需要执行任何操作，它的任务是存在并被 SMAPI 加载。
2. 在该模组的任意命名空间或类中，创建一个 `StardewValley.Monster` 类或其现有子类（例如子类 Ghost）。
3. 创建一个无参数的默认构造函数（无参数）和一个仅带有 `Vector2` 参数的构造函数。默认构造函数用于适配 Stardew 的一些内部代码，而 Vector2 构造函数则将被 FTM 使用。
4. 根据需要自定义怪物。重写 virtual 方法以更改怪物的“基础”行为，如果怪物需要任何新属性，请务必将它们添加到 `NetFields` 列表中，并使用 ISerializable 类型。
5. 当模组完成后，您应该就能够使用 [list_monsters](#list_monsters) 命令找到您自定义怪物的完整名称。随后即可使用该名称与 FTM 的 MonsterName 设置来生成您的怪物。

未来更新中可能会添加一个示例项目，并且可能会为某些类修复一些错误。目前，请参考 FTM 的[自定义怪物类](https://github.com/Esca-MMC/FarmTypeManager/tree/master/FarmTypeManager/Classes/In-Game/Monsters)作为格式示例。

### For Content Packs

#### Content Patcher Tokens

FTM 添加了以下自定义令牌供 Content Patcher 使用，这些令牌可以被其他数据包所使用。

若要在 Content Patcher 数据包中启用它们，您必须进行以下操作之一：

a. 在数据包的 `manifest.json` 文件中添加 FTM 作为依赖项：`"Dependencies": [{"UniqueID": "Esca.FarmTypeManager"}]`
b. 当您使用 FTM 的令牌时，添加此 "When" 条件：`"HasMod": "Esca.FarmTypeManager"`

格式 | 描述
----|-----
Esca.FarmTypeManager/NumberOfMonsters [location] | 此令牌输出指定位置当前的怪物数量。如果未提供位置名称，则将使用本地玩家的当前位置。使用 `whereami` 控制台命令可以查看您当前所在位置的名称。<br/><br/>如果位置不存在、未加载或在多人游戏中未与本地玩家共享，则此令牌将输出 `-1`。<br/><br/>注意：此令牌**不应**用于 FTM 的 CPConditions 字段，它无法在当天开始时检测 FTM 怪物，而该字段正是在此时使用的。未来版本的 FTM 计划添加对此的更多支持。

#### 游戏状态查询

FTM 向游戏状态查询系统中添加了以下[游戏状态查询](https://wiki.biligame.com/stardewvalley/模组:游戏状态查询)（GSQs）。

它们可以在 FTM 的 GameStateQueries 字段中使用，也可以在各种游戏数据资源的 "Condition" 字段中使用，还可以被其他模组使用。

格式 | 描述
----|-----
Esca.FarmTypeManager_LOCATION_EXISTS &lt;位置&gt; | 如果该名称对应的位置当前已加载，则此查询为 true。已加载的位置应当包括自定义位置、以及当天任何玩家访问过的所有矿井楼层等。
Esca.FarmTypeManager_LOCATION_IS_ACTIVE &lt;位置&gt; | 如果该名称对应的位置当前已加载并与本地玩家共享，则此查询为 true。对于主机玩家，这包括所有已加载的位置；对于多人游戏中的房客，这仅包括本地玩家的当前位置以及一些始终处于活动状态的位置。
Esca.FarmTypeManager_NUMBER_OF_MONSTERS &lt;位置&gt; &lt;最小值&gt; [最大值] | 如果指定位置当前有指定数量的怪物（从最小值到可选的最大值，包括两者），则此查询为 true。它还支持 "Here" 和 "Target" [位置参数](https://wiki.biligame.com/stardewvalley/模组:游戏状态查询#目标地点)。<br/><br/>注意：如果位置未加载，则会抛出错误；如果位置未处于活动状态，则会指示 "-1" 个怪物。强烈建议在此之前使用“位置处于活动状态”查询。<br/><br/>此令牌**不应**用于 FTM 的 GameStateQueries 字段，它无法在当天开始时检测 FTM 怪物，而该字段正是在此时使用的。未来版本的 FTM 计划添加对此的更多支持。