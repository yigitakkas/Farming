using UnityEngine;

public class HarvestPreview : ToolPreview
{
    private LayerMask _cropLayer;
    public bool CanHarvestHere { get; private set; }
    public Crop TargetCrop { get; private set; }
    
    public void Initialize(Transform groundCheck, float useRange, GameObject previewPrefab, LayerMask cropLayer)
    {
        base.Initialize(groundCheck, useRange, previewPrefab);
        _cropLayer = cropLayer;
    }
    
    public override void UpdatePreview()
    {
        if (_previewObject == null || _groundCheck == null) return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, _cropLayer))
        {
            Vector3 feetPosition = _groundCheck.position;
            float distanceXZ = Vector3.Distance(
                new Vector3(feetPosition.x, 0, feetPosition.z),
                new Vector3(hit.point.x, 0, hit.point.z)
            );
            
            Crop crop = hit.collider.GetComponent<Crop>();
            if (crop != null && crop.IsReadyToHarvest() && distanceXZ <= _useRange)
            {
                CanHarvestHere = true;
                TargetCrop = crop;
                _previewObject.SetActive(true);
                _previewObject.transform.position = hit.point + Vector3.up * 0.1f;
                
                if (_previewObject.TryGetComponent<Renderer>(out var renderer))
                {
                    renderer.material.color = Color.yellow;
                }
                return;
            }
        }
        
        CanHarvestHere = false;
        TargetCrop = null;
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