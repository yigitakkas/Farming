using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _backButton;
    
    [Header("Panel References")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _optionsPanel;

    private void Start()
    {
        // Always ensure time scale is 1 when entering main menu
        Time.timeScale = 1f;

        // Make sure options panel is hidden at start
        if (_optionsPanel != null)
        {
            _optionsPanel.SetActive(false);
        }

        // Make sure main menu is visible at start
        if (_mainMenuPanel != null)
        {
            _mainMenuPanel.SetActive(true);
        }

        // Set up button listeners
        if (_playButton != null)
        {
            _playButton.onClick.AddListener(() => {
                PlayButtonSound();
                OnPlayButton();
            });
        }

        if (_optionsButton != null)
        {
            _optionsButton.onClick.AddListener(() => {
                PlayButtonSound();
                OnOptionsButton();
            });
        }

        if (_quitButton != null)
        {
            _quitButton.onClick.AddListener(() => {
                PlayButtonSound();
                OnQuitButton();
            });
        }

        if (_backButton != null)
        {
            _backButton.onClick.AddListener(() => {
                PlayButtonSound();
                OnBackButton();
            });
        }
    }

    private void PlayButtonSound()
    {
        SoundManager.Instance.ButtonClick();
    }

    private void OnPlayButton()
    {
        SceneManager.LoadScene(1);
        SoundManager.Instance.PlayRandomGameMusic();
    }

    private void OnOptionsButton()
    {
        if (_optionsPanel != null && _mainMenuPanel != null)
        {
            _optionsPanel.SetActive(true);
            _mainMenuPanel.SetActive(false);
        }
    }

    private void OnBackButton()
    {
        if (_optionsPanel != null && _mainMenuPanel != null)
        {
            _optionsPanel.SetActive(false);
            _mainMenuPanel.SetActive(true);
        }
    }

    private void OnQuitButton()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
} 