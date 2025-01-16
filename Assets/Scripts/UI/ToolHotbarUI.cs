using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolHotbarUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject HotbarSlotPrefab;
    public Transform HotbarContainer;
    public int NumberOfSlots = GameConstants.MAX_HOTBAR_SLOTS;
    
    [Header("Selection Indicator")]
    public Image SelectionIndicator;
    
    private ToolSlot[] _slots;
    private PlayerInteraction _playerInteraction;
    
    private void Start()
    {
        _playerInteraction = FindObjectOfType<PlayerInteraction>();
        InitializeHotbar();
        
        // Subscribe to inventory changes
        InventorySystem.Instance.OnInventoryChanged += UpdateToolIcons;
        UpdateToolIcons();
    }
    
    private void OnDestroy()
    {
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnInventoryChanged -= UpdateToolIcons;
        }
    }
    
    private void InitializeHotbar()
    {
        _slots = new ToolSlot[NumberOfSlots];
        
        for (int i = 0; i < NumberOfSlots; i++)
        {
            GameObject slotObj = Instantiate(HotbarSlotPrefab, HotbarContainer);
            ToolSlot slot = slotObj.GetComponent<ToolSlot>();
            
            if (slot != null)
            {
                slot.SlotNumber = i + 1;
                slot.UpdateNumberText();
                _slots[i] = slot;
            }
        }
    }
    
    private void UpdateToolIcons()
    {
        // First, clear all slots
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] != null)
            {
                _slots[i].SetTool(null);
            }
        }
        
        // Get all tools from inventory
        var tools = InventorySystem.Instance.Items.FindAll(item => 
            item.Type == InventoryItem.ItemType.Tool);
            
        // Fill slots with tools from inventory
        for (int i = 0; i < tools.Count && i < _slots.Length; i++)
        {
            if (_slots[i] != null)
            {
                GameObject toolPrefab = InventorySystem.Instance.GetToolPrefab(tools[i].ItemId);
                if (toolPrefab != null)
                {
                    Tool tool = toolPrefab.GetComponent<Tool>();
                    if (tool != null)
                    {
                        _slots[i].SetTool(tool);
                    }
                }
            }
        }
    }
    
    private void Update()
    {
        // Update selection indicator
        if (_playerInteraction != null)
        {
            int currentIndex = _playerInteraction.CurrentToolIndex;
            if (currentIndex >= 0 && currentIndex < _slots.Length)
            {
                SelectionIndicator.gameObject.SetActive(true);
                RectTransform iconRect = _slots[currentIndex].ToolIcon.rectTransform;
                
                SelectionIndicator.transform.SetParent(_slots[currentIndex].transform, false);
                SelectionIndicator.rectTransform.anchorMin = iconRect.anchorMin;
                SelectionIndicator.rectTransform.anchorMax = iconRect.anchorMax;
                SelectionIndicator.rectTransform.anchoredPosition = iconRect.anchoredPosition;
                SelectionIndicator.rectTransform.sizeDelta = iconRect.sizeDelta;
            }
            else
            {
                SelectionIndicator.gameObject.SetActive(false);
            }
        }
        else
        {
            SelectionIndicator.gameObject.SetActive(false);
        }
    }
} 