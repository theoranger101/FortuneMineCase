using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Events;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
using WalletManagement;

namespace Roulette
{
    public class RouletteController : MonoBehaviour
    {
        public RewardItemCollection ItemCollection;
        public List<RouletteSlot> RouletteSlots = new List<RouletteSlot>();

        public float GlowAnimationDuration;
        public float GlowForSelectedDuration;

// #if UNITY_EDITOR
//         private void OnValidate()
//         {
//             RouletteSlots = RouletteSlots.OrderBy(slot => int.Parse(Regex.Match(slot.name, @"\d+").Value)).ToList();
//         }
// #endif

        private void Start()
        {
            Setup();
        }

        [Button]
        public void Setup()
        {
            var slotCount = RouletteSlots.Count;

            for (var i = 0; i < slotCount; i++)
            {
                var selectedItem = ItemCollection.Items.RandomElement();
                RouletteSlots[i].AssignItem(selectedItem);
            }

            GEM.AddListener<RouletteEvent>(OnSpinTheRoulette, channel: (int)RouletteEventType.Spin);
        }

        private void OnSpinTheRoulette(RouletteEvent evt)
        {
            var randomSlot = RouletteSlots.RandomElement();
            StartCoroutine(GlowAnim(randomSlot));
        }

        // [Button]
        // public void OnStartRoulette()
        // {
        //     var randomSlot = RouletteSlots.RandomElement();
        //     Debug.Log($"selected {randomSlot.RewardImg.sprite.name}");
        //     StartCoroutine(LightUpSlots(randomSlot));
        // }

        private IEnumerator GlowAnim(RouletteSlot selectedSlot)
        {
            foreach (var rouletteSlot in RouletteSlots)
            {
                rouletteSlot.GlowAnim(GlowAnimationDuration);
                yield return new WaitForSeconds(GlowAnimationDuration / 4f);
            }

            foreach (var rouletteSlot in RouletteSlots)
            {
                rouletteSlot.GlowAnim(GlowAnimationDuration);
                yield return new WaitForSeconds(GlowAnimationDuration / 4f);
            }

            foreach (var rouletteSlot in RouletteSlots)
            {
                if (selectedSlot == rouletteSlot)
                {
                    rouletteSlot.GlowAnim(GlowForSelectedDuration);
                    yield return new WaitForSeconds(GlowForSelectedDuration);
                    rouletteSlot.GlowAnim(GlowForSelectedDuration);
                    yield return new WaitForSeconds(GlowForSelectedDuration);
                    rouletteSlot.GlowAnim(GlowForSelectedDuration);

                    StartCoroutine(rouletteSlot.OnSelected(GlowForSelectedDuration));
                    
                    AddToWallet(rouletteSlot.Item);

                    yield break;
                }

                rouletteSlot.GlowAnim(GlowAnimationDuration);
                yield return new WaitForSeconds(GlowAnimationDuration / 1.5f);
            }
        }

        private void AddToWallet(RewardItem item)
        {
            using var evt = WalletEvent.Get(item, 1).SendGlobal((int)WalletEventType.Earn);
        }
    }
}