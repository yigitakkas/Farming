using UnityEngine;

public class FarmingTool : Tool
{
    [Header("Tool Type")]
    public FarmingToolType ToolType;
    
    public enum FarmingToolType
    {
        Spade,      // For planting
        Watering,   // For watering
        Scythe,     // For harvesting
        Hoe         // For both planting and harvesting
    }
    
    [Header("Interaction Settings")]
    public LayerMask PlantingLayer;
    public LayerMask CropLayer;
    public float PlantingHeight = 0f;
    
    [Header("Visual Feedback")]
    public GameObject PlantingPreview;
    public GameObject HarvestPreview;
    private GameObject _currentPreview;
    private GameObject _harvestPreview;
    private bool _canPlantHere;
    private Transform _groundCheck;
    
    [Header("Watering Settings")]
    public GameObject WateringPreview;
    public float WateringRadius = 1f; // Area of effect for watering
    public ParticleSystem WaterParticles;
    
    private GameObject _wateringPreview;
    
    private void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            _groundCheck = playerController.GroundCheck;
        }
        
        if (PlantingPreview != null)
        {
            _currentPreview = Instantiate(PlantingPreview);
            _currentPreview.SetActive(false);
            
            // Put preview on a different layer
            _currentPreview.layer = LayerMask.NameToLayer("Ignore Raycast");
            
            // Set layer for all children too
            foreach (Transform child in _currentPreview.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }
        }
        
        if (HarvestPreview != null)
        {
            _harvestPreview = Instantiate(HarvestPreview);
            _harvestPreview.SetActive(false);
            
            // Put on ignore raycast layer
            _harvestPreview.layer = LayerMask.NameToLayer("Ignore Raycast");
            foreach (Transform child in _harvestPreview.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }
        }
        
        if (WateringPreview != null)
        {
            _wateringPreview = Instantiate(WateringPreview);
            _wateringPreview.SetActive(false);
            
            // Put on ignore raycast layer
            _wateringPreview.layer = LayerMask.NameToLayer("Ignore Raycast");
            foreach (Transform child in _wateringPreview.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }
        }
        
        // Validate icon reference
        if (_toolIcon == null)
        {
            Debug.LogError($"Missing tool icon for {gameObject.name}");
        }
    }
    
    private void Update()
    {
        base.Update();
        
        // Clear all previews first
        HideAllPreviews();
        
        // Show appropriate previews based on tool type
        switch (ToolType)
        {
            case FarmingToolType.Spade:
                if (InventorySystem.Instance.GetSelectedSeed() != null)
                {
                    UpdatePlantingPreview();
                }
                break;
            
            case FarmingToolType.Scythe:
                UpdateHarvestPreview();
                break;
            
            case FarmingToolType.Hoe:
                bool isShowingHarvestPreview = UpdateHarvestPreview();
                if (!isShowingHarvestPreview && InventorySystem.Instance.GetSelectedSeed() != null)
                {
                    UpdatePlantingPreview();
                }
                break;
            
            case FarmingToolType.Watering:
                UpdateWateringPreview();
                break;
        }
    }
    
    private void HideAllPreviews()
    {
        if (_currentPreview != null) _currentPreview.SetActive(false);
        if (_harvestPreview != null) _harvestPreview.SetActive(false);
        if (_wateringPreview != null) _wateringPreview.SetActive(false);
    }
    
    private void UpdatePlantingPreview()
    {
        if (_currentPreview == null || _groundCheck == null) return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
        
        foreach (var hit in hits)
        {
            if (((1 << hit.collider.gameObject.layer) & PlantingLayer) != 0)
            {
                Vector3 feetPosition = _groundCheck.position;
                float distanceXZ = Vector3.Distance(
                    new Vector3(feetPosition.x, 0, feetPosition.z),
                    new Vector3(hit.point.x, 0, hit.point.z)
                );
                
                if (distanceXZ <= UseRange)
                {
                    _canPlantHere = true;
                    _currentPreview.SetActive(true);
                    _currentPreview.transform.position = hit.point + Vector3.up * PlantingHeight;
                    
                    if (_currentPreview.TryGetComponent<Renderer>(out var renderer))
                    {
                        renderer.material.color = _canUse ? Color.green : Color.red;
                    }
                    return;
                }
            }
        }
        
        _canPlantHere = false;
    }
    
    private bool UpdateHarvestPreview()
    {
        if (_harvestPreview == null) return false;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, CropLayer))
        {
            Vector3 feetPosition = _groundCheck.position;
            float distanceXZ = Vector3.Distance(
                new Vector3(feetPosition.x, 0, feetPosition.z),
                new Vector3(hit.point.x, 0, hit.point.z)
            );
            
            Crop crop = hit.collider.GetComponent<Crop>();
            if (crop != null && crop.IsReadyToHarvest() && distanceXZ <= UseRange)
            {
                _harvestPreview.SetActive(true);
                _harvestPreview.transform.position = hit.point + Vector3.up * 0.1f;
                
                if (_harvestPreview.TryGetComponent<Renderer>(out var renderer))
                {
                    renderer.material.color = _canUse ? Color.yellow : Color.red;
                }
                return true;
            }
        }
        
        _harvestPreview.SetActive(false);
        return false;
    }
    
    private void UpdateWateringPreview()
    {
        if (_wateringPreview == null || _groundCheck == null) return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, CropLayer))
        {
            Vector3 feetPosition = _groundCheck.position;
            float distanceXZ = Vector3.Distance(
                new Vector3(feetPosition.x, 0, feetPosition.z),
                new Vector3(hit.point.x, 0, hit.point.z)
            );
            
            if (distanceXZ <= UseRange)
            {
                _wateringPreview.SetActive(true);
                _wateringPreview.transform.position = hit.point;
                
                // Optional: Show which crops will be affected
                ShowWateringRadius(hit.point);
            }
        }
    }
    
    private void ShowWateringRadius(Vector3 center)
    {
        // Find all crops within radius
        Collider[] hitColliders = Physics.OverlapSphere(center, WateringRadius, CropLayer);
        foreach (var collider in hitColliders)
        {
            Crop crop = collider.GetComponent<Crop>();
            if (crop != null)
            {
                // Optional: Highlight crops that will be watered
            }
        }
    }
    
    public override void UseTool(Vector3 usePosition)
    {
        if (!_canUse) return;
        
        switch (ToolType)
        {
            case FarmingToolType.Spade:
                HandlePlanting();
                break;
            case FarmingToolType.Scythe:
                HandleHarvesting(usePosition);
                break;
            case FarmingToolType.Hoe:
                // Try harvesting first, if no harvestable crop is found, try planting
                if (!TryHarvesting(usePosition))
                {
                    HandlePlanting();
                }
                break;
            case FarmingToolType.Watering:
                HandleWatering(usePosition);
                break;
        }
    }
    
    private void HandlePlanting()
    {
        if (!_canPlantHere || !_currentPreview.activeSelf) return;
        
        // Get selected seed from inventory
        InventoryItem selectedSeed = InventorySystem.Instance.GetSelectedSeed();
        if (selectedSeed == null || selectedSeed.Type != InventoryItem.ItemType.Seed) return;
        
        // Get crop prefab and data from seed ID
        string cropId = selectedSeed.ItemId.Replace("_seed", "");
        GameObject cropPrefab = CropManager.Instance.GetCropPrefab(selectedSeed.ItemId);
        CropData cropData = CropManager.Instance.GetCropData(cropId);
        
        if (cropPrefab == null || cropData == null) return;
        
        // Plant the crop and initialize it
        Vector3 plantPosition = _currentPreview.transform.position;
        GameObject newCropObj = Instantiate(cropPrefab, plantPosition, Quaternion.identity);
        
        Crop newCrop = newCropObj.GetComponent<Crop>();
        if (newCrop != null)
        {
            newCrop.InitializeFromData(cropData);
        }
        
        // Remove seed from inventory
        InventorySystem.Instance.RemoveItem(selectedSeed.ItemId);
        
        _canUse = false;
        _useTimer = 0;
    }
    
    private void HandleHarvesting(Vector3 usePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit cropHit, 100f, CropLayer))
        {
            // Check XZ distance
            Vector3 feetPosition = _groundCheck.position;
            float distanceXZ = Vector3.Distance(
                new Vector3(feetPosition.x, 0, feetPosition.z),
                new Vector3(cropHit.point.x, 0, cropHit.point.z)
            );
            
            if (distanceXZ <= UseRange)
            {
                Crop crop = cropHit.collider.GetComponent<Crop>();
                if (crop != null)
                {
                    crop.Harvest();
                    _canUse = false;
                    _useTimer = 0;
                }
            }
        }
    }
    
    private bool TryHarvesting(Vector3 usePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit cropHit, 100f, CropLayer))
        {
            Vector3 feetPosition = _groundCheck.position;
            float distanceXZ = Vector3.Distance(
                new Vector3(feetPosition.x, 0, feetPosition.z),
                new Vector3(cropHit.point.x, 0, cropHit.point.z)
            );
            
            if (distanceXZ <= UseRange)
            {
                Crop crop = cropHit.collider.GetComponent<Crop>();
                if (crop != null && crop.IsReadyToHarvest())
                {
                    crop.Harvest();
                    _canUse = false;
                    _useTimer = 0;
                    return true;
                }
            }
        }
        
        return false;
    }
    
    private void HandleWatering(Vector3 usePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, CropLayer))
        {
            Vector3 feetPosition = _groundCheck.position;
            float distanceXZ = Vector3.Distance(
                new Vector3(feetPosition.x, 0, feetPosition.z),
                new Vector3(hit.point.x, 0, hit.point.z)
            );
            
            if (distanceXZ <= UseRange)
            {
                // Water all crops in radius
                Collider[] hitColliders = Physics.OverlapSphere(hit.point, WateringRadius, CropLayer);
                foreach (var collider in hitColliders)
                {
                    Crop crop = collider.GetComponent<Crop>();
                    if (crop != null)
                    {
                        crop.Water();
                    }
                }
                
                // Play particle effect
                if (WaterParticles != null)
                {
                    WaterParticles.transform.position = hit.point;
                    WaterParticles.Play();
                }
                
                _canUse = false;
                _useTimer = 0;
            }
        }
    }
    
    public override void OnEquip()
    {
        base.OnEquip();
        if (_currentPreview != null)
        {
            _currentPreview.SetActive(true);
        }
        if (_harvestPreview != null)
        {
            _harvestPreview.SetActive(false);  // Start with it hidden
        }
    }
    
    public override void OnUnequip()
    {
        base.OnUnequip();
        if (_currentPreview != null)
        {
            _currentPreview.SetActive(false);
        }
        if (_harvestPreview != null)
        {
            _harvestPreview.SetActive(false);
        }
    }
    
    private void OnDestroy()
    {
        if (_currentPreview != null)
        {
            Destroy(_currentPreview);
        }
        if (_harvestPreview != null)
        {
            Destroy(_harvestPreview);
        }
        if (_wateringPreview != null)
        {
            Destroy(_wateringPreview);
        }
    }
} 