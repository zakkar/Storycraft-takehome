using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;

namespace PhotoShare.Core
{
    public class PhotoShareController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private UIDocument uiDocument;

        [Header("Animation Settings")]
        [SerializeField] private float modalFadeInDuration = 0.3f;
        [SerializeField] private float toastDuration = 2.5f;

        [Header("Reward Settings")]
        [SerializeField] private bool enableRewards = true;
        [SerializeField]
        private Rewards.RewardData shareReward = new Rewards.RewardData
        {
            rewardName = "Photo Share Bonus",
            goldAmount = 100,
            gemsAmount = 10,
            description = "Thanks for sharing your adventure!"
        };

        private VisualElement root;
        private PhotoShareUI shareUI;
        private ToastSystem toastSystem;
        private Rewards.RewardSystem rewardSystem;

        private void Awake()
        {
            if (uiDocument == null)
                uiDocument = GetComponent<UIDocument>();

            InitializeUI();
        }

        private void InitializeUI()
        {

            root = uiDocument.rootVisualElement;

            shareUI = new PhotoShareUI(root, this);
            toastSystem = new ToastSystem(root, this);
            rewardSystem = new Rewards.RewardSystem(root, this);

            shareUI.OnShareButtonClicked += HandleShareClicked;
            shareUI.OnSaveButtonClicked += HandleSaveClicked;
            shareUI.OnCloseButtonClicked += HandleCloseClicked;
            shareUI.OnShareOptionSelected += HandleShareOptionSelected;

            rewardSystem.OnRewardClaimed += HandleRewardClaimed;
            rewardSystem.OnRewardPopupClosed += HandleRewardClosed;

            Debug.Log("Photo Share UI initialized from UXML!");
        }


        public void ShowShareUI(Texture2D screenshot = null)
        {
            shareUI.Show(screenshot, modalFadeInDuration);
        }

        private void HandleShareClicked()
        {
            shareUI.ShowShareSheet();
        }

        private void HandleSaveClicked()
        {
            StartCoroutine(SimulateSaveOperation());
        }

        private void HandleCloseClicked()
        {
            shareUI.Hide();
        }

        private void HandleShareOptionSelected(ShareOption option)
        {
            shareUI.HideShareSheet();

            string message = "Shared successfully";
            switch (option)
            {
                case ShareOption.WhatsApp : message = "Shared to WhatsApp (mock)"; break;
                case ShareOption.Messenger: message = "Shared to Messenger (mock)"; break; 
                case ShareOption.CopyLink: message = "Link copied to clipboard"; break;
                case ShareOption.SaveToGallery: message = "Saved to gallery (mock)"; break;
                default: message = "Shared successfully"; break;
            };

            toastSystem.ShowToast(message, toastDuration);

            // Close share UI and show reward
            shareUI.Hide();

            if (enableRewards)
            {
                StartCoroutine(ShowRewardAfterDelay(0.5f));
            }
        }

        private IEnumerator ShowRewardAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            rewardSystem.ShowReward(shareReward);
        }

        private void HandleRewardClaimed(Rewards.RewardData reward)
        {
            Debug.Log($"Reward claimed: {reward.goldAmount} gold, {reward.gemsAmount} gems");

            // PlayerInventory.AddGold(reward.goldAmount);

            toastSystem.ShowToast($"Received {reward.goldAmount} gold and {reward.gemsAmount} gems!", toastDuration);
        }

        private void HandleRewardClosed()
        {
            Debug.Log("Player returned to game after claiming reward");
        }

        private IEnumerator SimulateSaveOperation()
        {
            yield return new WaitForSeconds(0.5f);
            toastSystem.ShowToast("Photo saved successfully", toastDuration);
        }

        private void OnDestroy()
        {
            if (shareUI != null)
            {
                shareUI.OnShareButtonClicked -= HandleShareClicked;
                shareUI.OnSaveButtonClicked -= HandleSaveClicked;
                shareUI.OnCloseButtonClicked -= HandleCloseClicked;
                shareUI.OnShareOptionSelected -= HandleShareOptionSelected;
                shareUI.Cleanup();
            }

            if (rewardSystem != null)
            {
                rewardSystem.OnRewardClaimed -= HandleRewardClaimed;
                rewardSystem.OnRewardPopupClosed -= HandleRewardClosed;
                rewardSystem.Cleanup();
            }
        }

        // Debug purpose
        [ContextMenu("Test Share")]
        public void TriggerShare() => ShowShareUI();
    }
}