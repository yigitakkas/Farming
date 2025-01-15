using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private Transform _itemsContainer;
    [SerializeField] private GameObject _itemSlotPrefab;
    
    [Header("Selection Colors")]
    [SerializeField] private Color _selectedColor = new Color(0.7f, 1f, 0.7f, 1f); // Light green
    [SerializeField] private Color _normalColor = Color.white;
    
    private List<GameObject> _slotObjects = new List<GameObject>();
    
    private void Start()
    {
        UpdateInventoryUI();
        _inventoryPanel.SetActive(false);
        
        InventorySystem.Instance.OnInventoryChanged += UpdateInventoryUI;
        InventorySystem.Instance.OnSeedSelected += UpdateSelectedSeedVisual;
    }
    
    private void Update()
    {
        // Toggle inventory with 'I' key
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }
    
    private void ToggleInventory()
    {
        _inventoryPanel.SetActive(!_inventoryPanel.activeSelf);
        
        // Optional: pause game when inventory is open
        if (GameManager.Instance != null)
        {
            if (_inventoryPanel.activeSelf)
            {
                GameManager.Instance.PauseGame();
            }
            else
            {
                GameManager.Instance.ResumeGame();
            }
        }
    }
    
    private void UpdateInventoryUI()
    {
        // Clear existing slots
        foreach (Transform child in _itemsContainer)
        {
            Destroy(child.gameObject);
        }
        _slotObjects.Clear();

        // Create all slots
        for (int i = 0; i < InventorySystem.Instance.MaxInventorySlots; i++)
        {
            GameObject slot = Instantiate(_itemSlotPrefab, _itemsContainer);
            _slotObjects.Add(slot);
            
            Image slotBackground = slot.GetComponent<Image>();
            Image iconImage = slot.transform.Find("ItemIcon")?.GetComponent<Image>();
            TextMeshProUGUI nameText = slot.transform.Find("ItemText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI quantityText = slot.transform.Find("ItemQuantityText")?.GetComponent<TextMeshProUGUI>();
            
            // Check if we have an item for this slot
            if (i < InventorySystem.Instance.Items.Count)
            {
                InventoryItem item = InventorySystem.Instance.Items[i];
                
                // Set item data
                if (iconImage != null)
                {
                    iconImage.sprite = item.ItemIcon;
                    iconImage.enabled = item.ItemIcon != null;
                }
                
                if (nameText != null)
                {
                    nameText.text = $"{item.ItemName}";
                }
                
                if (quantityText != null)
                {
                    quantityText.text = $"x{item.Quantity}";
                }
                
                // Add click handler
                int index = i; // Capture the index for the lambda
                Button button = slot.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => OnItemClicked(item));
                }
                
                // Set initial color based on whether this is the selected seed
                if (slotBackground != null)
                {
                    bool isSelected = item == InventorySystem.Instance.GetSelectedSeed();
                    slotBackground.color = isSelected ? _selectedColor : _normalColor;
                }
            }
            else
            {
                // Empty slot
                if (iconImage != null) iconImage.enabled = false;
                if (nameText != null) nameText.text = "";
                if (quantityText != null) quantityText.text = "";
                if (slotBackground != null) slotBackground.color = _normalColor;
            }
        }
    }
    
    private void OnItemClicked(InventoryItem item)
    {
        if (item.Type == InventoryItem.ItemType.Seed)
        {
            InventorySystem.Instance.SelectSeed(item);
        }
    }
    
    private void UpdateSelectedSeedVisual(InventoryItem selectedSeed)
    {
        // Update all slot backgrounds
        for (int i = 0; i < _slotObjects.Count; i++)
        {
            Image slotBackground = _slotObjects[i].GetComponent<Image>();
            if (slotBackground != null)
            {
                if (i < InventorySystem.Instance.Items.Count)
                {
                    InventoryItem slotItem = InventorySystem.Instance.Items[i];
                    slotBackground.color = (slotItem == selectedSeed) ? _selectedColor : _normalColor;
                }
                else
                {
                    slotBackground.color = _normalColor;
                }
            }
        }
    }
    
    private void OnDestroy()
    {
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnInventoryChanged -= UpdateInventoryUI;
            InventorySystem.Instance.OnSeedSelected -= UpdateSelectedSeedVisual;
        }
    }
} 