using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game State")]
    private bool _isGamePaused;
    public bool IsGamePaused => _isGamePaused;
    
    //public float PlayerMoney { get; private set; }
    public float PlayerMoney;
    
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
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1) // Game scene
        {
            _isGamePaused = false;
            Time.timeScale = 1f;
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
        
        // Play money sound when gaining money
        if (amount > 0)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.MoneyUpSound);
        }
    }
} 