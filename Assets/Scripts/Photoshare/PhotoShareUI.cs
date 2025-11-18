using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;

namespace PhotoShare.Core
{
    public enum ShareOption
    {
        WhatsApp,
        Messenger,
        CopyLink,
        SaveToGallery
    }

    public class PhotoShareUI
    {
        public event Action OnShareButtonClicked;
        public event Action OnSaveButtonClicked;
        public event Action OnCloseButtonClicked;
        public event Action<ShareOption> OnShareOptionSelected;

        private VisualElement root;
        private VisualElement modalOverlay;
        private VisualElement modalContainer;
        private VisualElement photoPreview;
        private Button shareButton;
        private Button saveButton;
        private Button closeButton;
        private VisualElement shareSheet;
        private Button shareWhatsApp;
        private Button shareMessenger;
        private Button shareCopyLink;
        private Button shareSave;
        private Button shareCancel;
        private MonoBehaviour coroutineRunner;

        public PhotoShareUI(VisualElement rootElement, MonoBehaviour runner)
        {
            root = rootElement;
            coroutineRunner = runner;
            QueryElements();
            SetupEventHandlers();
        }

        private void QueryElements()
        {
            modalOverlay = root.Q<VisualElement>("share-overlay");
            modalContainer = root.Q<VisualElement>("share-modal-container");
            photoPreview = root.Q<VisualElement>("photo-preview");
            shareButton = root.Q<Button>("share-button");
            saveButton = root.Q<Button>("save-button");
            closeButton = root.Q<Button>("close-button");
            shareSheet = root.Q<VisualElement>("share-sheet");
            shareWhatsApp = root.Q<Button>("share-whatsapp");
            shareMessenger = root.Q<Button>("share-messenger");
            shareCopyLink = root.Q<Button>("share-copylink");
            shareSave = root.Q<Button>("share-save");
            shareCancel = root.Q<Button>("share-cancel");

            if (modalOverlay == null) Debug.LogError("share-overlay not found!");
            if (modalContainer == null) Debug.LogError("share-modal-container not found!");
            if (photoPreview == null) Debug.LogError("photo-preview not found!");
            if (shareButton == null) Debug.LogError("share-button not found!");
        }

        private void SetupEventHandlers()
        {
            shareButton?.RegisterCallback<ClickEvent>(evt => OnShareButtonClicked?.Invoke());
            saveButton?.RegisterCallback<ClickEvent>(evt => OnSaveButtonClicked?.Invoke());
            closeButton?.RegisterCallback<ClickEvent>(evt => OnCloseButtonClicked?.Invoke());
            shareWhatsApp?.RegisterCallback<ClickEvent>(evt => OnShareOptionSelected?.Invoke(ShareOption.WhatsApp));
            shareMessenger?.RegisterCallback<ClickEvent>(evt => OnShareOptionSelected?.Invoke(ShareOption.Messenger));
            shareCopyLink?.RegisterCallback<ClickEvent>(evt => OnShareOptionSelected?.Invoke(ShareOption.CopyLink));
            shareSave?.RegisterCallback<ClickEvent>(evt => OnShareOptionSelected?.Invoke(ShareOption.SaveToGallery));
            shareCancel?.RegisterCallback<ClickEvent>(evt => HideShareSheet());
        }

        public void Show(Texture2D screenshot, float duration)
        {
            if (photoPreview != null)
                photoPreview.style.backgroundImage = screenshot;

            if (modalOverlay != null)
            {
                modalOverlay.style.display = DisplayStyle.Flex;
                modalOverlay.style.visibility = Visibility.Visible;
            }

            if (modalContainer != null)
            {
                modalContainer.style.display = DisplayStyle.Flex;
                modalContainer.style.visibility = Visibility.Visible; 
            }

            if (coroutineRunner != null)
                coroutineRunner.StartCoroutine(AnimateModalIn(duration));
        }

        public void Hide()
        {
            if (modalOverlay != null)
            {
                modalOverlay.style.display = DisplayStyle.None;
                modalOverlay.style.visibility = Visibility.Hidden;
            }
            if (modalContainer != null)
            {
                modalContainer.style.display = DisplayStyle.None;
                modalContainer.style.visibility = Visibility.Hidden;
            }
            HideShareSheet();
        }

        public void ShowShareSheet()
        {
            if (shareSheet != null)
            {
                shareSheet.style.display = DisplayStyle.Flex;
                shareSheet.style.visibility = Visibility.Visible;

                if (coroutineRunner != null)
                    coroutineRunner.StartCoroutine(AnimateShareSheetIn());
            }
        }

        public void HideShareSheet()
        {
            if (shareSheet != null)
            {
                shareSheet.style.display =  DisplayStyle.None;
                shareSheet.style.visibility = Visibility.Hidden;
            }
        }

        private IEnumerator AnimateModalIn(float duration)
        {
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float eased = 1 - Mathf.Pow(1 - t, 3);

                if (modalOverlay != null)
                    modalOverlay.style.opacity = eased;
                if (modalContainer != null)
                {
                    modalContainer.style.opacity = eased;
                    modalContainer.style.scale = new StyleScale(new Scale(Vector3.Lerp(new Vector3(0.8f, 0.8f, 1), Vector3.one, eased)));
                }
                yield return null;
            }

            if (modalOverlay != null) modalOverlay.style.opacity = 1;
            if (modalContainer != null)
            {
                modalContainer.style.opacity = 1;
                modalContainer.style.scale = new Scale(Vector3.one);
            }
            Debug.Log("Animation complete");
        }

        private IEnumerator AnimateShareSheetIn()
        {
            float duration = 0.25f;
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float eased = 1 - Mathf.Pow(1 - t, 3);
                if (shareSheet != null)
                {
                    shareSheet.style.opacity = eased;
                    shareSheet.style.translate = new Translate(0, Length.Percent((1 - eased) * 20));
                }
                yield return null;
            }
            if (shareSheet != null)
            {
                shareSheet.style.opacity = 1;
                shareSheet.style.translate = new Translate(0, 0);
            }
        }

        public void Cleanup()
        {
            shareButton?.UnregisterCallback<ClickEvent>(evt => OnShareButtonClicked?.Invoke());
            saveButton?.UnregisterCallback<ClickEvent>(evt => OnSaveButtonClicked?.Invoke());
            closeButton?.UnregisterCallback<ClickEvent>(evt => OnCloseButtonClicked?.Invoke());
            shareWhatsApp?.UnregisterCallback<ClickEvent>(evt => OnShareOptionSelected?.Invoke(ShareOption.WhatsApp));
            shareMessenger?.UnregisterCallback<ClickEvent>(evt => OnShareOptionSelected?.Invoke(ShareOption.Messenger));
            shareCopyLink?.UnregisterCallback<ClickEvent>(evt => OnShareOptionSelected?.Invoke(ShareOption.CopyLink));
            shareSave?.UnregisterCallback<ClickEvent>(evt => OnShareOptionSelected?.Invoke(ShareOption.SaveToGallery));
            shareCancel?.UnregisterCallback<ClickEvent>(evt => HideShareSheet());
        }
    }
}