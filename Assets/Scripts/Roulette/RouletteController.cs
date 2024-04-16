using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace Roulette
{
    public class RouletteController : MonoBehaviour
    {
        public RewardItemCollection ItemCollection;

        public List<RouletteSlot> RouletteSlots = new List<RouletteSlot>();

        [Button]
        public void Setup()
        {
            var slotCount = RouletteSlots.Count;

            for (var i = 0; i < slotCount; i++)
            {
                var selectedItem = ItemCollection.Items.RandomElement();
                RouletteSlots[i].AssignItem(selectedItem);
            }
        }

        [Button]
        public void OnStartRoulette()
        {
            var randomSlot = RouletteSlots.RandomElement();
            Debug.Log($"selected {randomSlot.RewardImage.sprite.name}");
            StartCoroutine(LightUpSlots(randomSlot));
        }

        private IEnumerator LightUpSlots(RouletteSlot selectedSlot)
        {
            foreach (var rouletteSlot in RouletteSlots)
            {
                rouletteSlot.LightUpAnimation();
                yield return new WaitForSeconds(0.25f);
            }
            
            foreach (var rouletteSlot in RouletteSlots)
            {
                rouletteSlot.LightUpAnimation();
                yield return new WaitForSeconds(0.25f);
            }
            
            foreach (var rouletteSlot in RouletteSlots)
            {
                rouletteSlot.LightUpAnimation();

                if (selectedSlot == rouletteSlot)
                {
                    // 
                    yield break;
                }
                
                yield return new WaitForSeconds(0.25f);
            }
        }
    }
}