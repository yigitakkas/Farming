using UnityEngine;

[System.Serializable]
public class StartingInventoryItem
{
    public enum ItemSourceType
    {
        Tool,
        Seed,
        Crop
    }
    
    [Header("Item Type")]
    [Tooltip("Select whether this is a tool, seed, or crop")]
    public ItemSourceType SourceType;
    
    [Header("Item Source")]
    [Tooltip("Only needed for tools - drag the tool prefab here")]
    public GameObject ItemPrefab;
    [Tooltip("For seeds/crops, enter the crop ID (e.g., 'carrot', 'broccoli')")]
    public string CropId;
    
    [Header("Quantity")]
    [Min(1)]
    public int StartingQuantity = 1;
} 