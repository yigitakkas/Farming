using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarketItemUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image ItemIcon;
    public TextMeshProUGUI ItemNameText;
    public TextMeshProUGUI PriceText;
    public TextMeshProUGUI QuantityText;
    public Button ActionButton;
    public TextMeshProUGUI ButtonText;
    public Image BackgroundImage;
    
    private InventoryItem _item;
    private bool _isBuyMode;
    
    public void SetupEmptySlot()
    {
        _item = null;
        ItemIcon.enabled = false;
        ItemNameText.text = "";
        PriceText.text = "";
        QuantityText.text = "";
        ActionButton.gameObject.SetActive(false);
    }
    
    public void SetupBuyItem(InventoryItem item)
    {
        _item = item;
        _isBuyMode = true;
        ButtonText.text = "Buy";
        UpdateUI();
        
        ActionButton.onClick.RemoveAllListeners();
        ActionButton.onClick.AddListener(BuyItem);
        ActionButton.gameObject.SetActive(true);
    }
    
    public void SetupSellItem(InventoryItem item)
    {
        _item = item;
        _isBuyMode = false;
        ButtonText.text = "Sell";
        UpdateUI();
        
        ActionButton.onClick.RemoveAllListeners();
        ActionButton.onClick.AddListener(SellItem);
        ActionButton.gameObject.SetActive(true);
    }
    
    private void UpdateUI()
    {
        if (_item != null)
        {
            // Update icon
            ItemIcon.sprite = _item.ItemIcon;
            ItemIcon.enabled = true;
            
            // Update texts
            ItemNameText.text = _item.ItemName;
            
            // Update quantity (only show for sell items)
            if (!_isBuyMode)
            {
                var inventoryItem = InventorySystem.Instance.Items.Find(i => i.ItemId == _item.ItemId);
                if (inventoryItem != null)
                {
                    QuantityText.text = $"x{inventoryItem.Quantity}";
                }
            }
            else
            {
                QuantityText.text = "";
            }
            
            // Update price
            float price = _isBuyMode ? _item.Value : _item.Value * 0.5f;
            PriceText.text = _isBuyMode ? $"Buy: ${price}" : $"Sell: ${price}";
            
            // Update button state
            if (_isBuyMode)
            {
                ActionButton.interactable = GameManager.Instance.PlayerMoney >= _item.Value;
            }
        }
    }
    
    private void SellItem()
    {
        float sellPrice = _item.Value * 0.5f;
        InventorySystem.Instance.RemoveItem(_item.ItemId);
        GameManager.Instance.AddMoney(sellPrice);
        
        // Check if we still have any of this item
        var remainingItem = InventorySystem.Instance.Items.Find(i => i.ItemId == _item.ItemId);
        if (remainingItem != null)
        {
            // Update the UI to show new quantity
            UpdateUI();
        }
        else
        {
            // If no items left, empty the slot
            SetupEmptySlot();
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
} 