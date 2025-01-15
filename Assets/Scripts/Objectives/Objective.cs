using UnityEngine;
using System;

[System.Serializable]
public class Objective
{
    public string Id;
    public string Title;
    public virtual string Description { get; set;}
    public float Reward;
    public bool IsCompleted { get; private set; }
    public event Action<Objective> OnCompleted;
    public event Action<Objective> OnProgressUpdated;
    
    public virtual bool CheckCompletion()
    {
        return false;
    }
    
    public void Complete()
    {
        if (!IsCompleted)
        {
            IsCompleted = true;
            GameManager.Instance.AddMoney(Reward);
            OnCompleted?.Invoke(this);
        }
    }
    
    protected void InvokeProgressUpdate()
    {
        OnProgressUpdated?.Invoke(this);
    }
} 