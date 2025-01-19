using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public enum TimeOfDay
{
    Dawn,   // 5-7
    Morning,// 7-12
    Noon,   // 12-14
    Evening,// 14-19
    Dusk,   // 19-21
    Night   // 21-5
}

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    
    [Header("Time Settings")]
    [SerializeField] private float _realMinutesPerDay = 20f; // 20 real minutes = 1 game day
    [SerializeField] private float _startHour = 6f; // Start at 6 AM
    
    public float RealSecondsPerDay => _realMinutesPerDay * 60f;
    
    [Header("Lighting")]
    [SerializeField] private Light _directionalLight;
    [SerializeField] private Gradient _skyboxColor;
    [SerializeField] private Material _skyboxMaterial;

    private float _defaultLightIntensity = 1.1f;
    private Color _defaultSkyTint = new Color(0.5f, 0.5f, 0.5f);
    private float _defaultAtmosphereThickness = 0.6425911f;
    
    public float CurrentHour { get; private set; }
    public int CurrentDay { get; private set; }
    public event Action<float> OnHourChanged;
    public event Action<int> OnDayChanged;
    
    public TimeOfDay CurrentTimeOfDay
    {
        get
        {
            if (CurrentHour >= 5 && CurrentHour < 7) return TimeOfDay.Dawn;
            if (CurrentHour >= 7 && CurrentHour < 12) return TimeOfDay.Morning;
            if (CurrentHour >= 12 && CurrentHour < 14) return TimeOfDay.Noon;
            if (CurrentHour >= 14 && CurrentHour < 19) return TimeOfDay.Evening;
            if (CurrentHour >= 19 && CurrentHour < 21) return TimeOfDay.Dusk;
            return TimeOfDay.Night;
        }
    }
    
    public event Action<TimeOfDay> OnTimeOfDayChanged;
    private TimeOfDay _lastTimeOfDay;
    
    private float _nextLightingUpdate = 0f;
    private const float LIGHTING_UPDATE_INTERVAL = 0.5f; // Update every half second
    
    private Vector3 _currentLightRotation;
    private float _currentLightIntensity;
    private Color _currentSkyboxTint;
    private float _currentAtmosphereThickness;
    
    public float RealMinutesPerDay 
    { 
        get => _realMinutesPerDay;
        set => _realMinutesPerDay = value;
    }
    
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
        
        CurrentHour = _startHour;
        CurrentDay = 1;
        
        // Initialize lighting values
        _currentLightRotation = new Vector3((_startHour - 6) * 15f, -30f, 0f);
        _currentLightIntensity = _defaultLightIntensity;
        _currentSkyboxTint = _defaultSkyTint;
        _currentAtmosphereThickness = _defaultAtmosphereThickness;
    }
    
    private void Update()
    {
        if (!GameManager.Instance.IsGamePaused)
        {
            // Calculate how much of a day passes in one real second
            float dayProgress = Time.deltaTime / RealSecondsPerDay;
            
            // Convert to hours (24 hours per day)
            float hourChange = dayProgress * 24f;
            
            // Update current hour
            float previousHour = CurrentHour;
            CurrentHour += hourChange;
            
            // Handle day change
            if (CurrentHour >= 24f)
            {
                CurrentHour -= 24f;
                CurrentDay++;
                OnDayChanged?.Invoke(CurrentDay);
            }
            
            // Update lighting less frequently
            if (Time.time >= _nextLightingUpdate)
            {
                UpdateLighting();
                _nextLightingUpdate = Time.time + LIGHTING_UPDATE_INTERVAL;
            }
            
            // Smooth transitions between updates
            ApplyLightingTransitions();
            
            // Notify hour change
            if (Mathf.Floor(previousHour) != Mathf.Floor(CurrentHour))
            {
                OnHourChanged?.Invoke(CurrentHour);
            }
            
            // Check for time of day change
            TimeOfDay currentTimeOfDay = CurrentTimeOfDay;
            if (currentTimeOfDay != _lastTimeOfDay)
            {
                _lastTimeOfDay = currentTimeOfDay;
                OnTimeOfDayChanged?.Invoke(currentTimeOfDay);
            }
        }
    }
    
    private void UpdateLighting()
    {
        float sunRotation = (CurrentHour - 6) * 15f;
        _currentLightRotation = new Vector3(sunRotation, -30f, 0f);
        
        float timeOfDay = CurrentHour / 24f;
        
        // Adjust light intensity based on time of day
        _currentLightIntensity = Mathf.Lerp(0.1f, 1f, (Mathf.Sin((timeOfDay * 2f - 0.5f) * Mathf.PI) + 1f) * 0.5f);
        
        // Update sky color and atmosphere
        _currentSkyboxTint = _skyboxColor.Evaluate(timeOfDay);
        _currentAtmosphereThickness = Mathf.Lerp(1f, 0.5f, Mathf.Abs(Mathf.Sin(timeOfDay * Mathf.PI))); // Thicker at noon, thinner at night
    }
    
    private void ApplyLightingTransitions()
    {
        // Smooth rotation transition
        _directionalLight.transform.rotation = Quaternion.Lerp(
            _directionalLight.transform.rotation,
            Quaternion.Euler(_currentLightRotation),
            Time.deltaTime * 2f
        );
        
        // Smooth intensity transition
        _directionalLight.intensity = Mathf.Lerp(
            _directionalLight.intensity,
            _currentLightIntensity,
            Time.deltaTime * 2f
        );
        
        // Smooth skybox transitions
        _skyboxMaterial.SetColor("_SkyTint", Color.Lerp(
            _skyboxMaterial.GetColor("_SkyTint"),
            _currentSkyboxTint,
            Time.deltaTime * 2f
        ));
        
        _skyboxMaterial.SetFloat("_AtmosphereThickness", Mathf.Lerp(
            _skyboxMaterial.GetFloat("_AtmosphereThickness"),
            _currentAtmosphereThickness,
            Time.deltaTime * 2f
        ));
    }
    
    public void SetTime(float hour)
    {
        CurrentHour = hour;
        OnHourChanged?.Invoke(CurrentHour);
        
        // Update time of day
        TimeOfDay newTimeOfDay = CurrentTimeOfDay;
        if (newTimeOfDay != _lastTimeOfDay)
        {
            _lastTimeOfDay = newTimeOfDay;
            OnTimeOfDayChanged?.Invoke(newTimeOfDay);
        }
    }
    
    public void AdvanceDay()
    {
        CurrentDay++;
        OnDayChanged?.Invoke(CurrentDay);
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
        if (scene.buildIndex == 1)
        {
            Invoke(nameof(ResetManager), 0.1f);

            CurrentHour = _startHour;
            _currentLightRotation = new Vector3((_startHour - 6) * 15f, -30f, 0f);
            _currentLightIntensity = _defaultLightIntensity;
            _currentSkyboxTint = _defaultSkyTint;
            _currentAtmosphereThickness = _defaultAtmosphereThickness;


            ApplyLightingImmediately();


            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
            }
        }
    }

    public void ResetManager()
    {
        CurrentHour = _startHour;
        CurrentDay = 1;

        _currentLightRotation = new Vector3((_startHour - 6) * 15f, -30f, 0f);
        _currentLightIntensity = _defaultLightIntensity;
        _currentSkyboxTint = _defaultSkyTint;
        _currentAtmosphereThickness = _defaultAtmosphereThickness;

        ApplyLightingImmediately();
    }


    private void ApplyLightingImmediately()
    {
        // Apply directional light settings
        _directionalLight.transform.rotation = Quaternion.Euler(_currentLightRotation);
        _directionalLight.intensity = _currentLightIntensity;

        // Apply skybox settings
        _skyboxMaterial.SetColor("_SkyTint", _currentSkyboxTint);
        _skyboxMaterial.SetFloat("_AtmosphereThickness", _currentAtmosphereThickness);
    }
} 