using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

namespace PhotoShare.Core
{
    public class ToastSystem
    {
        private VisualElement root;
        private VisualElement toastContainer;
        private Label toastLabel;
        private MonoBehaviour coroutineRunner;
        private Coroutine currentToast;

        public ToastSystem(VisualElement rootElement, MonoBehaviour runner)
        {
            root = rootElement;
            coroutineRunner = runner;
            QueryElements();
        }

        private void QueryElements()
        {
            toastContainer = root.Q<VisualElement>("toast-container");
            toastLabel = root.Q<Label>("toast-label");

            if (toastContainer == null)
                Debug.LogError("toast-container not found in UXML!");
            if (toastLabel == null)
                Debug.LogError("toast-label not found in UXML!");
        }

        public void ShowToast(string message, float duration)
        {
            if (toastLabel == null || toastContainer == null) return;

            if (currentToast != null)
                coroutineRunner.StopCoroutine(currentToast);

            toastLabel.text = message;
            currentToast = coroutineRunner.StartCoroutine(ToastRoutine(duration));
        }

        private IEnumerator ToastRoutine(float duration)
        {
            // Fade in
            toastContainer.style.display = DisplayStyle.Flex;
            float fadeInTime = 0.25f;
            float elapsed = 0;

            while (elapsed < fadeInTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeInTime;
                toastContainer.style.opacity = t;
                toastContainer.style.translate = new Translate(0, Length.Percent((1 - t) * -10));
                yield return null;
            }

            toastContainer.style.opacity = 1;
            toastContainer.style.translate = new Translate(0, 0);

            // Hold
            yield return new WaitForSeconds(duration);

            // Fade out
            float fadeOutTime = 0.3f;
            elapsed = 0;

            while (elapsed < fadeOutTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeOutTime;
                toastContainer.style.opacity = 1 - t;
                toastContainer.style.translate = new Translate(0, Length.Percent(-t * 10));
                yield return null;
            }

            toastContainer.style.display = DisplayStyle.None;
            toastContainer.style.opacity = 0;
            currentToast = null;
        }
    }
}