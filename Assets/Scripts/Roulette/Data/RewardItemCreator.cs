using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

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

            Directory.CreateDirectory($"Assets/Resources/Rewards/{Path}");

            var newItemCollection = CreateInstance<RewardItemCollection>();

            foreach (var item in items)
            {
                var newItemData = CreateInstance<RewardItemData>();
                newItemData.Sprite =
                    new AssetReferenceT<Sprite>(AssetDatabase.AssetPathToGUID($"{fullPath}/{item.Name}"));

                newItemData.name = item.Name;
                var croppedName = item.Name.Split(".png")[0];

                AssetDatabase.CreateAsset(newItemData, $"Assets/Resources/Rewards/{Path}/{croppedName}.asset");
                AssetDatabase.SaveAssets();

                newItemCollection.Items.Add(newItemData);
            }

            AssetDatabase.CreateAsset(newItemCollection, $"Assets/Resources/Rewards/{Path}Collection.asset");
        }
    }
}