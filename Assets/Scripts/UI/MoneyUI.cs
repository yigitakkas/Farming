using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _moneyText;
    
    private void Start()
    {
        // Subscribe to money changes
        GameManager.Instance.OnMoneyChanged += UpdateMoneyDisplay;
        
        // Initial display
        UpdateMoneyDisplay(GameManager.Instance.PlayerMoney);
    }
    
    private void UpdateMoneyDisplay(float amount)
    {
        if (_moneyText != null)
        {
            _moneyText.text = $"${amount:F2}"; 
        }
    }
    
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMoneyChanged -= UpdateMoneyDisplay;
        }
    }
} 