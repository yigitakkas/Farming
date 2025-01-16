using UnityEngine;

[System.Serializable]
public class MarketItemData
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
    
    [Header("Stock Settings")]
    [Tooltip("Override the default item value. Leave at 0 to use default")]
    public float CustomValue;
    [Tooltip("How many of this item to stock")]
    public int StockQuantity = 1;
} 