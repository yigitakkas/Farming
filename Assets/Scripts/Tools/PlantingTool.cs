using UnityEngine;

public class PlantingTool : Tool
{
    [Header("Planting Settings")]
    public GameObject CropPrefab;
    public LayerMask PlantingLayer;
    public float PlantingHeight = 0f;
    
    [Header("Visual Feedback")]
    public GameObject PlantingPreview;
    private GameObject _currentPreview;
    private bool _canPlantHere;
    private Transform _playerTransform;
    private Transform _groundCheck;
    
    private void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        _playerTransform = player.transform;
        
        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            _groundCheck = playerController.GroundCheck;
        }
        
        if (PlantingPreview != null)
        {
            _currentPreview = Instantiate(PlantingPreview);
            _currentPreview.SetActive(false);
        }
    }
    
    private void Update()
    {
        base.Update();
        UpdatePlantingPreview();
    }
    
    // Updates the preview marker position and visibility
    private void UpdatePlantingPreview()
    {
        if (_currentPreview == null || _groundCheck == null) return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
        
        foreach (var hit in hits)
        {
            if (((1 << hit.collider.gameObject.layer) & PlantingLayer) != 0)
            {
                Vector3 feetPosition = _groundCheck.position;
                float distanceXZ = Vector3.Distance(
                    new Vector3(feetPosition.x, 0, feetPosition.z),
                    new Vector3(hit.point.x, 0, hit.point.z)
                );
                
                if (distanceXZ <= UseRange)
                {
                    _canPlantHere = true;
                    _currentPreview.SetActive(true);
                    _currentPreview.transform.position = hit.point + Vector3.up * PlantingHeight;
                    
                    if (_currentPreview.TryGetComponent<Renderer>(out var renderer))
                    {
                        renderer.material.color = _canUse ? Color.green : Color.red;
                    }
                    return;
                }
            }
        }
        
        _canPlantHere = false;
        _currentPreview.SetActive(false);
    }
    
    // Plants a crop at the preview location
    public override void UseTool(Vector3 usePosition)
    {
        if (!_canUse || !_canPlantHere || _groundCheck == null) return;
        
        if (_currentPreview != null && _currentPreview.activeSelf)
        {
            Vector3 plantPosition = _currentPreview.transform.position;
            GameObject newCrop = Instantiate(CropPrefab, plantPosition, Quaternion.identity);
            
            _canUse = false;
            _useTimer = 0;
        }
    }
    
    // Activates the preview when tool is equipped
    public override void OnEquip()
    {
        base.OnEquip();
        if (_currentPreview != null)
        {
            _currentPreview.SetActive(true);
        }
    }
    
    // Deactivates the preview when tool is unequipped
    public override void OnUnequip()
    {
        base.OnUnequip();
        if (_currentPreview != null)
        {
            _currentPreview.SetActive(false);
        }
    }
    
    // Cleans up preview object when tool is destroyed
    private void OnDestroy()
    {
        if (_currentPreview != null)
        {
            Destroy(_currentPreview);
        }
    }
} 