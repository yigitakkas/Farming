using UnityEngine;
using System.Collections.Generic;

public class MarketInventoryManager : MonoBehaviour
{
    [SerializeField] private Transform _buyItemContainer;
    [SerializeField] private Transform _sellItemContainer;
    [SerializeField] private GameObject _marketItemPrefab;
    
    [Header("Shop Stock")]
    [SerializeField] private List<MarketItemData> _marketStock = new List<MarketItemData>();
    private List<InventoryItem> _shopItems = new List<InventoryItem>();
    
    private void Start()
    {
        InitializeShopItems();
        InventorySystem.Instance.OnItemSold += HandleSoldItem;
        InventorySystem.Instance.OnInventoryChanged += RefreshCurrentTab;
    }
    
    private void OnDestroy()
    {
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnItemSold -= HandleSoldItem;
            InventorySystem.Instance.OnInventoryChanged -= RefreshCurrentTab;
        }
    }
    
    private void RefreshCurrentTab()
    {
        if (!MarketTabManager.Instance.IsBuyTabActive)
        {
            PopulateSellTab();
        }
    }
    
    private void HandleSoldItem(InventoryItem soldItem)
    {
        var existingItem = _shopItems.Find(item => item.ItemId == soldItem.ItemId);
        
        if (existingItem == null)
        {
            InventoryItem shopItem = new InventoryItem(
                soldItem.ItemId,
                soldItem.ItemName,
                soldItem.Type,
                soldItem.ItemIcon,
                soldItem.Value,
                soldItem.SellMultiplier
            );
            _shopItems.Add(shopItem);
            
            if (MarketTabManager.Instance.IsBuyTabActive)
            {
                PopulateBuyTab();
            }
        }
    }
    
    private void InitializeShopItems()
    {
        _shopItems.Clear();
        
        foreach (var marketItem in _marketStock)
        {
            switch (marketItem.SourceType)
            {
                case MarketItemData.ItemSourceType.Tool:
                    AddToolToShop(marketItem);
                    break;
                    
                case MarketItemData.ItemSourceType.Seed:
                    AddSeedToShop(marketItem);
                    break;
                    
                case MarketItemData.ItemSourceType.Crop:
                    AddCropToShop(marketItem);
                    break;
            }
        }
    }
    
    private void AddToolToShop(MarketItemData marketItem)
    {
        if (marketItem.ItemPrefab == null) return;
        
        Tool tool = marketItem.ItemPrefab.GetComponent<Tool>();
        if (tool != null)
        {
            float value = marketItem.CustomValue > 0 ? marketItem.CustomValue : tool.BaseValue;
            
            InventoryItem item = new InventoryItem(
                tool.ToolId,
                tool.ToolName,
                InventoryItem.ItemType.Tool,
                tool.ToolIcon,
                value
            );
            
            item.Quantity = marketItem.StockQuantity;
            _shopItems.Add(item);
        }
    }
    
    private void AddSeedToShop(MarketItemData marketItem)
    {
        if (string.IsNullOrEmpty(marketItem.CropId)) return;
        
        InventoryItem seedItem = CropManager.Instance.CreateSeedItem(marketItem.CropId);
        if (seedItem != null)
        {
            if (marketItem.CustomValue > 0)
            {
                seedItem.Value = marketItem.CustomValue;
            }
            seedItem.Quantity = marketItem.StockQuantity;
            _shopItems.Add(seedItem);
        }
    }
    
    private void AddCropToShop(MarketItemData marketItem)
    {
        if (string.IsNullOrEmpty(marketItem.CropId)) return;
        
        CropData cropData = CropManager.Instance.GetCropData(marketItem.CropId);
        if (cropData != null)
        {
            float value = marketItem.CustomValue > 0 ? marketItem.CustomValue : cropData.BaseValue;
            
            InventoryItem cropItem = new InventoryItem(
                cropData.CropId,
                cropData.CropName,
                InventoryItem.ItemType.Crop,
                cropData.CropIcon,
                value
            );
            
            cropItem.Quantity = marketItem.StockQuantity;
            _shopItems.Add(cropItem);
        }
    }
    
    public void PopulateBuyTab()
    {
        ClearContainer(_buyItemContainer);
        
        for (int i = 0; i < GameConstants.MAX_MARKET_SLOTS; i++)
        {
            GameObject itemUI = Instantiate(_marketItemPrefab, _buyItemContainer);
            MarketItemUI marketItem = itemUI.GetComponent<MarketItemUI>();
            
            if (i < _shopItems.Count)
            {
                marketItem.SetupBuyItem(_shopItems[i]);
            }
            else
            {
                marketItem.SetupEmptySlot();
            }
        }
    }
    
    public void PopulateSellTab()
    {
        ClearContainer(_sellItemContainer);
        
        for (int i = 0; i < GameConstants.MAX_MARKET_SLOTS; i++)
        {
            GameObject itemUI = Instantiate(_marketItemPrefab, _sellItemContainer);
            MarketItemUI marketItem = itemUI.GetComponent<MarketItemUI>();
            
            if (i < InventorySystem.Instance.Items.Count)
            {
                marketItem.SetupSellItem(InventorySystem.Instance.Items[i]);
            }
            else
            {
                marketItem.SetupEmptySlot();
            }
        }
    }
    
    private void ClearContainer(Transform container)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
} 