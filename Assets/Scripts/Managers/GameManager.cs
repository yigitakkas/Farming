using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game State")]
    private bool _isGamePaused;
    public bool IsGamePaused => _isGamePaused;
    
    public float PlayerMoney { get; private set; }
    
    public event Action<float> OnMoneyChanged;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void PauseGame()
    {
        _isGamePaused = true;
        Time.timeScale = 0f;
    }
    
    public void ResumeGame()
    {
        _isGamePaused = false;
        Time.timeScale = 1f;
    }
    
    public void AddMoney(float amount)
    {
        PlayerMoney += amount;
        OnMoneyChanged?.Invoke(PlayerMoney);
    }
} 