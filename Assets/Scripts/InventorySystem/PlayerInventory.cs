using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Inventory
{
    [CreateAssetMenu(fileName = "PlayerInventory", menuName = "Inventory/Player Inventory")]
    public class PlayerInventory : ScriptableObject
    {
        [Tooltip("All items the player currently owns")]
        public List<InventoryEntry> entries = new List<InventoryEntry>();

        public IEnumerable<InventoryEntry> GetEntriesByCategory(ItemCategory category)
        {
            return entries.Where(entry => entry.item != null && entry.item.category == category);
        }

        public IEnumerable<InventoryEntry> GetAllEntries()
        {
            return entries.Where(entry => entry.item != null);
        }
    }
}