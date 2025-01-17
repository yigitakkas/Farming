using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance;

    public AudioSource AudioSource;
    public AudioSource SfxSource;
    public AudioSource ClickSource;

    public AudioClip ButtonClickSound;

    public AudioClip MainMenuMusic;
    public List<AudioClip> GameMusics;

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
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        PlayMusic(MainMenuMusic, true);
        _originalVolume = AudioSource.volume;
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
            SoundManager.Instance.CrossfadeMusic(randomMusic);
        }
        else
        {
            Debug.LogWarning("GameMusics listesi bo� veya atanmad�!");
        }
    }

    public void CrossfadeMusic(AudioClip newClip, float fadeDuration = 0.5f)
    {
        if (_isMusicMuted) ToggleMusic();
        StartCoroutine(FadeOutAndIn(newClip, fadeDuration));
    }

    private IEnumerator FadeOutAndIn(AudioClip newClip, float duration)
    {
        float startVolume = AudioSource.volume;

        while (AudioSource.volume > 0)
        {
            AudioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        AudioSource.Stop();
        AudioSource.clip = newClip;
        AudioSource.Play();

        while (AudioSource.volume < startVolume)
        {
            AudioSource.volume += startVolume * Time.deltaTime / duration;
            yield return null;
        }
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
}
