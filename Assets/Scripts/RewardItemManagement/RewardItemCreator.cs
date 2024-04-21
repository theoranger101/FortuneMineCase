using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;

namespace RewardItemManagement
{
    /// <summary>
    /// scriptable tool to easily create scriptables of the reward items inside atlases.
    /// </summary>
    [CreateAssetMenu]
    public class RewardItemCreator : ScriptableObject
    {
        public Material MaterialTemplate;
        
        private const string RootAtlasFolder = "Assets/Sprites/Atlases/Rewards";
        private const string RootMaterialFolder = "Assets/Materials/Rewards";
        private const string RootScriptableFolder = "Assets/GameData/Rewards";
        
        private int count;
        
        // in more advanced, i would write on top of the existing data instead of
        //  having to re-create all of them in every method call.
        [Button]
        private void CreateAtlasItems()
        {
            count = 0;
            
            var directoryInfo = new DirectoryInfo(RootAtlasFolder);
            var items = directoryInfo.GetFiles("*.spriteatlas");

            for (var i = items.Length - 1; i >= 0; i--)
            {
                var atlasPath = $"{RootAtlasFolder}/{items[i].Name}";
                var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);

                var atlasName = items[i].Name.Split("Atlas_")[1];
                atlasName = atlasName.Split(".spriteatlas")[0];
                
                var directoryName = $"{RootScriptableFolder}/{atlasName}";
                Directory.CreateDirectory(directoryName);

                var newItemCollection = CreateInstance<RewardItemCollection>();

                var sprites = new Sprite[atlas.spriteCount];
                atlas.GetSprites(sprites);

                for (var k = 0; k < sprites.Length; k++)
                {
                    var newItemData = CreateRewardItem(atlasPath, sprites[k], directoryName);
                    newItemCollection.Items.Add(newItemData);
                    
                    count++;
                }
                
                AssetDatabase.CreateAsset(newItemCollection, $"{RootScriptableFolder}/{atlasName}Collection.asset");
            }
        }

        private RewardItem CreateRewardItem(string atlasPath, Sprite sprite, string directoryName)
        {
            var newItemData = CreateInstance<RewardItem>();
            
            newItemData.Id = count;

            var croppedName = sprite.name.Split("(Clone)")[0];
            newItemData.name = $"{count}_{croppedName}";
            newItemData.Name = croppedName;
            
            AssignSprite(newItemData, atlasPath, sprite);

            newItemData.Material = new AssetReferenceT<Material>(
                AssetDatabase.AssetPathToGUID(CreateMaterial(sprite.texture, newItemData.name)));

            AssetDatabase.CreateAsset(newItemData, $"{directoryName}/{newItemData.name}.asset");
            AssetDatabase.SaveAssets();

            return newItemData;
        }

        private void AssignSprite(RewardItem rewardItem, string atlasPath, Sprite sprite)
        {
            rewardItem.Sprite =
                new AssetReference(
                    AssetDatabase.AssetPathToGUID($"{atlasPath}"));
            
            rewardItem.Sprite.SetEditorSubObject(sprite);
            rewardItem.Sprite.SubObjectName = sprite.name;
            
            rewardItem.SpritePath =  $"{RootAtlasFolder}/{rewardItem.Sprite.editorAsset.name}.spriteatlas[{rewardItem.Name}]";
        }

        private string CreateMaterial(Texture texture, string name)
        {
            var newMaterial = new Material(MaterialTemplate)
            {
                mainTexture = texture
            };

            var materialPath = $"{RootMaterialFolder}/M_{name}.mat";
            
            AssetDatabase.CreateAsset(newMaterial, materialPath);
            AssetDatabase.SaveAssets();

            return materialPath;
        }
    }
}