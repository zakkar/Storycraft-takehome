using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class DebugDisplay : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            var uiDoc = GetComponent<UIDocument>();
            var root = uiDoc.rootVisualElement;

            Debug.Log("=== DISPLAY DEBUG ===");

            var overlay = root.Q<VisualElement>("share-overlay");
            if (overlay != null)
            {
                Debug.Log($"Overlay Display: {overlay.style.display.value}");
                Debug.Log($"Overlay Computed Display: {overlay.resolvedStyle.display}");
                Debug.Log($"Overlay Visibility: {overlay.style.visibility.value}");
                Debug.Log($"Overlay Opacity: {overlay.style.opacity.value}");
                Debug.Log($"Overlay Width: {overlay.resolvedStyle.width}");
                Debug.Log($"Overlay Height: {overlay.resolvedStyle.height}");

                overlay.style.display = DisplayStyle.Flex;
                overlay.style.visibility = Visibility.Visible;
                overlay.style.opacity = 1;
                Debug.Log("→ Forced overlay to show");
            }

            var modal = root.Q<VisualElement>("share-modal-container");
            if (modal != null)
            {
                Debug.Log($"Modal Display: {modal.style.display.value}");
                Debug.Log($"Modal Computed Display: {modal.resolvedStyle.display}");

                modal.style.display = DisplayStyle.Flex;
                modal.style.visibility = Visibility.Visible;
                modal.style.opacity = 1;
                Debug.Log("→ Forced modal to show");
            }
        }
    }
}