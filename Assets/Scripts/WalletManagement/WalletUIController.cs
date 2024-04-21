using System;
using Events;
using Promises;
using RewardItemManagement;
using RouletteManagement;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace WalletManagement
{
    public class WalletUIController : MonoBehaviour
    {
        public ParticleMoveToTargetAnimation ParticleAnim;

        public Button WalletIconButton;

        public Transform WalletIconT;
        public ParticleSystem WalletRewardPs;
        public Camera TargetCamera;

        private ParticleSystemRenderer m_PsRenderer;

        private float AnimDelay => m_Settings.ParticleAnimationDelay;
        private float AnimDuration => m_Settings.ParticleAnimationDuration;

        private Promise<bool> m_ParticleAnimPromise;

        private GeneralSettings m_Settings;

        private void Start()
        {
            m_Settings = GeneralSettings.Get();

            m_PsRenderer = WalletRewardPs.GetComponent<ParticleSystemRenderer>();

            ParticleAnim.Initialize(WalletIconT, WalletRewardPs, TargetCamera);

            GEM.AddListener<WalletEvent>(OnAddToWallet, channel: (int)WalletEventType.Earn);
            WalletIconButton.onClick.AddListener(OnWalletPopup);
        }

        private void OnDisable()
        {
            GEM.RemoveListener<WalletEvent>(OnAddToWallet, channel: (int)WalletEventType.Earn);
            WalletIconButton.onClick.RemoveListener(OnWalletPopup);
        }

        private void OnAddToWallet(WalletEvent evt)
        {
            var item = evt.RewardItem;
            var promise = item.RuntimeData().GetMaterial();

            promise.OnResultT += (success, material) =>
            {
                if (!success)
                {
                    throw new Exception($"Material for {item.name} could not be loaded.");
                }

                m_PsRenderer.sharedMaterial = material;
                m_ParticleAnimPromise = ParticleAnim.OnPlay(AnimDelay, AnimDuration);
                m_ParticleAnimPromise.OnResultT += (b1, b2) =>
                {
                    using var rouletteEvt = RouletteEvent.Get().SendGlobal((int)RouletteEventType.SpinSequenceEnd);
                };
            };
        }

        private void OnWalletPopup()
        {
            using var walletEvt = WalletEvent.Get().SendGlobal((int)WalletEventType.GetAllValues);
            using var evt = UIEvent.Get(walletEvt.AllValues).SendGlobal((int)UIEventType.WalletPopup);
        }
    }
}