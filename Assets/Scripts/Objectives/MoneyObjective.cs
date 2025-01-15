using UnityEngine;
using System;

public class MoneyObjective : Objective
{
    private float _targetMoney;
    
    public MoneyObjective(string id, string title, string description, float targetMoney, float reward)
    {
        Id = id;
        Title = title;
        Description = description;
        Reward = reward;
        _targetMoney = targetMoney;
    }
    
    public override bool CheckCompletion()
    {
        return GameManager.Instance.PlayerMoney >= _targetMoney;
    }
} 