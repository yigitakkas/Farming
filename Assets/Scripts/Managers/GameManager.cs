using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game State")]
    public bool IsGamePaused;
    public float GameTime;
    public float DayLength = 24f; // In minutes
    
    [Header("Economy")]
    public float PlayerMoney = 0f;
    
    public event Action<float> OnMoneyChanged;
    public event Action<int> OnDayChanged;
    
    private int _currentDay = 1;
    
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
    
    private void Update()
    {
        if (!IsGamePaused)
        {
            GameTime += Time.deltaTime;
            if (GameTime >= DayLength * 60) // Convert minutes to seconds
            {
                NewDay();
            }
        }
    }
    
    public void AddMoney(float amount)
    {
        PlayerMoney += amount;
        OnMoneyChanged?.Invoke(PlayerMoney);
    }
    
    private void NewDay()
    {
        GameTime = 0;
        _currentDay++;
        OnDayChanged?.Invoke(_currentDay);
    }
    
    public void PauseGame()
    {
        IsGamePaused = true;
        Time.timeScale = 0;
    }
    
    public void ResumeGame()
    {
        IsGamePaused = false;
        Time.timeScale = 1;
    }
} 