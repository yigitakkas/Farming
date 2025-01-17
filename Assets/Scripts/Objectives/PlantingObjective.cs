using UnityEngine;

public class PlantingObjective : Objective
{
    private int _targetCount;
    private int _currentCount;
    
    public override string Description => $"{base.Description} ({_currentCount}/{_targetCount})";
    
    public PlantingObjective(string id, string title, string description, int targetCount, float reward)
    {
        Id = id;
        Title = title;
        Description = description;
        Reward = reward;
        _targetCount = targetCount;
        _currentCount = 0;
        
        // Subscribe to planting events
        FarmingActions.OnPlantingComplete += OnPlantingComplete;
    }
    
    private void OnPlantingComplete()
    {
        _currentCount++;
        InvokeProgressUpdate();
    }
    
    public override bool CheckCompletion()
    {
        return _currentCount >= _targetCount;
    }
} 