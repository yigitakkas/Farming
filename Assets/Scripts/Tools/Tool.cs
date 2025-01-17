using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    [Header("Tool Settings")]
    public string ToolId;
    public string ToolName;
    public GameObject ToolModel;
    public float UseRange = 2f;
    public float UseDelay = 0.2f;
    [SerializeField] protected Sprite _toolIcon;
    public Sprite ToolIcon => _toolIcon;
    
    [Header("Shop Settings")]
    public float BaseValue = 100f;  // Base price of the tool
    
    [Header("Tool Attributes")]
    [SerializeField] protected ToolAttributes _attributes = new ToolAttributes();
    public ToolAttributes Attributes => _attributes;
    
    protected bool _canUse = true;
    protected float _useTimer;
    
    [TextArea(3, 5)]
    [SerializeField] private string _description;
    
    public string GetDescription()
    {
        string desc = _description;
            
        return desc;
    }
    
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