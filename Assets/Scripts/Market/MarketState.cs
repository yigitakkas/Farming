using UnityEngine;

public class MarketState : MonoBehaviour
{
    public static MarketState Instance { get; private set; }
    
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
    }
    
    private bool _isOpen;
    public bool IsOpen => _isOpen;
    
    public void Open()
    {
        _isOpen = true;
    }
    
    public void Close()
    {
        _isOpen = false;
    }
} 