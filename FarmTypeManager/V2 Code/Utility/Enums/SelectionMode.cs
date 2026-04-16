namespace FarmTypeManager.Utilities
{
    /// <summary>A pattern to use when selecting elements from a collection.</summary>
    public enum SelectionMode
    {
        /// <summary>A random element should be selected each time.</summary>
        /// <remarks>For example, if 5 elements are requested, 5 random elements should be returned. The same element might be randomly chosen multiple times in a row, even if others exist.</remarks>
        Random,
        /// <summary>Elements should be selected in random order.</summary>
        /// <remarks>For example, if 5 elements are requested, 5 elements should be returned in random order. The same element should only be returned multiple times if the collection has fewer than 5 elements.</remarks>
        RandomOrder,
        /// <summary>Elements should be selected in order, e.g. from top to bottom.</summary>
        /// <remarks>For example, if 5 elements are requested, the first 5 elements should be returned. The same element should only be returned multiple times if the collection has fewer than 5 elements.</remarks>
        Order,
        /// <summary>Elements should be selected in reverse order, e.g. from bottom to top.</summary>
        /// <remarks>For example, if 5 elements are requested, the last 5 elements should be returned. The same element should only be returned multiple times if the collection has fewer than 5 elements.</remarks>
        ReverseOrder,
        /// <summary>Every element in the collection should be selected, once per request. This should happen in order, e.g. from top to bottom.</summary>
        /// <remarks>For example, if 5 sets of elements are requested, every entry in the list should be returned 5 times, in order from top to bottom each time. Note that this should return (5 * collection size) elements, not 5 elements.</remarks>
        All,
        /// <summary>Every element in the collection should be selected, once per request. This should happen in reverse order, e.g. from bottom to top.</summary>
        /// <remarks>For example, if 5 sets of elements are requested, every entry in the list should be returned 5 times, in reverse order from bottom to top each time. Note that this should return (5 * collection size) elements, not 5 elements.</remarks>
        ReverseAll
    }
}
