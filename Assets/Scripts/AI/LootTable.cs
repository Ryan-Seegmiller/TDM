using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    /// <summary>
    /// Weighted table of items.
    /// </summary>
    [CreateAssetMenu(menuName = "GameData/AI/LootTable")]
    public class LootTable : ScriptableObject
    {
        #region Types
        /// <summary>
        /// Weighted entry for AI.LootTable.
        /// </summary>
        [System.Serializable]
        public struct LootTableEntry
        {
            public string itemID;
            [Range(0, 1)] public float weight;
        }
        #endregion

        #region Properties
        /// <summary> Whether item weights should auto balance. [This is nessesary for random item selection to work as intended.] </summary>
        [Tooltip("Whether item weights should auto balance. \n[This is nessesary for random item selection to work as intended.]")]
        public bool balance = true;
        /// <summary> Items in the loot table and their selection chance. </summary>
        public LootTableEntry[] items;
        /// <summary> Extra weights used to calculate auto balancing of the loot table. </summary>
        private float[] weights;
        #endregion

        #region Methods
        /// <summary>
        /// Get a random item from the loot table. [NOTE] : Adheres to the item weights.
        /// </summary>
        /// <returns>ItemData</returns>
        public ItemData GetRandom()
        {
            System.Random rand = new System.Random();
            double gen = rand.NextDouble();
            float num = 0;
            foreach (LootTableEntry entrey in items)
            {
                num += entrey.weight;
                if (gen < num) { return ItemPool.instance.itemReferences.GetItemData(entrey.itemID); }
            }
            return ItemPool.instance.itemReferences.GetItemData(items[rand.Next(items.Length)].itemID);
        }
        #endregion

        #region Unity Callbacks
        public void OnValidate()
        {
            // check the cached weights
            if (weights == null || weights.Length != items.Length)
            {
                weights = new float[items.Length];
                for (int i = 0; i < weights.Length; i++)
                {
                    weights[i] = items[i].weight;
                }
            }
            
            // balace the weights
            if (balance)
            {
                if (items.Length == 1) { items[0].weight = weights[0] = 1f; return; }

                int preserve = 0; // Index of weight to be preserved. Preferably the last item the user edited.
                float total = 0; // Sum of the current weights.

                // sum weights
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].weight != weights[i]) { preserve = i; }
                    total += items[i].weight;
                }

                // adjust weights
                float offset = (total - 1f) * (1f / (items.Length - 1)); // difference to adjust all weights by.
                for (int i = 0; i < weights.Length; i++)
                {
                    if (i != preserve)
                    {
                        items[i].weight = weights[i] = Mathf.Clamp01(items[i].weight - offset);
                    }
                }
                weights[preserve] = items[preserve].weight;
            }
        }
        #endregion
    }
}
