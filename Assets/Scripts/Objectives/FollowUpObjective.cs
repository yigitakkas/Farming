using UnityEngine;

[System.Serializable]
public class FollowUpObjective
{
    public string TriggerObjectiveId;  // When this objective completes...
    [SerializeReference] public ObjectiveConfig NewObjective;  // ...this objective will be added
} 