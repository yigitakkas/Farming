using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MarketItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
    
    private void OnEnable()
    {
        // Subscribe to money changes to update button state
        GameManager.Instance.OnMoneyChanged += UpdateButtonState;
    }
    
    private void OnDisable()
    {
        GameManager.Instance.OnMoneyChanged -= UpdateButtonState;
    }
    
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
            ItemIcon.enabled = true;
            ItemIcon.sprite = _item.ItemIcon;
            ItemNameText.text = _item.ItemName;
            
            // Calculate price based on buy/sell mode
            float price = _isBuyMode ? _item.Value : _item.Value * _item.SellMultiplier;
            PriceText.text = $"${price:F2}";
            
            // Only show quantity in sell mode
            if (_isBuyMode)
            {
                QuantityText.gameObject.SetActive(false);
            }
            else
            {
                QuantityText.text = $"x{_item.Quantity}";
                QuantityText.gameObject.SetActive(true);
            }
            
            UpdateButtonState(_isBuyMode ? GameManager.Instance.PlayerMoney : 0);
        }
        else
        {
            ItemIcon.enabled = false;
            ItemNameText.text = "";
            PriceText.text = "";
            QuantityText.text = "";
            QuantityText.gameObject.SetActive(false);
            ActionButton.gameObject.SetActive(false);
        }
    }
    
    private void UpdateButtonState(float currentMoney)
    {
        if (_item != null)
        {
            if (_isBuyMode)
            {
                ActionButton.interactable = currentMoney >= _item.Value;
                ButtonText.text = "Buy";
            }
        }
    }
    
    private void SellItem()
    {
        InventorySystem.Instance.SellItem(_item);
        SetupEmptySlot();
    }
    
    private void BuyItem()
    {
        if (GameManager.Instance.PlayerMoney >= _item.Value)
        {
            // Create a copy of the item with quantity of 1
            InventoryItem itemToBuy = new InventoryItem(
                _item.ItemId,
                _item.ItemName,
                _item.Type,
                _item.ItemIcon,
                _item.Value,
                _item.SellMultiplier
            );
            itemToBuy.Quantity = 1;
            
            if (InventorySystem.Instance.AddItem(itemToBuy))
            {
                GameManager.Instance.AddMoney(-_item.Value);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_item == null) return;

        string description = "";
        if (_item.Type == InventoryItem.ItemType.Tool)
        {
            Tool toolData = InventorySystem.Instance.GetToolPrefab(_item.ItemId)?.GetComponent<Tool>();
            if (toolData != null)
            {
                description = toolData.GetDescription();
            }
        }
        else if (_item.Type == InventoryItem.ItemType.Seed)
        {
            string cropId = _item.ItemId.Replace("_seed", "");
            CropData cropData = CropManager.Instance.GetCropData(cropId);
            if (cropData != null)
            {
                description = $"Plant this to grow {cropData.CropName}.\n\n" + cropData.GetDescription();
            }
        }
        else if (_item.Type == InventoryItem.ItemType.Crop)
        {
            CropData cropData = CropManager.Instance.GetCropData(_item.ItemId);
            if (cropData != null)
            {
                description = cropData.GetDescription();
            }
        }

        float price = _isBuyMode ? _item.Value : _item.Value * _item.SellMultiplier;
        TooltipUI.Instance.ShowTooltip(
            _item.ItemName,
            description,
            price,
            GetComponent<RectTransform>()
        );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.HideTooltip();
    }
} 