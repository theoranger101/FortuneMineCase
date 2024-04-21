using System;
using Conditionals;
using Promises;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace RewardItemManagement
{
    public class RewardItemRuntimeData : IDisposable
    {
        public RewardItem RewardItem;

        private AsyncOperationHandle IconHandle;
        private Sprite Icon;

        private AsyncOperationHandle MaterialHandle;
        private Material Material;

        public void Initialize(RewardItem item)
        {
            RewardItem = item;
        }

        public Promise<Sprite> GetIcon()
        {
            Promise<Sprite> promise = Promise<Sprite>.Create();

            if (Icon != null)
            {
                Conditional.WaitFrames(1)
                    .Do(() => { promise.Complete(Icon); });

                return promise;
            }

            if (IconHandle.IsValid())
            {
                IconHandle.Completed += handle =>
                {
                    if (handle.Result == null)
                    {
                        promise.Fail();
                    }

                    Icon = (Sprite)handle.Result;
                    promise.Complete(Icon);
                };

                return promise;
            }

            var assetName = RewardItem.SpritePath;
            IconHandle = Addressables.LoadAssetAsync<Sprite>(assetName);

            IconHandle.Completed += handle =>
            {
                if (handle.Result == null)
                {
                    promise.Fail();
                }

                Icon = (Sprite)handle.Result;
                promise.Complete(Icon);
            };

            return promise;
        }

        public Promise<Material> GetMaterial()
        {
            Promise<Material> promise = Promise<Material>.Create();

            if (Material != null)
            {
                Conditional.WaitFrames(1)
                    .Do(() => { promise.Complete(Material); });

                return promise;
            }

            if (MaterialHandle.IsValid())
            {
                MaterialHandle.Completed += handle =>
                {
                    if (handle.Result == null)
                    {
                        promise.Fail();
                    }

                    promise.Complete((Material)handle.Result);
                };

                return promise;
            }

            MaterialHandle = RewardItem.Material.LoadAssetAsync<Material>();

            MaterialHandle.Completed += handle =>
            {
                if (handle.Result == null)
                {
                    promise.Fail();
                }

                Material = (Material)MaterialHandle.Result;
                promise.Complete(Material);
            };

            return promise;
        }

        private void Release()
        {
            RewardItem.Material.ReleaseAsset();
            RewardItem.Sprite.ReleaseAsset();
        }

        public void Dispose()
        {
            Release();
        }
    }
}