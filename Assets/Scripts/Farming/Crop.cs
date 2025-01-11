using UnityEngine;
using System;

public class Crop : MonoBehaviour, IInteractable
{
    [Header("Crop Settings")]
    public string CropId;
    public string CropName;
    public int GrowthStages = 3;
    public float TimePerStage = 0.5f;
    public float WaterConsumptionPerDay = 0.2f;
    public float BaseValue = 10f;
    
    [Header("Visual References")]
    public GameObject SmallModelPrefab;
    public GameObject MediumModelPrefab;
    public GameObject LargeModelPrefab;
    public Sprite CropIcon;
    
    [Header("Current State")]
    [SerializeField] private float CurrentGrowthTime;
    [SerializeField] private int CurrentStage;
    public bool IsWatered;
    public bool IsFullyGrown => CurrentStage >= GrowthStages - 1;
    
    // References to instantiated models
    private GameObject _smallModel;
    private GameObject _mediumModel;
    private GameObject _largeModel;
    
    public event Action<Crop> OnCropHarvested;
    
    private void Awake()
    {
        CurrentStage = 0;
        CurrentGrowthTime = 0;
        IsWatered = true;
        
        // Instantiate models as children
        if (SmallModelPrefab) 
            _smallModel = Instantiate(SmallModelPrefab, transform.position, transform.rotation, transform);
        
        if (MediumModelPrefab)
            _mediumModel = Instantiate(MediumModelPrefab, transform.position, transform.rotation, transform);
        
        if (LargeModelPrefab)
            _largeModel = Instantiate(LargeModelPrefab, transform.position, transform.rotation, transform);
    }
    
    private void Start()
    {
        // Verify models were instantiated
        if (_smallModel == null || _mediumModel == null || _largeModel == null)
        {
            Debug.LogError($"Failed to instantiate model(s) for {gameObject.name}");
        }
        
        UpdateVisuals();
    }
    
    private void Update()
    {
        if (!IsFullyGrown && IsWatered)
        {
            // Progress growth based on game time
            CurrentGrowthTime += Time.deltaTime / (GameManager.Instance.DayLength * 60);
            
            // Check if we should advance to next stage
            if (CurrentGrowthTime >= TimePerStage)
            {
                CurrentGrowthTime = 0;
                CurrentStage++;
                UpdateVisuals();
            }
        }
    }
    
    public void Water()
    {
        IsWatered = true;
    }
    
    public bool IsReadyToHarvest()
    {
        return IsFullyGrown;
    }
    
    public void Harvest()
    {
        if (IsReadyToHarvest())
        {
            // Create inventory item with icon
            InventoryItem harvestedCrop = new InventoryItem(
                CropId,
                CropName,
                InventoryItem.ItemType.Crop,
                CropIcon,
                BaseValue
            );
            
            // Add to inventory
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
    }
    
    public void Interact(PlayerInteraction player)
    {
        if (IsReadyToHarvest())
        {
            Harvest();
        }
    }
} 