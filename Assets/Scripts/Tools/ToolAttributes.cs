using UnityEngine;

[System.Serializable]
public class ToolAttributes
{
    [Header("Growth Modifiers")]
    [Tooltip("Lower value means faster growth")]
    public float GrowthTimeMultiplier = 1f;
    
    [Tooltip("Lower value means less water consumption")]
    public float WaterConsumptionMultiplier = 1f;
    
    [Header("Harvest Modifiers")]
    [Tooltip("Higher value means more items harvested")]
    public float HarvestQuantityMultiplier = 1f;
} 