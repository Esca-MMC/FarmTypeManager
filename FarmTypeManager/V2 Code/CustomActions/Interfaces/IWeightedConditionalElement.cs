using StardewValley.GameData;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Represents elements with fields that affect whether, and how often, they're selected from a collection.</summary>
    public interface IWeightedConditionalElement
    {
        /// <summary>The weight to use when selecting this element from a collection. Equivalent to adding the same element to the collection multiple times.</summary>
        /// <remarks>
        /// When using random selection, elements with higher weight are more likely to be selected.
        /// When selecting elements in a specific order, elements with higher weight will be selected multiple times, as if that many copies of the same element were adjacent in the list.
        /// Elements with 0 or less weight should never be selected.
        /// </remarks>
        public int Weight { get; set; }

        /// <summary>A game state query (GSQ) condition that allows this element to be selected from a collection. If false (and not null or empty), selection is prevented.</summary>
        public string Condition { get; set; }

        /// <summary>A mail flag that prevents this element being selected from a collection. If null or empty, repeated selection is allowed.</summary>
        /// <remarks>
        /// <para>
        /// This setting typically ensures that this instance is only selected once, e.g. that an action is only performed once, unless the mail flag is later removed by other means.
        /// <see cref="TriggerActionData.MarkActionApplied"/> serves a similar purpose elsewhere, but doesn't use mail flags; these systems don't directly affect each other.
        /// </para>
        /// <para>
        /// If this field's value is null or empty, the feature is ignored, and the element can be selected multiple times.
        /// If this field's is set, and the current player has a matching mail flag, this element should be ignored/skipped in collections.
        /// When this element is selected, the mail flag (if any) should be automatically applied for the current player, preventing any further selection.
        /// </para>
        /// </remarks>
        public string MarkAppliedWithFlag { get; set; }
    }
}
