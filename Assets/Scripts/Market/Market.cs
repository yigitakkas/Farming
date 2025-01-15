using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Market : MonoBehaviour, IInteractable
{
    [Header("UI References")]
    [SerializeField] private GameObject _marketPanel;
    [SerializeField] private GameObject _buyTabContent;
    [SerializeField] private GameObject _sellTabContent;
    
    [Header("Tab Buttons")]
    [SerializeField] private Button _buyTabButton;
    [SerializeField] private Button _sellTabButton;
    [SerializeField] private Color _selectedTabColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color _unselectedTabColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    
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
        ShowBuyTab(); // This will also set the correct tab visuals
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
        UpdateTabVisuals(true);
        
        // Clear existing items
        foreach (Transform child in _buyItemContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create all slots (empty or filled)
        for (int i = 0; i < GameConstants.MAX_MARKET_SLOTS; i++)
        {
            GameObject itemUI = Instantiate(_marketItemPrefab, _buyItemContainer);
            MarketItemUI marketItem = itemUI.GetComponent<MarketItemUI>();
            
            // If we have a shop item for this slot, set it up
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
    
    public void ShowSellTab()
    {
        _buyTabContent.SetActive(false);
        _sellTabContent.SetActive(true);
        UpdateTabVisuals(false);
        
        // Clear existing items
        foreach (Transform child in _sellItemContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create all slots (empty or filled)
        for (int i = 0; i < GameConstants.MAX_MARKET_SLOTS; i++)
        {
            GameObject itemUI = Instantiate(_marketItemPrefab, _sellItemContainer);
            MarketItemUI marketItem = itemUI.GetComponent<MarketItemUI>();
            
            // If we have an inventory item for this slot, set it up
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
    
    private void UpdateTabVisuals(bool isBuyTab)
    {
        // Update button colors
        _buyTabButton.image.color = isBuyTab ? _selectedTabColor : _unselectedTabColor;
        _sellTabButton.image.color = isBuyTab ? _unselectedTabColor : _selectedTabColor;
    }
} 