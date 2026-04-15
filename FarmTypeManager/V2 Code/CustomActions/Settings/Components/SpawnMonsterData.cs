using StardewValley.GameData;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>A data model used to create one or more monster instances, e.g. by <see cref="SpawnMonsterHandler"/>.</summary>
    public class SpawnMonsterData : ITimesToPerformSettings, IWeightedConditionalElement
    {
        /***************************/
        /* ITimesToPerformSettings */
        /***************************/

        //NOTE: In this class, these fields represent the number of monsters this data should create.

        public int MinTimes { get; set; } = 1;
        public int MaxTimes { get; set; } = 1;
        public List<QuantityModifier> TimesModifiers { get; set; } = null;
        public QuantityModifier.QuantityModifierMode TimesModifierMode { get; set; } = default;

        /*******************************/
        /* IWeightedConditionalElement */
        /*******************************/
        public int Weight { get; set; } = 1;
        public string Condition { get; set; } = null;
        public string MarkAppliedWithFlag { get; set; } = null;

        /**************************/
        /* Properties - Selection */
        /**************************/

        /// <summary>The random chance that this entry should produce no items, from 0 (always produce items normally) to 1 (never produce items).</summary>
        public double ChanceToSkip { get; set; } = 0;

        /**************************************/
        /* Properties - General customization */
        /**************************************/

        /// <summary>A monster ID to use. Merged with <see cref="IdList"/> if both are used.</summary>
        public string Id { get; set; } = null;

        /// <summary>A list of random monster IDs to use. Merged with <see cref="Id"/> if both are used.</summary>
        public List<string> IdList { get; set; } = null;

        /// <summary>The behavior to use when selecting a monster ID from <see cref="IdList"/>.</summary>
        public SelectionMode IdListMode { get; set; } = SelectionMode.Random;

        /// <inheritdoc cref="MonsterData.Sprite"/>
        public string Sprite { get; set; } = null;

        /// <inheritdoc cref="MonsterData.Scale"/>
        public float? Scale { get; set; } = null;

        /// <inheritdoc cref="MonsterData.HideShadow"/>
        public bool? HideShadow { get; set; } = null;

        /// <inheritdoc cref="MonsterData.MaxHealth"/>
        public int? MaxHealth { get; set; } = null;

        /// <inheritdoc cref="MonsterData.CurrentHealth"/>
        public int? CurrentHealth { get; set; } = null;

        /// <inheritdoc cref="MonsterData.Damage"/>
        public int? Damage { get; set; } = null;

        /// <inheritdoc cref="MonsterData.Defense"/>
        public int? Defense { get; set; } = null;

        /// <inheritdoc cref="MonsterData.DodgeChance"/>
        public double? DodgeChance { get; set; } = null;

        /// <inheritdoc cref="MonsterData.Experience"/>
        public int? Experience { get; set; } = null;

        /// <inheritdoc cref="MonsterData.SightRange"/>
        public int? SightRange { get; set; } = null;

        /// <inheritdoc cref="MonsterData.FocusedOnFarmers"/>
        public bool? FocusedOnFarmers { get; set; } = null;

        /// <inheritdoc cref="MonsterData.FacingDirection"/>
        public int? FacingDirection { get; set; } = null;

        /// <inheritdoc cref="MonsterData.LootData"/>
        public ItemSpawnField LootData { get; set; } = null;

        /// <inheritdoc cref="MonsterData.DisableExtraLoot"/>
        public bool DisableExtraLoot { get; set; } = false;

        /// <inheritdoc cref="MonsterData.DisableInstantKill"/>
        public bool DisableInstantKill { get; set; } = false;

        /// <inheritdoc cref="MonsterData.DisableStun"/>
        public bool DisableStun { get; set; } = false;

        /// <inheritdoc cref="MonsterData.DaysUntilRemoved"/>
        public int? DaysUntilRemoved { get; set; } = 1;

        /********************************************/
        /* Properties - Type-specific customization */
        /********************************************/

        /// <summary>An override value for a monster's color, formatted as a space-separated string of RGB or RGBA values (e.g. "255 0 0" for red). Ignored if null.  Combined with <see cref="ColorList"/> if both are provided.</summary>
        public string Color { get; set; } = null;

        /// <summary>A list of possible override values for a monster's color, formatted as space-separated strings of RGB or RGBA values (e.g. "255 0 0" for red). Ignored if null. Combined with <see cref="Color"/> if both are provided.</summary>
        public List<string> ColorList { get; set; } = null;

        /// <summary>The method to use when selecting a color.</summary>
        public SelectionMode ColorListMode { get; set; } = SelectionMode.Random;

        /// <inheritdoc cref="MonsterData.DisableRangedAttacks"/>
        public bool DisableRangedAttacks { get; set; } = false;

        /// <inheritdoc cref="MonsterData.Gender"/>
        public string Gender { get; set; } = null;

        /// <inheritdoc cref="MonsterData.Segments"/>
        public int? Segments { get; set; } = null;

        /***********/
        /* ModData */
        /***********/

        /// <inheritdoc cref="MonsterData.ModData"/>
        public Dictionary<string, string> ModData { get; set; } = null;
    }
}
