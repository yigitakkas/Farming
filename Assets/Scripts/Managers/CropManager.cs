using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CropData
{
    public string CropId;
    public string CropName;
    public GameObject CropPrefab;
    public Sprite CropIcon;
    public Sprite SeedIcon;
    public float BaseValue;
    public float SeedValue;
    
    public int GrowthStages = 3;
    public float TimePerStage = 0.5f;
    public float WaterConsumptionPerDay = 0.2f;
}

public class CropManager : MonoBehaviour
{
    public static CropManager Instance { get; private set; }
    
    [SerializeField] private List<CropData> _cropDatabase = new List<CropData>();
    private Dictionary<string, CropData> _cropLookup;
    
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
        
        _cropLookup = new Dictionary<string, CropData>();
        InitializeCropLookup();
    }
    
    private void InitializeCropLookup()
    {
        _cropLookup.Clear();
        foreach (var cropData in _cropDatabase)
        {
            if (!string.IsNullOrEmpty(cropData.CropId))
            {
                _cropLookup[cropData.CropId] = cropData;
            }
            else
            {
                Debug.LogError($"Found crop data with empty CropId in CropManager database");
            }
        }
    }
    
    public GameObject GetCropPrefab(string seedId)
    {
        if (string.IsNullOrEmpty(seedId)) return null;
        
        // Convert seed ID to crop ID (e.g., "carrot_seed" -> "carrot")
        string cropId = seedId.Replace("_seed", "");
        
        if (_cropLookup.TryGetValue(cropId, out CropData data))
        {
            return data.CropPrefab;
        }
        
        Debug.LogWarning($"No crop found for seed ID: {seedId}");
        return null;
    }
    
    public InventoryItem CreateSeedItem(string cropId)
    {
        if (string.IsNullOrEmpty(cropId)) return null;
        
        if (_cropLookup.TryGetValue(cropId, out CropData data))
        {
            return new InventoryItem(
                $"{cropId}_seed",
                $"{data.CropName} Seed",
                InventoryItem.ItemType.Seed,
                data.SeedIcon,
                data.SeedValue
            );
        }
        
        Debug.LogWarning($"No crop data found for ID: {cropId}");
        return null;
    }
    
    public Sprite GetCropIcon(string cropId)
    {
        if (string.IsNullOrEmpty(cropId)) return null;
        
        if (_cropLookup.TryGetValue(cropId, out CropData data))
        {
            return data.CropIcon;
        }
        
        Debug.LogWarning($"No crop icon found for ID: {cropId}");
        return null;
    }
    
    public CropData GetCropData(string cropId)
    {
        if (string.IsNullOrEmpty(cropId)) return null;
        
        if (_cropLookup.TryGetValue(cropId, out CropData data))
        {
            return data;
        }
        
        Debug.LogWarning($"No crop data found for ID: {cropId}");
        return null;
    }
} 