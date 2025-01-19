using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance;

    public AudioSource AudioSource;
    public AudioSource SfxSource;
    public AudioSource ClickSource;

    public AudioClip ButtonClickSound;

    public AudioClip MainMenuMusic;
    public List<AudioClip> GameMusics;

    [Header("UI Sounds")]
    public AudioClip ErrorSound;
    public AudioClip MoneyUpSound;

    private bool _isMusicMuted = false;

    private int _lastPlayedIndex = -1;
    private float _originalVolume=0.06f;
    private float _loweredVolume = 0.03f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Load saved volume settings
            LoadVolumeSettings();
            
            // Start main menu music at saved volume
            if (AudioSource != null && MainMenuMusic != null)
            {
                AudioSource.clip = MainMenuMusic;
                AudioSource.loop = true;
                AudioSource.Play();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(AudioClip musicClip, bool loop = true)
    {
        if(AudioSource!=null)
        {
            AudioSource.clip = musicClip;
            AudioSource.loop = loop;
            AudioSource.Play();
        }
    }

    public void ToggleMusic()
    {
        _isMusicMuted = !_isMusicMuted;
        AudioSource.mute = _isMusicMuted;
    }

    public void LowerMusicVolume()
    {
        AudioSource.volume = _loweredVolume;
    }

    public void RestoreOriginalVolume()
    {
        AudioSource.volume = _originalVolume;
    }

    public void StopMusic(float fadeDuration)
    {
        StartCoroutine(FadeOutMusic(fadeDuration));
    }
    public void PlaySFX(AudioClip sfxClip)
    {
        if(SfxSource!=null)
            SfxSource.PlayOneShot(sfxClip);
    }

    public void PlayRandomGameMusic()
    {
        if (GameMusics != null && GameMusics.Count > 0)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, GameMusics.Count);
            }
            while (randomIndex == _lastPlayedIndex);

            _lastPlayedIndex = randomIndex;

            AudioClip randomMusic = GameMusics[randomIndex];
            CrossfadeMusic(randomMusic);
        }
        else
        {
            Debug.LogWarning("GameMusics listesi boş veya atanmadı!");
        }
    }

    public void CrossfadeMusic(AudioClip newClip, float fadeDuration = 0.5f)
    {
        if (_isMusicMuted) ToggleMusic();
        
        // Keep current volume throughout the transition
        float savedVolume = AudioSource.volume;
        StartCoroutine(FadeOutAndIn(newClip, fadeDuration, savedVolume));
    }

    private IEnumerator FadeOutAndIn(AudioClip newClip, float duration, float savedVolume)
    {
        // Fade out while preserving saved volume
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            AudioSource.volume = Mathf.Lerp(savedVolume, 0, timer / duration);
            yield return null;
        }

        // Change clip
        AudioSource.Stop();
        AudioSource.clip = newClip;
        AudioSource.Play();
        
        // Fade back to saved volume
        timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            AudioSource.volume = Mathf.Lerp(0, savedVolume, timer / duration);
            yield return null;
        }
        
        // Ensure we end at exactly the saved volume
        AudioSource.volume = savedVolume;
    }

    private IEnumerator FadeOutMusic(float duration)
    {
        float startVolume = AudioSource.volume;
        while (AudioSource.volume > 0)
        {
            AudioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        AudioSource.Stop();
        AudioSource.volume = startVolume; 
    }

    public void ButtonClick()
    {
        ClickSource.PlayOneShot(ButtonClickSound);
    }

    public void SetMusicVolume(float volume)
    {
        AudioSource.volume = volume;
        SaveVolumeSettings();
    }

    public void SetSFXVolume(float volume)
    {
        SfxSource.volume = volume;
        ClickSource.volume = volume;
        SaveVolumeSettings();
    }

    private void LoadVolumeSettings()
    {
        AudioSource.volume = PlayerPrefs.GetFloat("MusicVolume", _originalVolume);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        SfxSource.volume = sfxVolume;
        ClickSource.volume = sfxVolume;
    }

    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", AudioSource.volume);
        PlayerPrefs.SetFloat("SFXVolume", SfxSource.volume);
        PlayerPrefs.Save();
    }
}
