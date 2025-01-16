using UnityEngine;

public class PlantingPreview : ToolPreview
{
    private LayerMask _plantingLayer;
    private float _plantingHeight;
    public bool CanPlantHere { get; private set; }
    
    public void Initialize(Transform groundCheck, float useRange, GameObject previewPrefab, 
        LayerMask plantingLayer, float plantingHeight)
    {
        base.Initialize(groundCheck, useRange, previewPrefab);
        _plantingLayer = plantingLayer;
        _plantingHeight = plantingHeight;
    }
    
    public override void UpdatePreview()
    {
        if (_previewObject == null || _groundCheck == null) return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
        
        foreach (var hit in hits)
        {
            if (((1 << hit.collider.gameObject.layer) & _plantingLayer) != 0)
            {
                Vector3 feetPosition = _groundCheck.position;
                float distanceXZ = Vector3.Distance(
                    new Vector3(feetPosition.x, 0, feetPosition.z),
                    new Vector3(hit.point.x, 0, hit.point.z)
                );
                
                if (distanceXZ <= _useRange)
                {
                    CanPlantHere = true;
                    _previewObject.SetActive(true);
                    _previewObject.transform.position = hit.point + Vector3.up * _plantingHeight;
                    return;
                }
            }
        }
        
        CanPlantHere = false;
        Hide();
    }
    
    public override void Hide()
    {
        if (_previewObject != null)
        {
            _previewObject.SetActive(false);
        }
    }
} 