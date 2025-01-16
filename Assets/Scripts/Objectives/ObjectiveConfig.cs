using UnityEngine;

[System.Serializable]
public abstract class ObjectiveConfig
{
    public string Id;
    public string Title;
    [TextArea(2, 4)]
    public string Description;
    public float Reward;
}

[System.Serializable]
public class MoneyObjectiveConfig : ObjectiveConfig
{
    public float TargetMoney;
}

[System.Serializable]
public class CropObjectiveConfig : ObjectiveConfig
{
    public int TargetCount;
}