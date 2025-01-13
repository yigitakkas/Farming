using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolHotbarUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject HotbarSlotPrefab;
    public Transform HotbarContainer;
    public int NumberOfSlots = GameConstants.MAX_HOTBAR_SLOTS;
    public float SlotSpacing = 10f;
    
    [Header("Selection Indicator")]
    public Image SelectionIndicator;
    
    private ToolSlot[] _slots;
    private PlayerInteraction _playerInteraction;
    
    private void Start()
    {
        _playerInteraction = FindObjectOfType<PlayerInteraction>();
        InitializeHotbar();
        UpdateToolIcons();
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
    
    private void Update()
    {
        // Update selection indicator
        if (_playerInteraction != null)
        {
            int currentIndex = _playerInteraction.CurrentToolIndex;
            if (currentIndex >= 0 && currentIndex < _slots.Length)
            {
                SelectionIndicator.gameObject.SetActive(true);
                // Get the tool icon's RectTransform
                RectTransform iconRect = _slots[currentIndex].ToolIcon.rectTransform;
                
                // Set the indicator to match the icon's position and size
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
    
    private void UpdateToolIcons()
    {
        if (_playerInteraction != null)
        {
            // First, set all slots to empty
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] != null)
                {
                    _slots[i].SetTool(null);
                }
            }

            // Then set tools for slots that have tools
            if (_playerInteraction.ToolPrefabs != null)
            {
                for (int i = 0; i < _playerInteraction.ToolPrefabs.Length && i < _slots.Length; i++)
                {
                    Tool tool = _playerInteraction.ToolPrefabs[i].GetComponent<Tool>();
                    if (tool != null)
                    {
                        _slots[i].SetTool(tool);
                    }
                }
            }
        }
    }
} 