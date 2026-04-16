using System.Collections.Generic;
using System.ComponentModel;

namespace FarmTypeManager.Utilities
{
    /// <summary>Static methods used with collections, e.g. to sort or access lists.</summary>
    public static class Collections
    {
        /// <summary>Randomizes the order of elements in a mutable list.</summary>
        /// <param name="list">The list to randomize.</param>
        public static void RandomizeList<T>(List<T> list)
        {
            for (int index = list.Count - 1; index > 0; index--) //for each index except the first, looping backward
            {
                int random = Properties.Random.Next(index + 1); //get a random index between 0 and this tile's index

                //swap the current element with the element at the random index
                var temp = list[random];
                list[random] = list[index];
                list[index] = temp;
            }
        }

        /// <summary>Yields elements from a list using the specified selection mode.</summary>
        /// <param name="list">The list of elements to use.</param>
        /// <param name="mode">The selection mode to use.</param>
        /// <param name="timesToSelect">The number of elements (or sets of elements) to return, depending on mode.</param>
        /// <returns>A yielded series of elements from the list.</returns>
        public static IEnumerable<T> SelectElementsByMode<T>(List<T> list, SelectionMode mode, int timesToSelect)
        {
            if (list == null || list.Count < 1 || timesToSelect < 1) //if no elements were provided/requested
                yield break; //just return an empty set

            switch (mode)
            {
                case SelectionMode.Random:
                    {
                        for (int yieldCount = 0; yieldCount < timesToSelect; yieldCount++)
                            yield return list[Properties.Random.Next(list.Count)]; //return the requested number of random elements
                        yield break;
                    }
                case SelectionMode.RandomOrder:
                    {
                        int returnCount = 0;
                        while (true)
                        {
                            RandomizeList(list);
                            for (int index = 0; index < list.Count; index++) //for each element in randomized order
                            {
                                yield return list[index];
                                returnCount++;
                                if (returnCount >= timesToSelect) //if enough elements have been returned
                                    yield break;
                            }
                        }
                    }
                case SelectionMode.Order:
                    {
                        int returnCount = 0;
                        while (true)
                        {
                            for (int index = 0; index < list.Count; index++) //for each element in order
                            {
                                yield return list[index];
                                returnCount++;
                                if (returnCount >= timesToSelect) //if enough elements have been returned
                                    yield break;
                            }
                        }
                    }
                case SelectionMode.ReverseOrder:
                    {
                        int returnCount = 0;
                        while (true)
                        {
                            for (int index = list.Count - 1; index >= 0; index--) //for each element in reverse order
                            {
                                yield return list[index];
                                returnCount++;
                                if (returnCount >= timesToSelect) //if enough elements have been returned
                                    yield break;
                            }
                        }
                    }
                case SelectionMode.All:
                    {
                        for (int x = 0; x < timesToSelect; x++) //repeat the whole process each time
                            for (int index = 0; index < list.Count; index++) //for each element in order
                                yield return list[index];
                        yield break;
                    }
                case SelectionMode.ReverseAll:
                    {
                        for (int x = 0; x < timesToSelect; x++) //repeat the whole process each time
                            for (int index = list.Count - 1; index >= 0; index--) //for each element in reverse order
                                yield return list[index];
                        yield break;
                    }
                default:
                    throw new InvalidEnumArgumentException(nameof(mode), (int)mode, typeof(SelectionMode)); //unrecognized mode value
            }
        }
    }
}
