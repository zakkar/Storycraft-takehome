using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace PhotoShare.Testing
{
    [RequireComponent(typeof(Core.PhotoShareController))]
    public class ScreenshotController : MonoBehaviour
    {
        [SerializeField] private UIDocument photoshareUI;
        [SerializeField] private Button screenshotBtn;

        private Core.PhotoShareController controller;

        private void Awake()
        {
            controller = GetComponent<Core.PhotoShareController>();

            if(photoshareUI != null)
            {
                screenshotBtn = photoshareUI.rootVisualElement.Q<Button>("screenshot-button");
                screenshotBtn.RegisterCallback<ClickEvent>(evt => CaptureAndShare());

                // Debug purpose
                screenshotBtn.RegisterCallback<PointerDownEvent>(evt => Debug.Log("Pointer down on screenshot button"));
                screenshotBtn.RegisterCallback<PointerUpEvent>(evt => Debug.Log("Pointer up on screenshot button"));
            }
        }

        [ContextMenu("Capture Real Screenshot")]
        public void CaptureAndShare()
        {
            StartCoroutine(CaptureScreenshotCoroutine());
        }

        private System.Collections.IEnumerator CaptureScreenshotCoroutine()
        {
            // Remove the UI before the screenshot (can be a design decision)
            StyleEnum<DisplayStyle> previousDisplay = screenshotBtn.style.display;
            screenshotBtn.style.display = DisplayStyle.None;

            // Wait for end of frame to capture
            yield return new WaitForEndOfFrame();

            Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenshot.Apply();

            screenshotBtn.style.display = previousDisplay;

            controller.ShowShareUI(screenshot);
        }

        private void OnDisable()
        {
            if (screenshotBtn != null)
            {
                screenshotBtn.UnregisterCallback<ClickEvent>(evt => CaptureAndShare());
            }
        }
    }
}