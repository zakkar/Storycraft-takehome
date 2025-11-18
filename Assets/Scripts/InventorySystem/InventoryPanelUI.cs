using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Inventory.UI
{
    /// <summary>
    /// Main controller for the inventory panel UI.
    /// Handles tab switching, pagination, and item slot rendering.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class InventoryPanelUI : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private PlayerInventory playerInventory;

        [Header("Pagination")]
        [SerializeField] private int slotsPerPage = 8;

        // UI Element references
        private VisualElement slotGrid;
        private Label pageLabel;
        private Button tabResources;
        private Button tabBuildings;
        private Button tabDecorations;
        private Button tabOther;
        private Button pagePrev;
        private Button pageNext;

        // State
        private ItemCategory currentCategory = ItemCategory.Resources;
        private int currentPageIndex = 0;
        private List<InventoryEntry> currentEntries = new List<InventoryEntry>();

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            // Query UI elements
            slotGrid = root.Q<VisualElement>("slot-grid");
            pageLabel = root.Q<Label>("page-label");

            tabResources = root.Q<Button>("tab-resources");
            tabBuildings = root.Q<Button>("tab-buildings");
            tabDecorations = root.Q<Button>("tab-decorations");
            tabOther = root.Q<Button>("tab-other");

            pagePrev = root.Q<Button>("page-prev");
            pageNext = root.Q<Button>("page-next");

            // Subscribe to tab button clicks
            tabResources.clicked += () => ChangeCategory(ItemCategory.Resources);
            tabBuildings.clicked += () => ChangeCategory(ItemCategory.Buildings);
            tabDecorations.clicked += () => ChangeCategory(ItemCategory.Decorations);
            tabOther.clicked += () => ChangeCategory(ItemCategory.Other);

            // Subscribe to page navigation
            pagePrev.clicked += OnPrevPage;
            pageNext.clicked += OnNextPage;

            // Initialize with Resources tab
            ChangeCategory(ItemCategory.Resources);
        }

        /// <summary>
        /// Switches to a new category tab and resets to first page.
        /// </summary>
        private void ChangeCategory(ItemCategory newCategory)
        {
            currentCategory = newCategory;
            currentPageIndex = 0;

            // Update tab button visuals
            UpdateTabVisuals();

            // Filter inventory by category
            if (playerInventory != null)
            {
                currentEntries = playerInventory.GetEntriesByCategory(currentCategory).ToList();
                Debug.Log($"[Inventory] Changed to category: {currentCategory}, found {currentEntries.Count} items");
            }
            else
            {
                currentEntries.Clear();
                Debug.LogWarning("[Inventory] PlayerInventory is not assigned!");
            }

            RefreshPage();
        }

        /// <summary>
        /// Updates which tab button appears selected.
        /// </summary>
        private void UpdateTabVisuals()
        {
            tabResources.RemoveFromClassList("tab-selected");
            tabBuildings.RemoveFromClassList("tab-selected");
            tabDecorations.RemoveFromClassList("tab-selected");
            tabOther.RemoveFromClassList("tab-selected");

            switch (currentCategory)
            {
                case ItemCategory.Resources:
                    tabResources.AddToClassList("tab-selected");
                    break;
                case ItemCategory.Buildings:
                    tabBuildings.AddToClassList("tab-selected");
                    break;
                case ItemCategory.Decorations:
                    tabDecorations.AddToClassList("tab-selected");
                    break;
                case ItemCategory.Other:
                    tabOther.AddToClassList("tab-selected");
                    break;
            }
        }

        /// <summary>
        /// Navigates to the previous page.
        /// </summary>
        private void OnPrevPage()
        {
            if (currentPageIndex > 0)
            {
                currentPageIndex--;
                Debug.Log($"[Inventory] Page changed to: {currentPageIndex + 1}");
                RefreshPage();
            }
        }

        /// <summary>
        /// Navigates to the next page.
        /// </summary>
        private void OnNextPage()
        {
            int totalPages = GetTotalPages();
            if (currentPageIndex < totalPages - 1)
            {
                currentPageIndex++;
                Debug.Log($"[Inventory] Page changed to: {currentPageIndex + 1}");
                RefreshPage();
            }
        }

        /// <summary>
        /// Calculates total pages needed for current entries.
        /// </summary>
        private int GetTotalPages()
        {
            if (currentEntries.Count == 0) return 1;
            return Mathf.CeilToInt((float)currentEntries.Count / slotsPerPage);
        }

        /// <summary>
        /// Rebuilds the slot grid for the current page.
        /// </summary>
        private void RefreshPage()
        {
            slotGrid.Clear();

            int totalPages = GetTotalPages();
            int startIndex = currentPageIndex * slotsPerPage;
            int endIndex = Mathf.Min(startIndex + slotsPerPage, currentEntries.Count);

            // Create slots for items on this page
            for (int i = startIndex; i < endIndex; i++)
            {
                var entry = currentEntries[i];
                CreateItemSlot(entry);
            }

            // Fill remaining slots with empty placeholders for consistent grid shape
            int emptySlots = slotsPerPage - (endIndex - startIndex);
            for (int i = 0; i < emptySlots; i++)
            {
                CreateEmptySlot();
            }

            // Update page label
            pageLabel.text = $"{currentPageIndex + 1} / {totalPages}";

            // Enable/disable navigation buttons
            pagePrev.SetEnabled(currentPageIndex > 0);
            pageNext.SetEnabled(currentPageIndex < totalPages - 1);
        }

        /// <summary>
        /// Creates a populated inventory slot for an item.
        /// </summary>
        private void CreateItemSlot(InventoryEntry entry)
        {
            var slot = new VisualElement();
            slot.AddToClassList("inventory-slot");

            // Icon
            var icon = new VisualElement();
            icon.AddToClassList("inventory-slot-icon");
            if (entry.item.icon != null)
            {
                icon.style.backgroundImage = new StyleBackground(entry.item.icon);
            }
            slot.Add(icon);

            // Quantity label
            var quantityLabel = new Label(entry.quantity.ToString());
            quantityLabel.AddToClassList("inventory-slot-quantity");
            slot.Add(quantityLabel);

            // Click handler
            slot.RegisterCallback<ClickEvent>(evt =>
            {
                Debug.Log($"[Inventory] Slot clicked: {entry.item.itemId} ({entry.item.displayName}) x{entry.quantity}");
                OnSlotClicked(entry);
            });

            slotGrid.Add(slot);
        }

        /// <summary>
        /// Creates an empty placeholder slot to maintain grid shape.
        /// </summary>
        private void CreateEmptySlot()
        {
            var slot = new VisualElement();
            slot.AddToClassList("inventory-slot");
            slot.AddToClassList("inventory-slot-empty");
            slotGrid.Add(slot);
        }

        /// <summary>
        /// Virtual method called when a slot is clicked.
        /// Override this to integrate with placement or usage systems.
        /// </summary>
        protected virtual void OnSlotClicked(InventoryEntry entry)
        {
            // Hook for future placement logic or item usage
            // For example: PlacementManager.Instance.BeginPlacement(entry.item);
        }
    }
}