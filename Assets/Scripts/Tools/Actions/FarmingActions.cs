using UnityEngine;

public class FarmingActions : MonoBehaviour
{
    private ToolAttributes _attributes;
    
    public void Initialize(ToolAttributes attributes)
    {
        _attributes = attributes;
    }
    
    public void HandlePlanting(Vector3 position, InventoryItem selectedSeed)
    {
        if (selectedSeed == null || selectedSeed.Type != InventoryItem.ItemType.Seed) return;
        
        string cropId = selectedSeed.ItemId.Replace("_seed", "");
        GameObject cropPrefab = CropManager.Instance.GetCropPrefab(selectedSeed.ItemId);
        CropData cropData = CropManager.Instance.GetCropData(cropId);
        
        if (cropPrefab == null || cropData == null) return;
        
        cropData = ModifyCropData(cropData);
        
        GameObject newCropObj = Instantiate(cropPrefab, position, Quaternion.identity);
        Crop newCrop = newCropObj.GetComponent<Crop>();
        if (newCrop != null)
        {
            newCrop.InitializeFromData(cropData);
        }
        
        InventorySystem.Instance.RemoveItem(selectedSeed.ItemId);
    }
    
    public void HandleHarvesting(Crop crop)
    {
        if (crop == null || !crop.IsReadyToHarvest()) return;
        
        int baseQuantity = 1;
        int modifiedQuantity = Mathf.RoundToInt(baseQuantity * _attributes.HarvestQuantityMultiplier);
        
        for (int i = 0; i < modifiedQuantity; i++)
        {
            crop.Harvest();
        }
    }
    
    public void HandleWatering(Crop crop, ParticleSystem waterParticles)
    {
        if (crop == null) return;
        
        crop.Water();
        
        if (waterParticles != null)
        {
            waterParticles.transform.position = crop.transform.position;
            waterParticles.Play();
        }
    }
    
    private CropData ModifyCropData(CropData originalData)
    {
        return new CropData
        {
            CropId = originalData.CropId,
            CropName = originalData.CropName,
            CropPrefab = originalData.CropPrefab,
            CropIcon = originalData.CropIcon,
            SeedIcon = originalData.SeedIcon,
            BaseValue = originalData.BaseValue,
            SeedValue = originalData.SeedValue,
            GrowthStages = originalData.GrowthStages,
            TimePerStage = originalData.TimePerStage * _attributes.GrowthTimeMultiplier,
            WaterConsumptionPerDay = originalData.WaterConsumptionPerDay * _attributes.WaterConsumptionMultiplier
        };
    }
} 