using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ObjectiveDatabase", menuName = "Game/Objective Database")]
public class ObjectiveDatabase : ScriptableObject
{
    [Header("Initial Objectives")]
    public List<MoneyObjectiveConfig> InitialMoneyObjectives = new List<MoneyObjectiveConfig>();
    public List<CropObjectiveConfig> InitialCropObjectives = new List<CropObjectiveConfig>();
    public List<PlantingObjectiveConfig> InitialPlantingObjectives = new List<PlantingObjectiveConfig>();
    
    [Header("Follow-up Objectives")]
    public List<FollowUpObjective> FollowUpObjectives = new List<FollowUpObjective>();
} 