using System;
using RewardItemManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RewardItemWalletUI : MonoBehaviour
    {
        public Image IconImage;
        public TextMeshProUGUI AmountText;
        
        public void Setup(RewardItem item, int amount)
        {
            var promise = item.RuntimeData().GetIcon();
            promise.OnResultT += (success, sprite) =>
            {
                if (!success)
                {
                    throw new Exception("");
                }

                IconImage.sprite = sprite;
            };

            AmountText.text = amount.ToString();
        }
    }
}