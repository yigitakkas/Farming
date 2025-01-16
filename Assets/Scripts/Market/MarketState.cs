using UnityEngine;

public class MarketState : MonoBehaviour
{
    private bool _isOpen;
    public bool IsOpen => _isOpen;
    
    public void Open()
    {
        _isOpen = true;
        GameManager.Instance.PauseGame();
    }
    
    public void Close()
    {
        _isOpen = false;
        GameManager.Instance.ResumeGame();
    }
} 