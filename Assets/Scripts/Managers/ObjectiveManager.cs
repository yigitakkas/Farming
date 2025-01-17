using UnityEngine;
using System.Collections.Generic;
using System;

public class ObjectiveManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private ObjectiveDatabase _objectiveDatabase;
    
    public static ObjectiveManager Instance { get; private set; }
    
    public List<Objective> CurrentObjectives = new List<Objective>();
    public List<Objective> CompletedObjectives = new List<Objective>();
    
    public event Action<Objective> OnObjectiveCompleted;
    public event Action<Objective> OnNewObjectiveAdded;
    
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
        }
    }
    
    private void Start()
    {
        if (_objectiveDatabase == null)
        {
            Debug.LogError("Objective Database not assigned!");
            return;
        }

        // Add initial objectives
        foreach (var config in _objectiveDatabase.InitialMoneyObjectives)
        {
            AddObjective(CreateObjectiveFromConfig(config));
        }
        
        foreach (var config in _objectiveDatabase.InitialCropObjectives)
        {
            AddObjective(CreateObjectiveFromConfig(config));
        }

        foreach (var config in _objectiveDatabase.InitialPlantingObjectives)
        {
            AddObjective(CreateObjectiveFromConfig(config));
        }

        foreach (var config in _objectiveDatabase.InitialWateringObjectives)
        {
            AddObjective(CreateObjectiveFromConfig(config));
        }
    }
    
    private void Update()
    {
        CheckObjectives();
    }
    
    private void CheckObjectives()
    {
        for (int i = CurrentObjectives.Count - 1; i >= 0; i--)
        {
            Objective objective = CurrentObjectives[i];
            if (objective.CheckCompletion())
            {
                objective.Complete();
                CurrentObjectives.RemoveAt(i);
                CompletedObjectives.Add(objective);
                OnObjectiveCompleted?.Invoke(objective);
                
                // Add follow-up objective if appropriate
                AddFollowUpObjective(objective.Id);
            }
        }
    }
    
    private void AddFollowUpObjective(string completedObjectiveId)
    {
        var followUp = _objectiveDatabase.FollowUpObjectives.Find(f => f.TriggerObjectiveId == completedObjectiveId);
        if (followUp != null)
        {
            AddObjective(CreateObjectiveFromConfig(followUp.NewObjective));
        }
    }
    
    public void AddObjective(Objective objective)
    {
        CurrentObjectives.Add(objective);
        OnNewObjectiveAdded?.Invoke(objective);
    }
    
    private Objective CreateObjectiveFromConfig(ObjectiveConfig config)
    {
        switch (config)
        {
            case MoneyObjectiveConfig moneyConfig:
                return new MoneyObjective(
                    moneyConfig.Id,
                    moneyConfig.Title,
                    moneyConfig.Description ?? $"Earn ${moneyConfig.TargetMoney:F2}",
                    moneyConfig.TargetMoney,
                    moneyConfig.Reward
                );
                
            case CropObjectiveConfig cropConfig:
                return new CropObjective(
                    cropConfig.Id,
                    cropConfig.Title,
                    cropConfig.Description ?? $"Harvest {cropConfig.TargetCount} crops",
                    cropConfig.TargetCount,
                    cropConfig.Reward
                );
                
            case PlantingObjectiveConfig plantConfig:
                return new PlantingObjective(
                    plantConfig.Id,
                    plantConfig.Title,
                    plantConfig.Description ?? $"Plant {plantConfig.TargetCount} seeds",
                    plantConfig.TargetCount,
                    plantConfig.Reward
                );
                
            case WateringObjectiveConfig waterConfig:
                return new WateringObjective(
                    waterConfig.Id,
                    waterConfig.Title,
                    waterConfig.Description ?? $"Water plants {waterConfig.TargetCount} times",
                    waterConfig.TargetCount,
                    waterConfig.Reward
                );
                
            default:
                Debug.LogError($"Unknown objective config type: {config.GetType()}");
                return null;
        }
    }
} 