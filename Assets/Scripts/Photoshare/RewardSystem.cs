using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;

namespace PhotoShare.Rewards
{
    [Serializable]
    public class RewardData
    {
        public string rewardName = "Photo Share Bonus";
        public int goldAmount = 100;
        public int gemsAmount = 10;
        public Sprite rewardIcon;
        public string description = "Thanks for sharing!";
    }

    public class RewardSystem
    {
        public event Action<RewardData> OnRewardClaimed;
        public event Action OnRewardPopupClosed;

        private VisualElement root;

        private VisualElement rewardOverlay;
        private VisualElement rewardPopup;
        private Label rewardTitle;
        private VisualElement rewardIcon;
        private Label rewardIconEmoji;
        private Label rewardDescription;
        private Label goldAmount;
        private Label gemsAmount;
        private Button claimButton;

        private MonoBehaviour coroutineRunner;
        private RewardData currentReward;
        private bool isShowing;

        public RewardSystem(VisualElement rootElement, MonoBehaviour runner)
        {
            root = rootElement;
            coroutineRunner = runner;
            QueryElements();
            SetupEventHandlers();
        }

        private void QueryElements()
        {
            rewardOverlay = root.Q<VisualElement>("reward-overlay");
            rewardPopup = root.Q<VisualElement>("reward-popup");
            rewardTitle = root.Q<Label>("reward-title");
            rewardIcon = root.Q<VisualElement>("reward-icon");
            rewardIconEmoji = root.Q<Label>("reward-icon-emoji");
            rewardDescription = root.Q<Label>("reward-description");
            goldAmount = root.Q<Label>("gold-amount");
            gemsAmount = root.Q<Label>("gems-amount");
            claimButton = root.Q<Button>("claim-button");

            // Validate
            if (rewardOverlay == null) Debug.LogError("reward-overlay not found in UXML!");
            if (rewardPopup == null) Debug.LogError("reward-popup not found in UXML!");
            if (claimButton == null) Debug.LogError("claim-button not found in UXML!");
        }

        private void SetupEventHandlers()
        {
            claimButton?.RegisterCallback<ClickEvent>(evt => HandleClaimClicked());
        }

        public void ShowReward(RewardData reward)
        {
            if (isShowing) return;

            currentReward = reward;
            isShowing = true;

            if (rewardTitle != null)
                rewardTitle.text = reward.rewardName;
            if (rewardDescription != null)
                rewardDescription.text = reward.description;
            if (goldAmount != null)
                goldAmount.text = $"+{reward.goldAmount}";
            if (gemsAmount != null)
                gemsAmount.text = $"+{reward.gemsAmount}";

            if (reward.rewardIcon != null && rewardIcon != null)
            {
                rewardIcon.style.backgroundImage = new StyleBackground(reward.rewardIcon);

                if (rewardIconEmoji != null)
                    rewardIconEmoji.style.display = DisplayStyle.None;
            }
            else
            {
                if (rewardIconEmoji != null)
                    rewardIconEmoji.style.display = DisplayStyle.Flex;
            }

            // Show with animation
            if (coroutineRunner != null)
                coroutineRunner.StartCoroutine(AnimateRewardIn());
        }

        private void HandleClaimClicked()
        {
            if (!isShowing) return;

            claimButton?.SetEnabled(false);

            OnRewardClaimed?.Invoke(currentReward);

            if (coroutineRunner != null)
                coroutineRunner.StartCoroutine(AnimateRewardOut());
        }

        private IEnumerator AnimateRewardIn()
        {
            if (rewardOverlay != null)
                rewardOverlay.style.display = DisplayStyle.Flex;
            if (rewardPopup != null)
                rewardPopup.style.display = DisplayStyle.Flex;

            float duration = 0.4f;
            float elapsed = 0;

            // Start values
            if (rewardOverlay != null) rewardOverlay.style.opacity = 0;
            if (rewardPopup != null)
            {
                rewardPopup.style.opacity = 0;
                rewardPopup.style.scale = new Scale(new Vector3(0.5f, 0.5f, 1));
            }

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Ease out back 
                float overshoot = 1.70158f;
                float eased = 1 + (overshoot + 1) * Mathf.Pow(t - 1, 3) + overshoot * Mathf.Pow(t - 1, 2);

                if (rewardOverlay != null)
                    rewardOverlay.style.opacity = t;
                if (rewardPopup != null)
                {
                    rewardPopup.style.opacity = t;
                    rewardPopup.style.scale = new Scale(Vector3.Lerp(new Vector3(0.5f, 0.5f, 1),Vector3.one, eased ));
                }

                yield return null;
            }

            if (rewardOverlay != null) rewardOverlay.style.opacity = 1;
            if (rewardPopup != null)
            {
                rewardPopup.style.opacity = 1;
                rewardPopup.style.scale = new Scale(Vector3.one);
            }

            // Animate currency numbers
            if (coroutineRunner != null)
                yield return coroutineRunner.StartCoroutine(AnimateCurrencyNumbers());
        }

        private IEnumerator AnimateCurrencyNumbers()
        {
            float duration = 0.6f;
            float elapsed = 0;

            int startGold = 0;
            int startGems = 0;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Ease out
                float eased = 1 - Mathf.Pow(1 - t, 3);

                int currentGold = Mathf.RoundToInt(Mathf.Lerp(startGold, currentReward.goldAmount, eased));
                int currentGems = Mathf.RoundToInt(Mathf.Lerp(startGems, currentReward.gemsAmount, eased));

                if (goldAmount != null)
                    goldAmount.text = $"+{currentGold}";
                if (gemsAmount != null)
                    gemsAmount.text = $"+{currentGems}";

                yield return null;
            }

            if (goldAmount != null)
                goldAmount.text = $"+{currentReward.goldAmount}";
            if (gemsAmount != null)
                gemsAmount.text = $"+{currentReward.gemsAmount}";
        }

        private IEnumerator AnimateRewardOut()
        {
            float duration = 0.3f;
            float elapsed = 0;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                if (rewardOverlay != null)
                    rewardOverlay.style.opacity = 1 - t;
                if (rewardPopup != null)
                {
                    rewardPopup.style.opacity = 1 - t;
                    rewardPopup.style.scale = new Scale(Vector3.Lerp(Vector3.one, new Vector3(0.8f, 0.8f, 1), t));
                }

                yield return null;
            }

            // Hide
            if (rewardOverlay != null)
                rewardOverlay.style.display = DisplayStyle.None;
            if (rewardPopup != null)
                rewardPopup.style.display = DisplayStyle.None;

            isShowing = false;

            // Reset icon
            if (rewardIconEmoji != null)
                rewardIconEmoji.style.display = DisplayStyle.Flex;
            if (rewardIcon != null)
                rewardIcon.style.backgroundImage = null;

            // Notify
            OnRewardPopupClosed?.Invoke();

            claimButton?.SetEnabled(true);
        }

        public void Cleanup()
        {
            claimButton?.UnregisterCallback<ClickEvent>(evt => HandleClaimClicked());
        }
    }
}