using UnityEngine;
using System.Collections.Generic;
using System;

public class ObjectiveManager : MonoBehaviour
{
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
        InitializeStartingObjectives();
    }
    
    private void Update()
    {
        CheckObjectives();
    }
    
    private void InitializeStartingObjectives()
    {
        // Add starting objectives
        AddObjective(new MoneyObjective("earn_100", "First Profits", "Earn $100", 100f, 20f));
        AddObjective(new CropObjective("harvest_5", "Beginning Farmer", "Harvest 5 crops", 5, 50f));
    }
    
    public void AddObjective(Objective objective)
    {
        CurrentObjectives.Add(objective);
        OnNewObjectiveAdded?.Invoke(objective);
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
                AddFollowUpObjective(objective);
            }
        }
    }
    
    private void AddFollowUpObjective(Objective completedObjective)
    {
        // Add progressively harder objectives based on completed ones
        switch (completedObjective.Id)
        {
            case "earn_100":
                AddObjective(new MoneyObjective("earn_500", "Growing Business", "Earn $500", 500f, 100f));
                break;
            case "harvest_5":
                AddObjective(new CropObjective("harvest_20", "Established Farmer", "Harvest 20 crops", 20, 150f));
                break;
        }
    }
} 