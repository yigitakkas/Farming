using UnityEngine;
using System.Collections;

public class FarmingActions : MonoBehaviour
{
    [Header("Sound Effects")]
    [SerializeField] private AudioClip _plantingSoundEffect;
    [SerializeField] private AudioClip _harvestingSoundEffect;
    [SerializeField] private AudioClip _wateringSoundEffect;
    
    public static event System.Action OnPlantingComplete;
    public static event System.Action OnWateringComplete;

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
        
        // Play planting sound
        if (_plantingSoundEffect != null)
        {
            SoundManager.Instance.PlaySFX(_plantingSoundEffect);
        }
        
        cropData = ModifyCropData(cropData);
        
        GameObject newCropObj = Instantiate(cropPrefab, position, Quaternion.identity);
        Crop newCrop = newCropObj.GetComponent<Crop>();
        if (newCrop != null)
        {
            newCrop.InitializeFromData(cropData);
            OnPlantingComplete?.Invoke();
        }
        
        InventorySystem.Instance.RemoveItem(selectedSeed.ItemId);
    }
    
    public void HandleHarvesting(Crop crop)
    {
        if (crop == null || !crop.IsReadyToHarvest()) return;
        
        // Play harvesting sound
        if (_harvestingSoundEffect != null)
        {
            SoundManager.Instance.PlaySFX(_harvestingSoundEffect);
        }
        
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
        
        // Play watering sound
        if (_wateringSoundEffect != null)
        {
            SoundManager.Instance.PlaySFX(_wateringSoundEffect);
        }
        
        if (waterParticles != null)
        {
            waterParticles.transform.position = crop.transform.position + Vector3.up * 0.5f;
            waterParticles.Play();
            
            StartCoroutine(StopWaterParticles(waterParticles));
        }
        
        crop.Water();
        OnWateringComplete?.Invoke();
    }
    
    private IEnumerator StopWaterParticles(ParticleSystem particles)
    {
        yield return new WaitForSeconds(0.5f);
        if (particles != null)
        {
            particles.Stop();
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