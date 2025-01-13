using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    [Header("Tool Settings")]
    public string ToolId;
    public string ToolName;
    public GameObject ToolModel;
    public float UseRange = 2f;
    public float UseDelay = 0.2f;
    public Sprite ToolIcon;
    
    protected bool _canUse = true;
    protected float _useTimer;
    
    public virtual void OnEquip()
    {
        gameObject.SetActive(true);
        if (ToolModel != null)
        {
            ToolModel.SetActive(true);
        }
    }
    
    public virtual void OnUnequip()
    {
        if (ToolModel != null)
        {
            ToolModel.SetActive(false);
        }
        gameObject.SetActive(false);
    }
    
    public abstract void UseTool(Vector3 usePosition);
    
    protected virtual void Update()
    {
        if (!_canUse)
        {
            _useTimer += Time.deltaTime;
            if (_useTimer >= UseDelay)
            {
                _canUse = true;
                _useTimer = 0;
            }
        }
    }
} 