using FarmTypeManager.Utilities;
using StardewValley;
using StardewValley.GameData;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace FarmTypeManager.CustomActions
{
    /// <summary>A data model for spawnable items, with additions for this mod's features.</summary>
    public class FTMSpawnItemData : GenericSpawnItemDataWithCondition, IWeightedConditionalElement
    {
        /*******************************/
        /* IWeightedConditionalElement */
        /*******************************/

        public int Weight { get; set; } = 1;
        public string MarkAppliedWithFlag { get; set; } = null;

        /**************************/
        /* Properties - Selection */
        /**************************/

        /// <summary>The random chance that this entry should produce no items, from 0 (always produce items normally) to 1 (never produce items).</summary>
        public double ChanceToSkip { get; set; } = 0;

        /**************************/
        /* Properties - All items */
        /**************************/

        /// <summary>Whether an item should be flagged to prevent it being picked up, if possible.</summary>
        /// <remarks>
        /// <para>For a basic <see cref="Object"/>, this should override fields like <see cref="Object.Fragility"/> and/or <see cref="IsSpawnedObject"/> to prevent player pickup.</para>
        /// <para>In all cases, it should set the key <see cref="FTMUtility.ModDataKeys.CanBePickedUp"/> to the value "false" in <see cref="Item.modData"/>, which should be checked by any code that prevents removal.</para>
        /// </remarks>
        public bool PreventPickup { get; set; } = false;

        /**************************/
        /* Properties - Furniture */
        /**************************/

        /// <summary>The number of times to rotate a placed <see cref="Furniture"/> item.</summary>
        public int? Rotation { get; set; } = null;

        /***********************/
        /* Properties - Object */
        /***********************/

        /// <summary>Whether an <see cref="Object"/> can be picked up by players.</summary>
        /// <remarks>This setting mainly affects <see cref="Object.IsSpawnedObject"/>, which controls whether players can pick up objects. If <see cref="PreventPickup"/> is true, it may override this setting.</remarks>
        public bool? IsSpawnedObject { get; set; } = null;

        /// <summary>Whether an <see cref="Object"/> should have its sprite flipped horizontally.</summary>
        public bool? Flipped { get; set; } = null;

        /// <summary>An <see cref="Object"/>'s fragility value, which affects certain methods of removing or destroying it.</summary>
        /// <remarks>
        /// <para>See <see cref="Object.Fragility"/>. Known behaviors in game version 1.6.15:</para>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see cref="Object.fragility_Removable"/></term>
        ///         <description>Default. The object can be picked up and/or harvested/destroyed by certain tools, if allowed by other settings.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Object.fragility_Delicate"/></term>
        ///         <description>Delicate/Fragile. The object is more brittle than normal, e.g. it can be destroyed by most tools. Contextual and rarely used.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Object.fragility_Indestructable"/></term>
        ///         <description>Indestructible. The object cannot be picked up or affected by tools.</description>
        ///     </item>
        /// </list>
        /// <para>If <see cref="PreventPickup"/> is true, it may override this setting.</para>
        /// </remarks>
        public int? Fragility { get; set; } = null;

        /// <summary>Whether an <see cref="Object"/> should be "on" or "off" when created.</summary>
        /// <remarks>See <see cref="Object.IsOn"/>. This primarily applies to big craftables with the <see cref="Torch"/> class, and decides whether their lights are active when spawned.</remarks>
        public bool? IsOn { get; set; } = null;

        /// <summary>An <see cref="Object"/>'s health or durability when created, e.g. the number of hits from a basic tool needed to harvest/destroy it. Primarily affects stones and ore.</summary>
        /// <remarks>See <see cref="Object.MinutesUntilReady"/>. Some tools use the value as the health/durability of hit objects. Some machines use it as a "minutes until ready" counter for contained objects.</remarks>
        public int? MinutesUntilReady { get; set; } = null;
    }
}