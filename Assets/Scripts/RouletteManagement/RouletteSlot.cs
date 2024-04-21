using System.Collections;
using DG.Tweening;
using Promises;
using RewardItemManagement;
using UnityEngine;
using UnityEngine.UI;

namespace RouletteManagement
{
    public class RouletteSlot : MonoBehaviour
    {
        [SerializeField]
        private Image BackgroundImg;

        [SerializeField]
        private Image RewardImg;
        [SerializeField]
        private Image GlowImg;

        [SerializeField]
        private Image SelectedTickImg;
        [SerializeField]
        private Image SelectedImg;

        [SerializeField]
        private Sprite GrayVersion;

        [HideInInspector]
        public RewardItem Item;

        private Promise<Sprite> m_IconPromise;

        private GeneralSettings m_Settings;

        private void Awake()
        {
            m_Settings = GeneralSettings.Get();
        }

        public Promise<Sprite> AssignItem(RewardItem item)
        {
            Item = item;
            var itemRuntimeData = item.RuntimeData();
            m_IconPromise = Item.RuntimeData().GetIcon();
            m_IconPromise.OnResultT += (success, sprite) =>
            {
                if (!success)
                    return;

                RewardImg.sprite = sprite;
            };

            return m_IconPromise;
        }

        public void Glow(float duration)
        {
            GlowImg.DOColor(Color.yellow, duration / 2f).OnComplete(() => { GlowImg.DOColor(Color.clear, duration); });
        }

        public IEnumerator GlowForSelected(float duration)
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(duration);
            
            Glow(duration);
            yield return waitForSeconds;
            Glow(duration);
            yield return waitForSeconds;
            Glow(duration);
        }

        public IEnumerator OnSelected(float duration)
        {
            SelectedImg.DOColor(Color.white, duration);
            SelectedTickImg.DOFillAmount(1, duration);

            yield return new WaitForSeconds(m_Settings.ParticleAnimationDuration);

            SetToUsed(0.1f);
        }

        private void SetToUsed(float duration)
        {
            BackgroundImg.sprite = GrayVersion;

            SelectedImg.DOColor(Color.clear, duration);
            RewardImg.DOColor(Color.clear, duration);
        }
    }
}