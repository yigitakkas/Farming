using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolSlot : MonoBehaviour
{
    [Header("UI Elements")]
    public Image ToolIcon;
    public TextMeshProUGUI NumberText;
    
    [HideInInspector]
    public int SlotNumber;
    
    public void SetTool(Tool tool)
    {
        if (tool != null && tool.ToolIcon != null)
        {
            ToolIcon.sprite = tool.ToolIcon;
            ToolIcon.enabled = true;
        }
        else
        {
            ToolIcon.enabled = false;
        }
    }
    
    public void UpdateNumberText()
    {
        if (NumberText != null)
        {
            NumberText.text = SlotNumber.ToString();
        }
    }
} 