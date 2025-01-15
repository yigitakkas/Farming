using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ObjectiveEntryUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _rewardText;
    [SerializeField] private Image _backgroundImage;
    
    private Objective _currentObjective;
    
    public void SetupObjective(Objective objective)
    {
        _currentObjective = objective;
        _currentObjective.OnProgressUpdated += UpdateProgress;
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        _titleText.text = _currentObjective.Title;
        _descriptionText.text = _currentObjective.Description;
        _rewardText.text = $"Reward: ${_currentObjective.Reward:F2}";
    }
    
    private void UpdateProgress(Objective objective)
    {
        UpdateUI();
    }
    
    private void OnDestroy()
    {
        if (_currentObjective != null)
        {
            _currentObjective.OnProgressUpdated -= UpdateProgress;
        }
    }
    
    public void ShowCompleted()
    {
        // Optional: Change appearance
        _backgroundImage.color = Color.green;
        _titleText.text += " (Completed!)";
    }
} 