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
    
    public enum ItemType
    {
        Seed,
        Crop,
        Tool,
        Upgrade
    }
    
    public InventoryItem(string id, string name, ItemType type, Sprite icon, float value)
    {
        this.ItemId = id;
        this.ItemName = name;
        this.Type = type;
        this.ItemIcon = icon;
        this.Quantity = 1;
        this.Value = value;
    }
} 