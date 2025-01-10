using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float InteractionRange = 2f;
    public LayerMask InteractionLayer;
    
    [Header("Tool Settings")]
    public Transform ToolHolder;
    public float HarvestDuration = 1f;
    
    private Camera _mainCamera;
    private bool _isInteracting;
    private float _interactionTimer;
    
    private void Start()
    {
        _mainCamera = Camera.main;
    }
    
    private void Update()
    {
        HandleInteraction();
    }
    
    private void HandleInteraction()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, InteractionRange, InteractionLayer))
            {
                // Check for different interactable objects
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact(this);
                }
                
                // Check for crop
                Crop crop = hit.collider.GetComponent<Crop>();
                if (crop != null && crop.IsReadyToHarvest())
                {
                    StartHarvesting(crop);
                }
            }
        }
    }
    
    private void StartHarvesting(Crop crop)
    {
        // Animation and harvesting logic here
        crop.Harvest();
    }
} 