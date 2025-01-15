using UnityEngine;
using TMPro;

public class TimeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timeText;
    
    private void Start()
    {
        TimeManager.Instance.OnHourChanged += UpdateTimeDisplay;
        UpdateTimeDisplay(TimeManager.Instance.CurrentHour);
    }
    
    private void UpdateTimeDisplay(float hour)
    {
        int hourInt = Mathf.FloorToInt(hour);
        int minutes = Mathf.FloorToInt((hour - hourInt) * 60);
        string ampm = hourInt >= 12 ? "PM" : "AM";
        hourInt = hourInt % 12;
        if (hourInt == 0) hourInt = 12;
        
        _timeText.text = $"{hourInt:D2}:{minutes:D2} {ampm}";
    }
    
    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnHourChanged -= UpdateTimeDisplay;
        }
    }
} 