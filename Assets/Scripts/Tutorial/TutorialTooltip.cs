using UnityEngine;
using TMPro;

public class TutorialTooltip : MonoBehaviour
{
    [SerializeField] private TMP_Text _tooltipText;
    [SerializeField] private float _floatSpeed = 0.5f;
    [SerializeField] private float _floatAmount = 10f;  // Pixels to float up/down
    private Vector2 _startPosition;
    private RectTransform _rectTransform;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _startPosition = _rectTransform.anchoredPosition;
    }

    private void Update()
    {
        // Make tooltip float up and down slightly in UI space
        _rectTransform.anchoredPosition = _startPosition + Vector2.up * (Mathf.Sin(Time.time * _floatSpeed) * _floatAmount);
    }
}