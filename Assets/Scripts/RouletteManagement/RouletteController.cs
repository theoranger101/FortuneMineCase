using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Conditionals;
using Events;
using RewardItemManagement;
using SceneManagement;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using Utils;
using WalletManagement;

namespace RouletteManagement
{
    public class RouletteController : MonoBehaviour
    {
        public RewardItemCollection ItemCollection;
        public List<RouletteSlot> RouletteSlots = new List<RouletteSlot>();

        private int m_SlotSetupCount = 0;

        public float GlowDuration => m_Settings.GlowDuration;
        public float GlowForSelectedDuration => m_Settings.GlowForSelectedDuration;

        public int GlowRoundCount => m_Settings.GlowRoundCount;

        private GeneralSettings m_Settings;

        [Button]
        private void SortSlots()
        {
            RouletteSlots = RouletteSlots.OrderBy(slot => int.Parse(Regex.Match(slot.name, @"\d+").Value)).ToList();
        }

        private void Start()
        {
            m_Settings = GeneralSettings.Get();
            StartCoroutine(Setup());
        }

        private IEnumerator Setup()
        {
            var slotCount = RouletteSlots.Count;

            for (var i = 0; i < slotCount; i++)
            {
                var selectedItem = ItemCollection.Items.RandomElement();
                var promise = RouletteSlots[i].AssignItem(selectedItem);

                promise.OnResultT += (success, sprite) =>
                {
                    if (!success)
                        return;

                    OnSlotSetupCompleted();
                };

                yield return 0;
            }

            GEM.AddListener<RouletteEvent>(OnSpinTheRoulette, channel: (int)RouletteEventType.Spin);
        }

        private void OnSlotSetupCompleted()
        {
            m_SlotSetupCount++;

            if (m_SlotSetupCount == RouletteSlots.Count)
            {
                using var evt = RouletteEvent.Get(ItemCollection)
                    .SendGlobal((int)RouletteEventType.SetupComplete);
            }
        }

        private void OnSpinTheRoulette(RouletteEvent evt)
        {
            var randomSlot = RouletteSlots.RandomElement();
            StartCoroutine(GlowSequence(randomSlot));
        }

        private IEnumerator GlowSequence(RouletteSlot selectedSlot)
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(GlowDuration / 4);

            for (var i = 0; i < GlowRoundCount - 1; i++)
            {
                for (var k = 0; k < RouletteSlots.Count; k++)
                {
                    RouletteSlots[k].Glow(GlowDuration);
                    yield return waitForSeconds;
                }
            }

            waitForSeconds = new WaitForSeconds(GlowDuration / 1.5f); // change wait for seconds for faster round

            for (var i = 0; i < RouletteSlots.Count; i++)
            {
                if (selectedSlot == RouletteSlots[i])
                {
                    yield return StartCoroutine(RouletteSlots[i].GlowForSelected(GlowForSelectedDuration));
                    StartCoroutine(RouletteSlots[i].OnSelected(GlowForSelectedDuration));

                    AddToWallet(RouletteSlots[i].Item);

                    RouletteSlots.Remove(RouletteSlots[i]);
                    if (RouletteSlots.Count == 0)
                    {
                        OnAllSlotsSelected();
                    }

                    yield break;
                }

                RouletteSlots[i].Glow(GlowDuration);
                yield return waitForSeconds;
            }
        }

        private void AddToWallet(RewardItem item)
        {
            using var walletEvt = WalletEvent.Get(item, 1).SendGlobal((int)WalletEventType.Earn);
            using var popupEvt = UIEvent.Get(item).SendGlobal((int)UIEventType.RewardPopup);
        }

        [Button]
        private void OnAllSlotsSelected()
        {
            using var selectedEvt = RouletteEvent.Get().SendGlobal((int)RouletteEventType.AllSlotsSelected);

            Conditional.Wait(m_Settings.ParticleAnimationDelay + m_Settings.ParticleAnimationDuration)
                .Do(() =>
                {
                    using var evt = SceneEvent.Get(SceneId.Barbeque).SendGlobal((int)SceneEventType.Unload);
                });
        }
    }
}