using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using StardewModdingAPI.Utilities;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Data describing a specific monster instance for creation and/or serialization.</summary>
    public class MonsterData
    {
        /**************************************/
        /* Properties - General customization */
        /**************************************/

        /// <summary>The ID used to spawn this type of monster.</summary>
        /// <remarks>
        /// <para>These IDs are defined and recognized by <see cref="MonsterManager">. They don't necessarily match a monster's class or any built-in field.</para>
        /// <para>This value should be stored in each monster's mod data for reference, using the key <see cref="FTMUtility.ModDataKeys.SpawnId"/>.</para>
        /// </remarks>
        public string SpawnId { get; set; } = null;

        /// <summary>An override value for the monster's sprite texture, e.g. "Characters/Monsters/Bat". Ignored if null.</summary>
        public string Sprite { get; set; } = null;

        /// <summary>The relative size at which to draw the monster's sprite, e.g. 1.5 to make it 50% larger. Ignored if null.</summary>
        public float? Scale { get; set; } = null;

        /// <summary>An override value for <see cref="NPC.HideShadow"/>. Ignored if null.</summary>
        public bool? HideShadow { get; set; } = null;

        /// <summary>An override value for <see cref="Monster.MaxHealth"/>. Ignored if null.</summary>
        public int? MaxHealth { get; set; } = null;

        /// <summary>An override value for <see cref="Monster.Health"/>. Ignored if null.</summary>
        public int? CurrentHealth { get; set; } = null;

        /// <summary>An override value for <see cref="Monster.DamageToFarmer"/>. Ignored if null.</summary>
        /// <remarks>Some monster types may ignore this value and/or use another field, e.g. one for ranged attack damage.</remarks>
        public int? Damage { get; set; } = null;

        /// <summary>An override value for <see cref="Monster.resilience"/>. Ignored if null.</summary>
        public int? Defense { get; set; } = null;

        /// <summary>An override value for <see cref="Monster.missChance"/>. Ignored if null.</summary>
        public double? DodgeChance { get; set; } = null;

        /// <summary>An override value for <see cref="Monster.ExperienceGained"/>. Ignored if null.</summary>
        public int? Experience { get; set; } = null;

        /// <summary>An override value for <see cref="NPC.moveTowardPlayerThreshold"/>. Ignored if null.</summary>
        /// <remarks>This may also set <see cref="NPC.IsWalkingTowardPlayer"/> to true; see the <see cref="NPC.moveTowardPlayer(int)"/> method.</remarks>
        public int? SightRange { get; set; } = null;

        /// <summary>An override value for <see cref="Monster.focusedOnFarmers"/>. Ignored if null.</summary>
        /// <remarks>Some types may need to set additional fields, e.g. <see cref="Skeleton.spottedPlayer"/>. This field might cause behavioral bugs with certain monsters types; support isn't guaranteed or required.</remarks>
        public bool? FocusedOnFarmers { get; set; } = null;

        /// <summary>An override value for <see cref="Character.FacingDirection"/>. Ignored if null.</summary>
        /// <remarks>To convert string directions like "up" into integers, use <see cref="Utility.TryParseDirection(string, out int)"/> or similar methods.</remarks>
        public int? FacingDirection { get; set; } = null;

        /// <summary>A data model for item creation. If not null, this generates items that replace the monster's normal item drops when defeated.</summary>
        public ItemSpawnField LootData { get; set; }

        /// <summary>If true, the monster's hard-coded drops should be disabled where possible, e.g. <see cref="Monster.getExtraDropItems"/>.</summary>
        /// <remarks>
        /// <para>FTM will disable <see cref="Monster.getExtraDropItems"/> using Harmony.</para>
        /// <para>However, if this setting is true and a custom monster type has its own unique item drop mechanisms, its handler should disable those too, if possible.</para>
        /// <para>Handlers can check the monster's mod data for the key <see cref="FTMUtility.ModDataKeys.ExtraLoot"/> with the value "false".</para>
        /// </remarks>
        public bool DisableExtraLoot { get; set; } = false;

        /// <summary>If true, this monster should be immune to "instant kill" effects, e.g. those caused by trinkets.</summary>
        /// <remarks>
        /// <para>FTM will attempt to disable these effects via Harmony.</para>
        /// <para>Custom monster types with other player-triggered "instant kill" effects should also be disabled by handlers, if possible.</para>
        /// <para>Handlers can check the monster's mod data for the key <see cref="FTMUtility.ModDataKeys.InstantKillImmunity"/> with the value "true".</para>
        /// </remarks>
        public bool DisableInstantKill { get; set; } = false;

        /// <summary>If true, this monster should be immune to stun effects, e.g. those caused by trinkets.</summary>
        /// <remarks>
        /// <para>FTM will attempt to disable these effects via Harmony.</para>
        /// <para>Custom monster types with other player-triggered stun effects should also be disabled by handlers, if possible.</para>
        /// <para>Handlers can check the monster's mod data for the key <see cref="FTMUtility.ModDataKeys.StunImmunity"/> with the value "true".</para>
        /// </remarks>
        public bool DisableStun { get; set; } = false;

        /// <summary>The number of days to wait before removing this monster from the game overnight. If null, it won't be automatically removed.</summary>
        /// <remarks>
        /// <para>
        /// This defaults to 1, which means the monster will be removed overnight: it will only will exist on the day it spawned.
        /// 2 will cause the monster to persist overnight once after spawning, then be removed the following night, and so on.
        /// Values lower than 1 are equivalent to 1.
        /// </para>
        /// <para>
        /// This should be used to generate a value for <see cref="DayToRemove"/>, e.g. during initial creation or customization.
        /// </para>
        /// </remarks>
        public int? DaysUntilRemoved { get; set; } = 1;

        /********************************************/
        /* Properties - Type-specific customization */
        /********************************************/

        /// <summary>An override value for a monster's color, if relevant. Ignored if null.</summary>
        public string Color { get; set; } = null;

        /// <summary>If true, any ranged attacks this monster uses should be disabled.</summary>
        public bool DisableRangedAttacks { get; set; } = false;

        /// <summary>An override value for this monster's gender, if relevant. Ignored if null.</summary>
        /// <remarks>This mainly effects <see cref="GreenSlime"/> appearances and mating behavior. For example, values starting with "M" or "F" set <see cref="GreenSlime.cute"/> to true or false.</remarks>
        public string Gender { get; set; } = null;

        /// <summary>An override value for the number of extra "segments" this monster has, if relevant. Ignored if null.</summary>
        /// <remarks>For example, this sets the number of tail segments on a royal <see cref="Serpent"/>, or the number of extra "stacked" slimes on top of a <see cref="GreenSlime"/>.</remarks>
        public int? Segments { get; set; } = null;

        /***********/
        /* ModData */
        /***********/

        /// <summary>A set of custom entries to add to the monster's <see cref="IHaveModData.modData"/> property.</summary>
        /// <remarks>If any other customizations edit the same entries during initial monster creation, this property's entries should overwrite them.</remarks>
        public Dictionary<string, string> ModData { get; set; } = [];

        /*************************************************/
        /* Properties - Placement and serialization info */
        /*************************************************/

        //NOTE: The monster's GameLocation and its name are omitted here for complexity and performance reasons.
        //      Monster creation and/or placement methods should apply it themselves as needed.
        //      Notably, none of the base game monster types need to know their location at construction time.

        /// <summary>The monster's pixel position, i.e. <see cref="Character.Position"/>.</summary>
        public Vector2? Position { get; set; } = null;

        /// <summary>The monster's current movement velocity, i.e. <see cref="Character.xVelocity"/> and <see cref="Character.yVelocity"/>.</summary>
        public Vector2? Velocity { get; set; } = null;

        /// <summary>The day when this monster should be removed from the game. If null, no automatic removal is needed.</summary>
        /// <remarks>
        /// This should be set based on <see cref="DaysUntilRemoved"/> at monster creation time, e.g. by adding that value to <see cref="WorldDate.GetDaysPlayed(int, Season, int)"/>.
        /// When the next in-game day matches (or exceeds) this value, the monster and its data should be removed.
        /// </remarks>
        public int? DayToRemove { get; set; } = null;

        /***********/
        /* Methods */
        /***********/

        /// <summary>Apply this data's customization properties to the monster instance created with it.</summary>
        /// <param name="monster">The monster created with this data.</param>
        public void ApplyToMonster(Monster monster)
        {
            if (monster == null)
                return;

            /*************************/
            /* General customization */
            /*************************/

            if (SpawnId != null)
                monster.modData[FTMUtility.ModDataKeys.SpawnId] = SpawnId;

            if (Sprite != null)
            {
                if (!Game1.content.DoesAssetExist<Texture2D>(Sprite))
                    throw new ArgumentException($"A monster's custom \"Sprite\" setting points to an asset that isn't currently loaded: \"{Sprite}\".");

                monster.Sprite = monster.Sprite = new AnimatedSprite(Sprite, monster.Sprite.CurrentFrame, monster.Sprite.SpriteWidth, monster.Sprite.SpriteHeight); //create a new sprite with the existing values
                monster.modData[FTMUtility.ModDataKeys.Sprite] = Sprite;
            }

            if (Scale.HasValue)
                monster.Scale = Scale.Value;

            if (HideShadow.HasValue)
                monster.HideShadow = HideShadow.Value;

            if (MaxHealth.HasValue)
            {
                monster.MaxHealth = MaxHealth.Value;
                if (!CurrentHealth.HasValue && monster.Health > MaxHealth.Value) //if current health isn't set (i.e. it wasn't customized during creation) and the monster's health is now higher than max
                    monster.Health = MaxHealth.Value; //reduce it to max
            }

            if (CurrentHealth.HasValue)
                monster.Health = CurrentHealth.Value;

            if (Damage.HasValue)
                monster.DamageToFarmer = Damage.Value;

            if (Defense.HasValue)
                monster.resilience.Value = Defense.Value;

            if (DodgeChance.HasValue)
                monster.missChance.Value = DodgeChance.Value;

            if (Experience.HasValue)
                monster.ExperienceGained = Experience.Value;

            if (SightRange.HasValue)
            {
                monster.moveTowardPlayer(SightRange.Value); //note: currently (SDV v1.6.15), this method sets the "moveTowardPlayerThreshold" value and "isWalkingTowardPlayer" flag, which causes the intended behavior
                monster.modData[FTMUtility.ModDataKeys.SightRange] = SightRange.Value.ToString();
            }

            if (FocusedOnFarmers.HasValue)
                monster.focusedOnFarmers = FocusedOnFarmers.Value;

            if (FacingDirection.HasValue)
                monster.faceDirection(FacingDirection.Value);

            if (LootData != null)
                monster.objectsToDrop?.Clear(); //clear any preset loot

            if (DisableExtraLoot)
                monster.modData[FTMUtility.ModDataKeys.ExtraLoot] = "false";

            if (DisableInstantKill)
                monster.modData[FTMUtility.ModDataKeys.InstantKillImmunity] = "true";

            if (DisableStun)
                monster.modData[FTMUtility.ModDataKeys.StunImmunity] = "true";

            /*******************************/
            /* Type-specific customization */
            /*******************************/

            if (Color != null)
                monster.modData[FTMUtility.ModDataKeys.Color] = Color;

            if (DisableRangedAttacks)
                monster.modData[FTMUtility.ModDataKeys.DisableRangedAttacks] = "true";

            if (Gender != null)
                monster.modData[FTMUtility.ModDataKeys.Gender] = Gender;

            if (Segments.HasValue)
                monster.modData[FTMUtility.ModDataKeys.Segments] = Segments.Value.ToString();

            /************************************/
            /* Placement and serialization info */
            /************************************/

            if (Position.HasValue)
                monster.Position = Position.Value;

            if (Velocity.HasValue)
            {
                monster.xVelocity = Velocity.Value.X;
                monster.yVelocity = Velocity.Value.Y;
            }

            if (DaysUntilRemoved.HasValue && DayToRemove == null) //if a removal date is needed
                DayToRemove = WorldDate.GetDaysPlayed(Game1.year, Game1.season, Game1.dayOfMonth) + DaysUntilRemoved; //set it based on the current in-game date

            /***********/
            /* ModData */
            /***********/

            if (ModData != null)
                foreach (var entry in ModData)
                    monster.modData[entry.Key] = entry.Value; //set each custom value (NOTE: this should be applied last, which prioritizes custom overrides and saved data)
        }

        /// <summary>Updates variable properties based on the monster instance's current state, if relevant.</summary>
        /// <param name="monster">The monster instance already associated with this data.</param>
        /// <remarks>
        /// For performance reasons, this method does NOT update all properties.
        /// It should only be called on the data instance that was used to create this monster instance.
        /// References to those should be retained together, if necessary.
        /// </remarks>
        public void UpdateFromMonster(Monster monster)
        {
            if (monster == null)
                return;

            //replace stored mod data with monster's current data (NOTE: this should be applied first, which allows other settings to update based on existing values)
            if (ModData == null)
                ModData = [];
            else
                ModData.Clear();
            foreach (var entry in monster.modData.Pairs)
                ModData[entry.Key] = entry.Value;

            /*************************/
            /* General customization */
            /*************************/

            //skip SpawnId and Sprite (post-spawn ID changes not supported; Sprite requires parsing)
            Scale = monster.Scale;
            HideShadow = monster.HideShadow;
            MaxHealth = monster.MaxHealth;
            CurrentHealth = monster.Health;
            Damage = monster.DamageToFarmer;
            Defense = monster.resilience.Value;
            DodgeChance = monster.missChance.Value;
            Experience = monster.ExperienceGained;
            //skip SightRange (covered by spawn/behavior section if necessary, and requires parsing)
            FocusedOnFarmers = monster.focusedOnFarmers;
            FacingDirection = monster.FacingDirection;
            //skip loot (not stored on monster instances)
            //skip general "disable" toggles (post-spawn changes not supported)
            //skip mod data here because it's handled above

            /*******************************/
            /* Type-specific customization */
            /*******************************/

            if (ModData.TryGetValue(FTMUtility.ModDataKeys.Color, out string color))
                Color = color;

            //skip "disable ranged attacks" (post-spawn changes not supported)

            if (ModData.TryGetValue(FTMUtility.ModDataKeys.Gender, out string gender))
                Gender = gender;

            if (ModData.TryGetValue(FTMUtility.ModDataKeys.Segments, out string segmentsText) && int.TryParse(segmentsText, out int segments))
                Segments = segments;

            /************************************/
            /* Placement and serialization info */
            /************************************/

            Position = monster.Position;

            Velocity = new(monster.xVelocity, monster.yVelocity);
        }
    }
}
