using UnityEngine;
using System.Collections.Generic;
using System;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }
    
    public int MaxInventorySlots = 24;
    public List<InventoryItem> Items = new List<InventoryItem>();
    
    public event Action OnInventoryChanged;
    public event Action<InventoryItem> OnItemAdded;
    
    private InventoryItem _selectedSeed;
    public event Action<InventoryItem> OnSeedSelected;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public bool AddItem(InventoryItem newItem)
    {
        // Check if item exists and is stackable
        var existingItem = Items.Find(item => item.ItemId == newItem.ItemId);
        
        if (existingItem != null)
        {
            existingItem.Quantity += newItem.Quantity;
            OnInventoryChanged?.Invoke();
            OnItemAdded?.Invoke(newItem);
            return true;
        }
        
        // Add new item if there's space
        if (Items.Count < MaxInventorySlots)
        {
            Items.Add(newItem);
            OnInventoryChanged?.Invoke();
            OnItemAdded?.Invoke(newItem);
            return true;
        }
        
        Debug.Log("Inventory is full!");
        return false;
    }
    
    public void RemoveItem(string itemId, int quantity = 1)
    {
        var item = Items.Find(i => i.ItemId == itemId);
        
        if (item != null)
        {
            item.Quantity -= quantity;
            
            if (item.Quantity <= 0)
            {
                Items.Remove(item);
                
                // Clear selected seed if we removed it
                if (_selectedSeed != null && _selectedSeed.ItemId == itemId)
                {
                    SelectSeed(null);
                }
            }
            
            OnInventoryChanged?.Invoke();
        }
    }
    
    public bool HasItem(string itemId, int quantity = 1)
    {
        var item = Items.Find(i => i.ItemId == itemId);
        return item != null && item.Quantity >= quantity;
    }
    
    public void SelectSeed(InventoryItem item)
    {
        // Only allow selection if we have the item
        if (item != null && !HasItem(item.ItemId))
        {
            return;
        }
        
        _selectedSeed = item;
        OnSeedSelected?.Invoke(item);
    }
    
    public InventoryItem GetSelectedSeed() => _selectedSeed;
    
    private void Start()
    {
        // Add some starting seeds
        AddStartingSeeds();
    }
    
    private void AddStartingSeeds()
    {
        // Get seed items from CropManager
        var carrotSeed = CropManager.Instance.CreateSeedItem("carrot");
        var broccoliSeed = CropManager.Instance.CreateSeedItem("broccoli");
        
        if (carrotSeed != null)
        {
            carrotSeed.Quantity = 5;
            AddItem(carrotSeed);
        }
        
        if (broccoliSeed != null)
        {
            broccoliSeed.Quantity = 5;
            AddItem(broccoliSeed);
        }
    }
} 