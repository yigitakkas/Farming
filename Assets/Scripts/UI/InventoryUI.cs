using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private Transform _itemsContainer;
    [SerializeField] private GameObject _itemSlotPrefab;

    private void Start()
    {
        UpdateInventoryUI();
        // Hide inventory at start
        _inventoryPanel.SetActive(false);
        
        // Subscribe to inventory changes
        InventorySystem.Instance.OnInventoryChanged += UpdateInventoryUI;
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
        Time.timeScale = _inventoryPanel.activeSelf ? 0f : 1f;
    }

    private void UpdateInventoryUI()
    {
        // Clear existing items
        foreach (Transform child in _itemsContainer)
        {
            Destroy(child.gameObject);
        }

        // Create all slots (empty or filled)
        for (int i = 0; i < InventorySystem.Instance.MaxInventorySlots; i++)
        {
            GameObject slot = Instantiate(_itemSlotPrefab, _itemsContainer);
            
            // Get references to UI components
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
            }
            else
            {
                // Empty slot
                if (iconImage != null)
                {
                    iconImage.enabled = false;
                }
                
                if (nameText != null)
                {
                    nameText.text = "";
                }
                if (quantityText != null)
                {
                    quantityText.text = "";
                }
            }
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnInventoryChanged -= UpdateInventoryUI;
        }
    }
} 