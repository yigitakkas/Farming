using UnityEngine;
using System;

public class CropObjective : Objective
{
    private int _targetCount;
    private int _currentCount;
    
    public override string Description => $"{base.Description} ({_currentCount}/{_targetCount})";
    
    public CropObjective(string id, string title, string description, int targetCount, float reward)
    {
        Id = id;
        Title = title;
        Description = description;
        Reward = reward;
        _targetCount = targetCount;
        
        // Subscribe to crop harvests
        InventorySystem.Instance.OnItemAdded += OnItemAdded;
    }
    
    private void OnItemAdded(InventoryItem item)
    {
        if (item.Type == InventoryItem.ItemType.Crop)
        {
            _currentCount += item.Quantity;
            InvokeProgressUpdate();
        }
    }
    
    public override bool CheckCompletion()
    {
        return _currentCount >= _targetCount;
    }
} 