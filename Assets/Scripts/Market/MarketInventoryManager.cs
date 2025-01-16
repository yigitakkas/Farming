using UnityEngine;
using System.Collections.Generic;

public class MarketInventoryManager : MonoBehaviour
{
    [SerializeField] private Transform _buyItemContainer;
    [SerializeField] private Transform _sellItemContainer;
    [SerializeField] private GameObject _marketItemPrefab;
    [SerializeField] private List<InventoryItem> _shopItems = new List<InventoryItem>();
    
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