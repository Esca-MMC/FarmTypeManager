using StardewValley;
using StardewValley.GameData;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace FarmTypeManager.CustomActions
{
    /// <summary>An data model for spawnable items, with additions for this mod's features.</summary>
    public class FTMSpawnItemData : GenericSpawnItemDataWithCondition
    {
        /************************/
        /* Properties - General */
        /************************/

        /// <summary>The weight to use for this entry when randomly selecting it, if applicable.</summary>
        /// <remarks>This is equivalent to adding additional copies of this entry to a set. For example, an entry with weight = 10 should be 10 times as likely to be selected as an entry with weight = 1.</remarks>
        public int Weight { get; set; } = 1;

        /// <summary>The random chance that this entry should produce no items, from 0 (always produce items normally) to 1 (never produce items).</summary>
        public double ChanceToSkip { get; set; } = 0;

        /**********************/
        /* Properties - Items */
        /**********************/

        /// <summary>Whether an item should be flagged as indestructible, i.e. prevented from being picked up, or destroyed.</summary>
        /// <remarks>
        /// <para>For a basic <see cref="Object"/>, this should set <see cref="Object.Fragility"/> to <see cref="Object.fragility_Indestructable"/>, overriding <see cref="Fragility"/> if necessary.</para>
        /// <para>"<see cref="FTMUtility.ModDataKeys.CanBePickedUp"/>": "false" should also be set in <see cref="Item.modData"/>, which applies additional changes to prevent removal.</para>
        /// </remarks>
        public bool Indestructible { get; set; } = false;

        /************************/
        /* Properties - Objects */
        /************************/

        /// <summary>Whether an <see cref="Object"/> can be picked up by players.</summary>
        /// <remarks>This setting mainly affects <see cref="Object.IsSpawnedObject"/>, which controls whether players can pick up objects. If <see cref="Indestructible"/> is true, it will generally override this setting.</remarks>
        public bool? CanPickUp { get; set; } = null;

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
        /// <para>If <see cref="Indestructible"/> is true, it will generally override this setting.</para>
        /// </remarks>
        public int? Fragility { get; set; } = null;

        /// <summary>An <see cref="Object"/>'s health or durability when created, e.g. the number of hits from a basic tool needed to harvest/destroy it. Primarily affects stones and ore.</summary>
        /// <remarks>See <see cref="Object.MinutesUntilReady"/>. Pickaxes treat that property on objects as a health/durability value.</remarks>
        public int? Health { get; set; } = null;

        /**************************/
        /* Properties - Furniture */
        /**************************/

        /// <summary>The number of times to rotate a placed <see cref="Furniture"/> item.</summary>
        public int? Rotation { get; set; } = null;
    }
}