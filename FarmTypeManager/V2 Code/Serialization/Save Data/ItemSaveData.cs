using StardewValley;
using StardewValley.Mods;
using Object = StardewValley.Object;

namespace FarmTypeManager.Serialization
{
    /// <summary>Data used to save and load an <see cref="Item"/> instance.</summary>
    public class ItemSaveData
    {
        /****************/
        /* Constructors */
        /****************/

        public ItemSaveData()
        {

        }

        /// <summary>Creates save data for this instance.</summary>
        /// <param name="instance">The instance to save.</param>
        public ItemSaveData(Item instance)
        {
            Object obj = instance as Object;

            Fragility = obj?.Fragility;
            Id = instance.QualifiedItemId;
            IsSpawnedObject = obj?.IsSpawnedObject;
            MinutesUntilReady = obj?.MinutesUntilReady;
            ModData = instance.modDataForSerialization is var modData && modData?.Length > 0 ? modData : null; //get the serializer-prepared version of the instance's mod data; if the data is null or empty, use null
            Quality = instance.Quality;
            Stack = instance.Stack;
        }

        /**********/
        /* Fields */
        /**********/

        //common

        public ModDataDictionary ModData;

        //Item

        public string Id;
        public int Stack;
        public int Quality;

        //Object

        public int? Fragility;
        public bool? IsSpawnedObject;
        public int? MinutesUntilReady;

        /***********/
        /* Methods */
        /***********/

        /// <summary>Creates an instance based on this save data.</summary>
        /// <returns>An instance based on this save data.</returns>
        public Item Create()
        {
            Item item = ItemRegistry.Create(itemId: Id, amount: Stack, quality: Quality, allowNull: true);

            if (item is Object obj)
            {
                if (Fragility.HasValue)
                    obj.Fragility = Fragility.Value;
                if (IsSpawnedObject.HasValue)
                    obj.IsSpawnedObject = IsSpawnedObject.Value;
                if (MinutesUntilReady.HasValue)
                    obj.MinutesUntilReady = MinutesUntilReady.Value;
            }

            if (ModData?.Length > 0)
            {
                item.modData.Clear();
                foreach (var entry in ModData.Pairs)
                    item.modData[entry.Key] = entry.Value;
            }

            return item;
        }
    }
}
