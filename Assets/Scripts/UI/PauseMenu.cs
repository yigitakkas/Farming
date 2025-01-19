using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _quitButton;

    [Header("Options Panel")]
    [SerializeField] private GameObject _optionsPanel;

    private bool _isPaused = false;
    private bool _canCheckPause = true;
    private bool _escHandledThisFrame = false;

    private void OnEnable()
    {
        Market.OnHandlingEsc += OnUIHandledEsc;
        InventorySystem.OnHandlingEsc += OnUIHandledEsc;
    }

    private void OnDisable()
    {
        Market.OnHandlingEsc -= OnUIHandledEsc;
        InventorySystem.OnHandlingEsc -= OnUIHandledEsc;
    }

    private void OnUIHandledEsc()
    {
        _escHandledThisFrame = true;
    }

    private void LateUpdate()
    {
        _escHandledThisFrame = false;  // Reset for next frame
    }

    private void Start()
    {
        // Make sure panels are hidden at start
        if (_pausePanel != null)
            _pausePanel.SetActive(false);
        if (_optionsPanel != null)
            _optionsPanel.SetActive(false);

        // Set up button listeners with sound
        if (_resumeButton != null)
            _resumeButton.onClick.AddListener(() => {
                SoundManager.Instance.ButtonClick();
                ResumeGame();
            });

        if (_optionsButton != null)
            _optionsButton.onClick.AddListener(() => {
                SoundManager.Instance.ButtonClick();
                ShowOptions();
            });

        if (_mainMenuButton != null)
            _mainMenuButton.onClick.AddListener(() => {
                SoundManager.Instance.ButtonClick();
                ReturnToMainMenu();
            });

        if (_quitButton != null)
            _quitButton.onClick.AddListener(() => {
                SoundManager.Instance.ButtonClick();
                QuitGame();
            });
    }

    private bool CanPause()
    {
        GameObject marketPanel = GameObject.FindObjectOfType<Market>()?.MarketPanel;
        bool isMarketOpen = marketPanel != null && marketPanel.activeSelf;
        bool isInventoryOpen = InventorySystem.Instance.IsInventoryOpen;

        return !isMarketOpen && !isInventoryOpen;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_escHandledThisFrame)
            {
                return;  // Another UI already handled ESC this frame
            }

            if (_isPaused)
            {
                ResumeGame();
                return;
            }

            if (!CanPause())
            {
                return;
            }

            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (!CanPause()) return;

        _isPaused = true;
        Time.timeScale = 0f;
        _pausePanel.SetActive(true);
        GameManager.Instance.PauseGame();
    }

    public void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1f; // Unfreeze the game
        _pausePanel.SetActive(false);
        _optionsPanel.SetActive(false);
        GameManager.Instance.ResumeGame();
    }

    private void ShowOptions()
    {
        _pausePanel.SetActive(false);
        _optionsPanel.SetActive(true);
    }

    public void BackFromOptions()
    {
        _optionsPanel.SetActive(false);
        _pausePanel.SetActive(true);
    }

    private void ReturnToMainMenu()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        SoundManager.Instance.CrossfadeMusic(SoundManager.Instance.MainMenuMusic);
        SceneManager.LoadScene(0);
    }

    private void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
} 