using UnityEngine;

public abstract class ToolPreview : MonoBehaviour
{
    protected Transform _groundCheck;
    protected float _useRange;
    protected GameObject _previewObject;
    
    public void Initialize(Transform groundCheck, float useRange, GameObject previewPrefab)
    {
        _groundCheck = groundCheck;
        _useRange = useRange;
        
        if (previewPrefab != null)
        {
            _previewObject = Instantiate(previewPrefab);
            _previewObject.SetActive(false);
            SetPreviewLayer();
        }
    }
    
    protected void SetPreviewLayer()
    {
        _previewObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        foreach (Transform child in _previewObject.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
    }
    
    public abstract void UpdatePreview();
    public abstract void Hide();
    
    protected void OnDestroy()
    {
        if (_previewObject != null)
        {
            Destroy(_previewObject);
        }
    }
    
    public Vector3 PreviewPosition => _previewObject?.transform.position ?? Vector3.zero;
} 