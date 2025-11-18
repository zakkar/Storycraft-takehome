using UnityEngine;

namespace Game.Inventory
{
    /// <summary>
    /// Defines a single inventory item type (e.g., "Wood", "Stone House").
    /// Acts as a template that can be referenced by actual inventory entries.
    /// </summary>
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Definition")]
    public class InventoryItemDefinition : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Unique identifier for this item")]
        public string itemId;

        [Tooltip("Display name shown to the player")]
        public string displayName;

        [Header("Visual")]
        [Tooltip("Icon sprite displayed in inventory slots")]
        public Sprite icon;

        [Header("Organization")]
        [Tooltip("Category for filtering and tab organization")]
        public ItemCategory category;

        [Header("Details")]
        [TextArea(2, 4)]
        [Tooltip("Description or tooltip text")]
        public string description;
    }
}