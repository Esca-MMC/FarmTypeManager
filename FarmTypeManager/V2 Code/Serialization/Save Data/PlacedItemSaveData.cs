using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Mods;
using static FarmTypeManager.ModEntry;

namespace FarmTypeManager.Serialization
{
    /// <summary>Data used to save and load a <see cref="PlacedItem"/> instance.</summary>
    public class PlacedItemSaveData
    {
        /****************/
        /* Constructors */
        /****************/

        public PlacedItemSaveData()
        {

        }

        /// <summary>Creates save data for this instance.</summary>
        /// <param name="instance">The instance to save.</param>
        public PlacedItemSaveData(PlacedItem instance)
        {
            Item = instance.Item != null ? new ItemSaveData(instance.Item) : null;
            LocationName = instance.Location?.NameOrUniqueName;
            ModData = instance.modDataForSerialization is var modData && modData.Length > 0 ? modData : null; //get the serializer-prepared version of the instance's mod data; if the data is null or empty, use null
            Tile = instance.Tile;
        }

        /**********/
        /* Fields */
        /**********/

        //common

        public string LocationName;
        public ModDataDictionary ModData;
        public Vector2 Tile;

        //PlacedItem

        public ItemSaveData Item;

        /***********/
        /* Methods */
        /***********/

        /// <summary>Creates an instance based on this save data.</summary>
        /// <returns>An instance based on this save data.</returns>
        /// <remarks>Note that this does not apply placement-related fields, e.g. in-game location or tile. Those should be applied separately if applicable.</remarks>
        public PlacedItem Create()
        {
            Item item = Item.Create();
            return new(item); //create and return a PlacedItem containing the item
        }
    }
}
