using UnityEngine;
using System;

public class Crop : MonoBehaviour, IInteractable
{
    [Header("Crop Settings")]
    private string _cropId;
    private string _cropName;
    private float _baseValue;
    private int _growthStages;
    private float _timePerStage;
    private float _waterConsumptionPerDay;
    
    public void InitializeFromData(CropData data)
    {
        _cropId = data.CropId;
        _cropName = data.CropName;
        _baseValue = data.BaseValue;
        _growthStages = data.GrowthStages;
        _timePerStage = data.TimePerStage;
        _waterConsumptionPerDay = data.WaterConsumptionPerDay;
        
        // Reset growth state
        CurrentStage = 0;
        CurrentGrowthTime = 0;
        _waterLevel = 0;
        
        // Initialize models
        InitializeModels();
        UpdateVisuals();
    }
    
    private void InitializeModels()
    {
        // Instantiate models as children
        if (SmallModelPrefab) 
            _smallModel = Instantiate(SmallModelPrefab, transform.position, transform.rotation, transform);
        
        if (MediumModelPrefab)
            _mediumModel = Instantiate(MediumModelPrefab, transform.position, transform.rotation, transform);
        
        if (LargeModelPrefab)
            _largeModel = Instantiate(LargeModelPrefab, transform.position, transform.rotation, transform);
            
        // Set layers
        SetCropLayers();
        
        // Initialize water reminder
        if (WaterReminderPrefab != null)
        {
            _waterReminderInstance = Instantiate(WaterReminderPrefab, transform);
            _waterReminderInstance.transform.localPosition = Vector3.up * 1.5f; // Adjust height as needed
            _waterReminderInstance.SetActive(false);
        }
    }
    
    private void SetCropLayers()
    {
        gameObject.layer = LayerMask.NameToLayer("Crops");
        foreach (Transform child in transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Crops");
        }
    }
    
    [Header("Visual References")]
    public GameObject SmallModelPrefab;
    public GameObject MediumModelPrefab;
    public GameObject LargeModelPrefab;
    public GameObject WaterReminderPrefab;
    
    [Header("Current State")]
    [SerializeField] private float CurrentGrowthTime;
    [SerializeField] private int CurrentStage;
    [Header("Watering")]
    [SerializeField] private float _waterLevel = 0f;
    [SerializeField] private float _maxWaterLevel = 1f;
    private bool _isWatered;
    public bool IsWatered => _waterLevel > 0f;
    public bool IsFullyGrown => CurrentStage >= _growthStages - 1;
    
    // References to instantiated models
    private GameObject _smallModel;
    private GameObject _mediumModel;
    private GameObject _largeModel;
    private GameObject _waterReminderInstance;
    
    public event Action<Crop> OnCropHarvested;
    
    private void Awake()
    {
        // Only set initial values if not initialized through InitializeFromData
        if (string.IsNullOrEmpty(_cropId))
        {
            CurrentStage = 0;
            CurrentGrowthTime = 0;
            _waterLevel = 0;
        }
    }
    
    private void Start()
    {
        // Verify models were instantiated
        if (_smallModel == null || _mediumModel == null || _largeModel == null)
        {
            Debug.LogError($"Failed to instantiate model(s) for {gameObject.name}");
        }
        
        UpdateVisuals();
        
        // Make sure crop is on the Crops layer
        gameObject.layer = LayerMask.NameToLayer("Crops");
        
        // Set the layer for all child objects too
        foreach (Transform child in transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Crops");
        }
    }
    
    private void Update()
    {
        if (!GameManager.Instance.IsGamePaused)
        {
            if (IsWatered)
            {
                // Grow only when watered
                float growthProgress = Time.deltaTime / TimeManager.Instance.RealSecondsPerDay;
                CurrentGrowthTime += growthProgress;
                
                // Consume water
                float waterUsed = _waterConsumptionPerDay * (Time.deltaTime / TimeManager.Instance.RealSecondsPerDay);
                _waterLevel = Mathf.Max(0f, _waterLevel - waterUsed);
                
                // Update growth stage if needed
                if (CurrentGrowthTime >= _timePerStage)
                {
                    CurrentGrowthTime = 0;
                    CurrentStage++;
                    
                    if (CurrentStage >= _growthStages)
                    {
                        CurrentStage = _growthStages - 1;
                    }
                    
                    UpdateVisuals();
                }
            }
            
            // Always update visuals for water state
            UpdateWaterVisuals();
        }
    }
    
    public void Water()
    {
        _waterLevel = _maxWaterLevel;
        UpdateWaterVisuals();
    }
    
    private void UpdateWaterVisuals()
    {
        if (_waterReminderInstance != null)
        {
            _waterReminderInstance.SetActive(_waterLevel <= 0f);
            
            // Optional: Make it face the camera
            if (_waterReminderInstance.activeSelf)
            {
                _waterReminderInstance.transform.rotation = Camera.main.transform.rotation;
            }
        }
    }
    
    public bool IsReadyToHarvest()
    {
        // Check if the crop is fully grown
        bool isReady = IsFullyGrown;
        return isReady;
    }
    
    public void Harvest()
    {
        if (IsReadyToHarvest())
        {
            InventoryItem harvestedCrop = new InventoryItem(
                _cropId,
                _cropName,
                InventoryItem.ItemType.Crop,
                CropManager.Instance.GetCropIcon(_cropId),
                _baseValue
            );
            
            if (InventorySystem.Instance.AddItem(harvestedCrop))
            {
                OnCropHarvested?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }
    
    private void UpdateVisuals()
    {
        // Disable all models first
        if (_smallModel) _smallModel.SetActive(false);
        if (_mediumModel) _mediumModel.SetActive(false);
        if (_largeModel) _largeModel.SetActive(false);
        
        // Enable the appropriate model based on growth stage
        switch (CurrentStage)
        {
            case 0:
                if (_smallModel) _smallModel.SetActive(true);
                break;
            case 1:
                if (_mediumModel) _mediumModel.SetActive(true);
                break;
            case 2:
                if (_largeModel) _largeModel.SetActive(true);
                break;
        }
    }
    
    private void OnDestroy()
    {
        // Clean up instantiated models if they weren't already destroyed
        if (_smallModel) Destroy(_smallModel);
        if (_mediumModel) Destroy(_mediumModel);
        if (_largeModel) Destroy(_largeModel);
        if (_waterReminderInstance) Destroy(_waterReminderInstance);
    }
    
    public void Interact(PlayerInteraction player)
    {
        if (IsReadyToHarvest())
        {
            Harvest();
        }
    }
} 