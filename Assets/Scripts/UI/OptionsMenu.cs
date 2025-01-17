using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [Header("Volume Sliders")]
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    [Header("Volume Text (Optional)")]
    [SerializeField] private TextMeshProUGUI _musicVolumeText;
    [SerializeField] private TextMeshProUGUI _sfxVolumeText;

    [Header("UI References")]
    [SerializeField] private Button _backButton;
    [SerializeField] private GameObject _optionsPanel;  // Reference to this options panel
    [SerializeField] private GameObject _previousPanel; // Reference to panel that opened options (main menu or pause menu)

    private void Start()
    {
        // Set up initial slider values
        if (_musicSlider != null)
        {
            _musicSlider.value = SoundManager.Instance.AudioSource.volume;
            _musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (_sfxSlider != null)
        {
            _sfxSlider.value = SoundManager.Instance.SfxSource.volume;
            _sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        UpdateVolumeTexts();

        if (_backButton != null)
        {
            _backButton.onClick.AddListener(() => {
                SoundManager.Instance.ButtonClick();
                OnBackButton();
            });
        }
    }

    private void OnBackButton()
    {
        _optionsPanel.SetActive(false);
        _previousPanel.SetActive(true);
    }

    private void OnMusicVolumeChanged(float value)
    {
        SoundManager.Instance.AudioSource.volume = value;
        UpdateVolumeTexts();
    }

    private void OnSFXVolumeChanged(float value)
    {
        SoundManager.Instance.SfxSource.volume = value;
        SoundManager.Instance.ClickSource.volume = value;
        UpdateVolumeTexts();
    }


    private void UpdateVolumeTexts()
    {
        if (_musicVolumeText != null)
        {
            _musicVolumeText.text = $"Music: {(_musicSlider.value * 100):F0}%";
        }

        if (_sfxVolumeText != null)
        {
            _sfxVolumeText.text = $"SFX: {(_sfxSlider.value * 100):F0}%";
        }
    }
} 