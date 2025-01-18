using UnityEngine;
using System.Collections.Generic;
using System;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }
    public static event System.Action OnHandlingEsc;
    
    [Header("Inventory Settings")]
    public int MaxInventorySlots = 24;
    
    [Header("Starting Items")]
    [SerializeField] private List<StartingInventoryItem> _startingItems = new List<StartingInventoryItem>();
    
    public List<InventoryItem> Items = new List<InventoryItem>();
    
    public event Action OnInventoryChanged;
    public event Action<InventoryItem> OnItemAdded;
    public event Action<InventoryItem> OnItemSold;
    
    private InventoryItem _selectedSeed;
    public event Action<InventoryItem> OnSeedSelected;
    public InventoryItem GetSelectedSeed() => _selectedSeed;
    
    [Header("Tool References")]
    [SerializeField] private List<GameObject> _toolPrefabs = new List<GameObject>();
    
    public bool IsInventoryOpen { get; private set; }
    
    [SerializeField] private GameObject _inventoryPanel;
    
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
        // If clicking the same seed that's already selected, deselect it
        if (item != null && _selectedSeed != null && item.ItemId == _selectedSeed.ItemId)
        {
            _selectedSeed = null;
            OnSeedSelected?.Invoke(null);
            return;
        }
        
        // Only allow selection if we have the item
        if (item != null && !HasItem(item.ItemId))
        {
            return;
        }
        
        _selectedSeed = item;
        OnSeedSelected?.Invoke(item);
    }
    
    private void Start()
    {
        AddStartingItems();
    }
    
    private void AddStartingItems()
    {
        foreach (var startingItem in _startingItems)
        {
            switch (startingItem.SourceType)
            {
                case StartingInventoryItem.ItemSourceType.Tool:
                    AddStartingTool(startingItem);
                    break;
                    
                case StartingInventoryItem.ItemSourceType.Seed:
                    AddStartingSeed(startingItem);
                    break;
                    
                case StartingInventoryItem.ItemSourceType.Crop:
                    AddStartingCrop(startingItem);
                    break;
            }
        }
    }
    
    private void AddStartingTool(StartingInventoryItem startingItem)
    {
        if (startingItem.ItemPrefab == null) return;
        
        Tool tool = startingItem.ItemPrefab.GetComponent<Tool>();
        if (tool != null)
        {
            InventoryItem item = new InventoryItem(
                tool.ToolId,
                tool.ToolName,
                InventoryItem.ItemType.Tool,
                tool.ToolIcon,
                tool.BaseValue
            );
            item.Quantity = startingItem.StartingQuantity;
            AddItem(item);
        }
    }
    
    private void AddStartingSeed(StartingInventoryItem startingItem)
    {
        if (string.IsNullOrEmpty(startingItem.CropId)) return;
        
        InventoryItem seedItem = CropManager.Instance.CreateSeedItem(startingItem.CropId);
        if (seedItem != null)
        {
            seedItem.Quantity = startingItem.StartingQuantity;
            AddItem(seedItem);
        }
    }
    
    private void AddStartingCrop(StartingInventoryItem startingItem)
    {
        if (string.IsNullOrEmpty(startingItem.CropId)) return;
        
        CropData cropData = CropManager.Instance.GetCropData(startingItem.CropId);
        if (cropData != null)
        {
            InventoryItem cropItem = new InventoryItem(
                cropData.CropId,
                cropData.CropName,
                InventoryItem.ItemType.Crop,
                cropData.CropIcon,
                cropData.BaseValue
            );
            cropItem.Quantity = startingItem.StartingQuantity;
            AddItem(cropItem);
        }
    }
    
    public void SellItem(InventoryItem item)
    {
        float sellPrice = item.Value * item.SellMultiplier;
        RemoveItem(item.ItemId);
        GameManager.Instance.AddMoney(sellPrice);
        OnItemSold?.Invoke(item);
    }
    
    public GameObject GetToolPrefab(string toolId)
    {
        foreach (var prefab in _toolPrefabs)
        {
            Tool tool = prefab.GetComponent<Tool>();
            if (tool != null && tool.ToolId == toolId)
            {
                return prefab;
            }
        }
        return null;
    }
    
    public void OpenInventory()
    {
        IsInventoryOpen = true;
        _inventoryPanel.SetActive(true);
        GameManager.Instance.PauseGame();
    }
    
    public void CloseInventory()
    {
        IsInventoryOpen = false;
        _inventoryPanel.SetActive(false);
        GameManager.Instance.ResumeGame();
        TooltipUI.Instance.HideTooltip();
    }
    
    public void ClearInventory()
    {
        Items.Clear();
        OnInventoryChanged?.Invoke();
    }
    
    private void Update()
    {
        // Toggle inventory with 'I' key
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
        
        // Close inventory with ESC key if it's open
        if (Input.GetKeyDown(KeyCode.Escape) && IsInventoryOpen)
        {
            OnHandlingEsc?.Invoke();
            CloseInventory();
        }
    }

    public void ToggleInventory()
    {
        if (IsInventoryOpen)
            CloseInventory();
        else
            OpenInventory();
    }
} 