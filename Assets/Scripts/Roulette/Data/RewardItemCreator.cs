using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;

namespace Roulette
{
    [CreateAssetMenu]
    public class RewardItemCreator : ScriptableObject
    {
        public const string RootFolder = "Assets/Sprites/Rewards";
        public string Path;

        [Button]
        public void CreateItems()
        {
            var fullPath = $"{RootFolder}/{Path}";
            var directoryInfo = new DirectoryInfo(fullPath);
            var items = directoryInfo.GetFiles("*.png");

            Directory.CreateDirectory($"Assets/GameData/Rewards/{Path}");

            var newItemCollection = CreateInstance<RewardItemCollection>();

            foreach (var item in items)
            {
                var newItemData = CreateInstance<RewardItem>();
                newItemData.Sprite =
                    new AssetReferenceAtlasedSprite(AssetDatabase.AssetPathToGUID($"{fullPath}/{item.Name}"));

                newItemData.name = item.Name;
                var croppedName = item.Name.Split(".png")[0];

                AssetDatabase.CreateAsset(newItemData, $"Assets/GameData/Rewards/{Path}/{croppedName}.asset");
                AssetDatabase.SaveAssets();

                newItemCollection.Items.Add(newItemData);
            }

            AssetDatabase.CreateAsset(newItemCollection, $"Assets/GameData/Rewards/{Path}Collection.asset");
        }

        public const string RootAtlasFolder = "Assets/Sprites/Atlases/Rewards";

        [Button]
        public void CreateAtlasItems()
        {
            var directoryInfo = new DirectoryInfo(RootAtlasFolder);
            var items = directoryInfo.GetFiles("*.spriteatlas");

            for (var i = items.Length - 1; i >= 0; i--)
            {
                var atlasPath = $"{RootAtlasFolder}/{items[i].Name}";
                var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);

                var atlasName = items[i].Name.Split("Atlas_")[1];
                atlasName = atlasName.Split(".spriteatlas")[0];
                var directoryName = $"Assets/GameData/Rewards/{atlasName}";
                Directory.CreateDirectory(directoryName);

                var newItemCollection = CreateInstance<RewardItemCollection>();

                var sprites = new Sprite[atlas.spriteCount];
                atlas.GetSprites(sprites);

                for (var k = 0; k < sprites.Length; k++)
                {
                    var newItemData = CreateInstance<RewardItem>();
                    newItemData.Sprite =
                        new AssetReference(
                            AssetDatabase.AssetPathToGUID($"{atlasPath}"));

                    newItemData.Sprite.SetEditorSubObject(sprites[k]);
                    newItemData.Sprite.SubObjectName = sprites[k].name;

                    Debug.Log($"{atlasPath}/{sprites[k].name}");

                    var croppedName = sprites[k].name.Split("(Clone)")[0];
                    newItemData.name = croppedName;

                    AssetDatabase.CreateAsset(newItemData, $"{directoryName}/{newItemData.name}.asset");
                    AssetDatabase.SaveAssets();

                    newItemCollection.Items.Add(newItemData);
                }

                AssetDatabase.CreateAsset(newItemCollection, $"Assets/GameData/Rewards/{atlasName}Collection.asset");
            }
        }
    }
}