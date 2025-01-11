using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CropIconGenerator : MonoBehaviour
{
    public IconGenerator IconGenerator;
    
    [System.Serializable]
    public class IconPrefabData
    {
        public GameObject ModelPrefab;
    }
    
    public IconPrefabData[] Prefabs;
    
    #if UNITY_EDITOR
    [ContextMenu("Generate Icons")]
    public void GenerateIcons()
    {
        foreach (var data in Prefabs)
        {
            if (data.ModelPrefab != null)
            {
                // Generate icon from the provided model
                Sprite icon = IconGenerator.GenerateIcon(data.ModelPrefab);
                
                // Save sprite as asset
                if (icon != null)
                {
                    string prefabName = data.ModelPrefab.name;
                    string path = $"Assets/Sprites/Icons/{prefabName}_Icon.png";
                    
                    // Save texture to file
                    byte[] bytes = icon.texture.EncodeToPNG();
                    System.IO.File.WriteAllBytes(path, bytes);
                    AssetDatabase.ImportAsset(path);
                    
                    // Set as sprite
                    TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (importer != null)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                        importer.SaveAndReimport();
                    }
                }
            }
        }
        
        AssetDatabase.SaveAssets();
    }
    #endif
} 