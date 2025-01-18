using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CropData
{
    [Header("Basic Info")]
    public string CropId;
    public string CropName;
    [TextArea(3, 5)]
    public string Description;
    
    [Header("Visuals")]
    public GameObject CropPrefab;
    public Sprite CropIcon;
    public Sprite SeedIcon;
    
    [Header("Economy")]
    [Tooltip("Value of the harvested crop")]
    public float BaseValue = 10f;
    [Tooltip("Cost to buy the seed")]
    public float SeedValue = 5f;
    [Range(0.1f, 1f)]
    [Tooltip("Percentage of value returned when selling (0.5 = 50%)")]
    public float SellMultiplier = 0.5f;
    
    [Header("Growth Settings")]
    [Range(1, 5)]
    public int GrowthStages = 3;
    
    [Tooltip("Time in game minutes for each growth stage (e.g., 60 = 1 game hour)")]
    public float TimePerStage = 60f;
    
    [Range(0.1f, 1f)]
    public float WaterConsumptionPerDay = 0.2f;

    public string GetDescription()
    {
        float totalHours = (TimePerStage * (GrowthStages - 1)) / 60f;
        return $"{Description}\nGrowth Time: {totalHours:F1} game hours\n" +
               $"Water Needs: {WaterConsumptionPerDay * 100:F0}% per day\n" +
               $"Sell Price: ${BaseValue:F2}";
    }

    public float GetTotalGrowthDays()
    {
        float totalMinutes = TimePerStage * (GrowthStages - 1);
        float totalHours = totalMinutes / 60f;
        return totalHours / 24f;
    }
}

public class CropManager : MonoBehaviour
{
    public static CropManager Instance { get; private set; }
    
    [Header("Crop Database")]
    [SerializeField] 
    [Tooltip("Configure all your crops here")]
    private List<CropData> _cropDatabase = new List<CropData>();
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
                data.SeedValue,
                data.SellMultiplier
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