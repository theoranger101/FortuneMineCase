using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Roulette
{
    public class RouletteSlot : MonoBehaviour
    {
        public Image LightUpImage;
        public Image RewardImage;
        
        public void AssignItem(RewardItemData itemData)
        {
            if (itemData.Sprite.Asset != null)
            {
                RewardImage.sprite = (Sprite)itemData.Sprite.Asset;

                return;
            }

            if (itemData.Sprite.OperationHandle.IsValid())
            {
                itemData.Sprite.OperationHandle.Completed += handle => { RewardImage.sprite = (Sprite)handle.Result; };

                return;
            }

            var opHandle = itemData.Sprite.LoadAssetAsync<Sprite>();

            opHandle.Completed += handle =>
            {
                if (handle.Result != null)
                {
                    RewardImage.sprite = handle.Result;
                }
            };
        }
        
        public void LightUpAnimation()
        {
            LightUpImage.DOColor(Color.yellow, 0.5f).OnComplete(() =>
            {
                LightUpImage.DOColor(Color.clear, 0.5f);
            });
        }
    }
}