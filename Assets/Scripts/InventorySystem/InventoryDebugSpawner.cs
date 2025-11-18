using UnityEngine;

namespace Game.Inventory
{
    /// <summary>
    /// Debug utility to quickly populate a PlayerInventory with test data.
    /// Useful for testing the inventory UI without manually creating many items.
    /// </summary>
    public class InventoryDebugSpawner : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private PlayerInventory playerInventory;

        [Header("Test Items (Create these first!)")]
        [SerializeField] private InventoryItemDefinition[] testItems;

        [ContextMenu("Populate Test Data")]
        private void PopulateTestData()
        {
            if (playerInventory == null)
            {
                Debug.LogError("[InventoryDebug] PlayerInventory is not assigned!");
                return;
            }

            if (testItems == null || testItems.Length == 0)
            {
                Debug.LogWarning("[InventoryDebug] No test items assigned. Please create some InventoryItemDefinition assets first.");
                return;
            }

            // Clear existing entries
            playerInventory.entries.Clear();

            // Add test items with random quantities
            int itemsAdded = 0;
            foreach (var item in testItems)
            {
                if (item != null)
                {
                    int quantity = Random.Range(1, 100);
                    playerInventory.entries.Add(new InventoryEntry(item, quantity));
                    itemsAdded++;
                    Debug.Log($"[InventoryDebug] Added: {item.displayName} ({item.category}) x{quantity}");
                }
            }

            Debug.Log($"[InventoryDebug] Populated {itemsAdded} items into {playerInventory.name}");

#if UNITY_EDITOR
            // Mark the asset dirty so changes are saved
            UnityEditor.EditorUtility.SetDirty(playerInventory);
#endif
        }

        private void OnValidate()
        {
            // Optional: Auto-populate when component values change in the editor
            // Uncomment if you want automatic population (can be annoying during setup)
            // PopulateTestData();
        }
    }
}