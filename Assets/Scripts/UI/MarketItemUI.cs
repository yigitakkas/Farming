using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarketItemUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image ItemIcon;
    public TextMeshProUGUI ItemNameText;
    public TextMeshProUGUI PriceText;
    public Button ActionButton; // Buy/Sell button
    
    private InventoryItem _item;
    private bool _isBuyMode;
    
    public void SetupBuyItem(InventoryItem item)
    {
        _item = item;
        _isBuyMode = true;
        UpdateUI();
        
        ActionButton.onClick.RemoveAllListeners();
        ActionButton.onClick.AddListener(BuyItem);
    }
    
    public void SetupSellItem(InventoryItem item)
    {
        _item = item;
        _isBuyMode = false;
        UpdateUI();
        
        ActionButton.onClick.RemoveAllListeners();
        ActionButton.onClick.AddListener(SellItem);
    }
    
    private void UpdateUI()
    {
        if (_item != null)
        {
            ItemIcon.sprite = _item.ItemIcon;
            ItemNameText.text = _item.ItemName;
            PriceText.text = _isBuyMode ? 
                $"Buy: ${_item.Value}" : 
                $"Sell: ${_item.Value * 0.5f}"; // Sell for half the buy price
            
            // Enable/disable buy button based on player money
            if (_isBuyMode)
            {
                ActionButton.interactable = GameManager.Instance.PlayerMoney >= _item.Value;
            }
        }
    }
    
    private void BuyItem()
    {
        if (GameManager.Instance.PlayerMoney >= _item.Value)
        {
            if (InventorySystem.Instance.AddItem(_item))
            {
                GameManager.Instance.AddMoney(-_item.Value);
            }
        }
    }
    
    private void SellItem()
    {
        float sellPrice = _item.Value * 0.5f;
        InventorySystem.Instance.RemoveItem(_item.ItemId);
        GameManager.Instance.AddMoney(sellPrice);
    }
} 