using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string ItemId;
    public string ItemName;
    public ItemType Type;
    public Sprite ItemIcon;
    public int Quantity;
    public float Value;
    public float SellMultiplier = 0.5f;
    
    public enum ItemType
    {
        Seed,
        Crop,
        Tool,
        Upgrade
    }
    
    public InventoryItem(string id, string name, ItemType type, Sprite icon, float value, float sellMultiplier = 0.5f)
    {
        this.ItemId = id;
        this.ItemName = name;
        this.Type = type;
        this.ItemIcon = icon;
        this.Value = value;
        this.SellMultiplier = sellMultiplier;
        this.Quantity = 1;
    }
} 