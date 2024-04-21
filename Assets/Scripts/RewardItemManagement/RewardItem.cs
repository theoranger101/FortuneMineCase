using Unity.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RewardItemManagement
{
    public class RewardItem : ScriptableObject
    {
        [ReadOnly]
        public int Id;

        [ReadOnly]
        public string Name;

        // for the sprite atlas to work properly, i cannot use AssetReferenceT<Sprite> here.
        // AssetReferencedAtlasedSprite is not viable either, since I want to be able to reference
        // from the script using the RewardItemCreator class.
        public AssetReference Sprite;

        public string SpritePath;
        
        public AssetReferenceT<Material> Material;
    }
}