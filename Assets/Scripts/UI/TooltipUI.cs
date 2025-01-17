using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance { get; private set; }

    [SerializeField] private GameObject _tooltipPanel;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private Vector2 _offset = new Vector2(100f, 0f); // Offset from slot

    private RectTransform _panelRectTransform;
    private Canvas _parentCanvas;
    private RectTransform _currentSlot;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _panelRectTransform = _tooltipPanel.GetComponent<RectTransform>();
        _parentCanvas = GetComponentInParent<Canvas>();
        HideTooltip();
    }

    private void Update()
    {
        if (_tooltipPanel.activeSelf && _currentSlot != null)
        {
            // Position tooltip relative to the inventory slot
            Vector3[] corners = new Vector3[4];
            _currentSlot.GetWorldCorners(corners);
            Vector2 slotRightEdge = corners[2]; // Top-right corner

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentCanvas.GetComponent<RectTransform>(),
                slotRightEdge,
                _parentCanvas.worldCamera,
                out Vector2 localPoint
            );

            _panelRectTransform.anchoredPosition = localPoint + _offset;
        }
    }

    public void ShowTooltip(string title, string description, float value, RectTransform slotTransform)
    {
        _currentSlot = slotTransform;
        _titleText.text = title;
        _descriptionText.text = description;
        _valueText.text = value > 0 ? $"Value: ${value:F2}" : "";
        _tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        _tooltipPanel.SetActive(false);
        _currentSlot = null;
    }
} 