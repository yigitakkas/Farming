using UnityEngine;
using System.Collections.Generic;

public class Market : MonoBehaviour, IInteractable
{
    [Header("UI References")]
    [SerializeField] private GameObject _marketPanel;
    [SerializeField] private GameObject _buyTabContent;
    [SerializeField] private GameObject _sellTabContent;
    
    [Header("Item Container References")]
    [SerializeField] private Transform _buyItemContainer;
    [SerializeField] private Transform _sellItemContainer;
    [SerializeField] private GameObject _marketItemPrefab;
    
    [Header("Shop Items")]
    [SerializeField] private List<InventoryItem> _shopItems = new List<InventoryItem>();
    
    private void Start()
    {
        // Set market layer
        gameObject.layer = LayerMask.NameToLayer("Market");
        
        // Hide market UI at start
        if (_marketPanel != null)
        {
            _marketPanel.SetActive(false);
        }
    }
    
    public void Interact(PlayerInteraction player)
    {
        OpenMarket();
    }
    
    public void OpenMarket()
    {
        _marketPanel.SetActive(true);
        GameManager.Instance.PauseGame();
        ShowBuyTab(); // Default to buy tab
    }
    
    public void CloseMarket()
    {
        _marketPanel.SetActive(false);
        GameManager.Instance.ResumeGame();
    }
    
    public void ShowBuyTab()
    {
        _buyTabContent.SetActive(true);
        _sellTabContent.SetActive(false);
        
        // Clear existing items
        foreach (Transform child in _buyItemContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create UI elements for each shop item
        foreach (InventoryItem item in _shopItems)
        {
            GameObject itemUI = Instantiate(_marketItemPrefab, _buyItemContainer);
            MarketItemUI marketItem = itemUI.GetComponent<MarketItemUI>();
            if (marketItem != null)
            {
                marketItem.SetupBuyItem(item);
            }
        }
    }
    
    public void ShowSellTab()
    {
        _buyTabContent.SetActive(false);
        _sellTabContent.SetActive(true);
        UpdateSellItems(); // Refresh sell items from inventory
    }
    
    private void UpdateSellItems()
    {
        // Clear existing items
        foreach (Transform child in _sellItemContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create UI elements for each inventory item
        foreach (InventoryItem item in InventorySystem.Instance.Items)
        {
            GameObject itemUI = Instantiate(_marketItemPrefab, _sellItemContainer);
            MarketItemUI marketItem = itemUI.GetComponent<MarketItemUI>();
            if (marketItem != null)
            {
                marketItem.SetupSellItem(item);
            }
        }
    }
} 