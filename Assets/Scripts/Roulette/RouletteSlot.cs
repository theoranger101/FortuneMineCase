using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Roulette
{
    public class RouletteSlot : MonoBehaviour
    {
        public Image BackgroundImg;
        
        public Image RewardImg;
        public Image GlowImg;

        public Image SelectedTickImg;
        public Image SelectedImg;

        public Sprite GrayVersion;

        [HideInInspector]
        public RewardItem Item;

        public void AssignItem(RewardItem item)
        {
            if (item.Sprite.Asset != null)
            {
                RewardImg.sprite = (Sprite)item.Sprite.Asset;

                return;
            }

            if (item.Sprite.OperationHandle.IsValid())
            {
                item.Sprite.OperationHandle.Completed += handle => { RewardImg.sprite = (Sprite)handle.Result; };

                return;
            }

            var assetName = $"Assets/Sprites/Atlases/{item.Sprite.editorAsset.name}.spriteatlas[{item.name}]";
            var opHandle = Addressables.LoadAssetAsync<Sprite>(assetName);

            opHandle.Completed += handle =>
            {
                if (handle.Result != null)
                {
                    RewardImg.sprite = handle.Result;
                }
            };
        }

        private void OnUnloadScene()
        {
            Item.Sprite.ReleaseAsset();
        }

        public void GlowAnim(float duration)
        {
            GlowImg.DOColor(Color.yellow, duration / 2f).OnComplete(() => { GlowImg.DOColor(Color.clear, duration); });
        }

        public IEnumerator OnSelected(float duration)
        {
            SelectedImg.DOColor(Color.white, duration);
            SelectedTickImg.DOFillAmount(1, duration);

            yield return new WaitForSeconds(1f);
            // play food move anim

            SetToUsed(0.1f);
        }

        public void SetToUsed(float duration)
        {
            BackgroundImg.sprite = GrayVersion;
            
            SelectedImg.DOColor(Color.clear, duration);
            RewardImg.DOColor(Color.clear, duration);
        }
    }
}