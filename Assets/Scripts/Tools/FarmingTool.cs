using UnityEngine;
using System.Collections;

public class FarmingTool : Tool
{
    public enum FarmingToolType
    {
        Planting,    // For planting only
        Harvesting,  // For harvesting only
        Watering,    // For watering only
        Cultivator   // For both planting and harvesting
    }
    
    [Header("Tool Type")]
    public FarmingToolType ToolType;
    
    [Header("Interaction Settings")]
    public LayerMask PlantingLayer;
    public LayerMask CropLayer;
    public float PlantingHeight = 0f;
    
    [Header("Visual Feedback")]
    public GameObject PlantingPreviewPrefab;
    public GameObject HarvestPreviewPrefab;
    public GameObject WateringPreviewPrefab;
    public ParticleSystem WaterParticles;
    
    private PlantingPreview _plantingPreview;
    private HarvestPreview _harvestPreview;
    private WateringPreview _wateringPreview;
    private FarmingActions _actions;
    private PlayerController _playerController;
    
    private void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var playerController = player.GetComponent<PlayerController>();
        Transform groundCheck = playerController?.GroundCheck;
        
        _actions = GetComponent<FarmingActions>();
        _actions.Initialize(_attributes);
        
        InitializePreviews(groundCheck);
        _playerController = playerController;
        
        // Make sure water particles are off at start
        if (WaterParticles != null)
        {
            WaterParticles.Stop();
        }
    }
    
    private void InitializePreviews(Transform groundCheck)
    {
        switch (ToolType)
        {
            case FarmingToolType.Planting:
            case FarmingToolType.Cultivator:
                _plantingPreview = gameObject.AddComponent<PlantingPreview>();
                _plantingPreview.Initialize(groundCheck, UseRange, PlantingPreviewPrefab, PlantingLayer, PlantingHeight);
                break;
        }
        
        if (ToolType == FarmingToolType.Harvesting || ToolType == FarmingToolType.Cultivator)
        {
            _harvestPreview = gameObject.AddComponent<HarvestPreview>();
            _harvestPreview.Initialize(groundCheck, UseRange, HarvestPreviewPrefab, CropLayer);
        }
        
        if (ToolType == FarmingToolType.Watering)
        {
            _wateringPreview = gameObject.AddComponent<WateringPreview>();
            _wateringPreview.Initialize(groundCheck, UseRange, WateringPreviewPrefab, CropLayer);
        }
    }
    
    private void Update()
    {
        base.Update();
        
        switch (ToolType)
        {
            case FarmingToolType.Planting:
                if (InventorySystem.Instance.GetSelectedSeed() != null)
                {
                    _plantingPreview?.UpdatePreview();
                }
                else
                {
                    _plantingPreview?.Hide();
                }
                break;
            
            case FarmingToolType.Harvesting:
                _harvestPreview?.UpdatePreview();
                break;
            
            case FarmingToolType.Cultivator:
                bool isShowingHarvestPreview = _harvestPreview?.CanHarvestHere ?? false;
                _harvestPreview?.UpdatePreview();
                
                if (InventorySystem.Instance.GetSelectedSeed() != null)
                {
                    if (!isShowingHarvestPreview)
                    {
                        _plantingPreview?.UpdatePreview();
                    }
                    else
                    {
                        _plantingPreview?.Hide();
                    }
                }
                else
                {
                    _plantingPreview?.Hide();
                }
                break;
            
            case FarmingToolType.Watering:
                _wateringPreview?.UpdatePreview();
                break;
        }
    }
    
    public override void UseTool(Vector3 usePosition)
    {
        if (!_canUse) return;
        
        switch (ToolType)
        {
            case FarmingToolType.Planting:
                if (_plantingPreview?.CanPlantHere ?? false)
                {
                    FaceTarget(_plantingPreview.PreviewPosition);
                    _playerController?.TriggerPlantingAnimation();
                    _actions.HandlePlanting(_plantingPreview.PreviewPosition, 
                        InventorySystem.Instance.GetSelectedSeed());
                }
                break;
                
            case FarmingToolType.Harvesting:
                if (_harvestPreview?.CanHarvestHere ?? false)
                {
                    FaceTarget(_harvestPreview.TargetCrop.transform.position);
                    _playerController?.TriggerHarvestingAnimation();
                    _actions.HandleHarvesting(_harvestPreview.TargetCrop);
                }
                break;
                
            case FarmingToolType.Cultivator:
                if (_harvestPreview?.CanHarvestHere ?? false)
                {
                    _playerController?.TriggerHarvestingAnimation();
                    _actions.HandleHarvesting(_harvestPreview.TargetCrop);
                }
                else if (_plantingPreview?.CanPlantHere ?? false)
                {
                    _playerController?.TriggerPlantingAnimation();
                    _actions.HandlePlanting(_plantingPreview.PreviewPosition,
                        InventorySystem.Instance.GetSelectedSeed());
                }
                break;
                
            case FarmingToolType.Watering:
                if (_wateringPreview?.CanWaterHere ?? false)
                {
                    StartCoroutine(HandleWateringEffect(_wateringPreview.TargetCrop));
                }
                break;
        }
        
        _canUse = false;
        _useTimer = 0;
    }
    
    private IEnumerator HandleWateringEffect(Crop targetCrop)
    {
        if (_playerController != null)
        {
            // Make player face the crop
            Vector3 directionToCrop = (targetCrop.transform.position - _playerController.transform.position).normalized;
            directionToCrop.y = 0; // Keep rotation only on horizontal plane
            _playerController.ModelTransform.rotation = Quaternion.LookRotation(directionToCrop);

            _playerController.TriggerWateringAnimation();
        }
        
        // Wait for animation to be roughly halfway
        yield return new WaitForSeconds(0.5f);
        
        // Apply the watering effect
        _actions.HandleWatering(targetCrop, WaterParticles);
    }
    
    public override void OnEquip()
    {
        base.OnEquip();
        _plantingPreview?.Hide();
        _harvestPreview?.Hide();
        _wateringPreview?.Hide();
    }
    
    public override void OnUnequip()
    {
        base.OnUnequip();
        _plantingPreview?.Hide();
        _harvestPreview?.Hide();
        _wateringPreview?.Hide();
    }
    
    private void OnDestroy()
    {
        if (_plantingPreview != null) Destroy(_plantingPreview);
        if (_harvestPreview != null) Destroy(_harvestPreview);
        if (_wateringPreview != null) Destroy(_wateringPreview);
        if (_actions != null) Destroy(_actions);
    }
    
    private void FaceTarget(Vector3 targetPosition)
    {
        if (_playerController != null)
        {
            Vector3 direction = (targetPosition - _playerController.transform.position).normalized;
            direction.y = 0;
            _playerController.ModelTransform.rotation = Quaternion.LookRotation(direction);
        }
    }
} 