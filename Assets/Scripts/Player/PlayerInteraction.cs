using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float InteractionRange = 2f;
    public LayerMask InteractionLayer;  // For general interactions like market
    public LayerMask FarmlandLayer;     // Specifically for farmland
    
    [Header("Tool Settings")]
    public Transform ToolHolder;
    public GameObject[] ToolPrefabs;
    [Header("Tool Position")]
    public Vector3 ToolOffset = Vector3.zero;
    public Vector3 ToolRotation = Vector3.zero;
    private Tool[] _instantiatedTools;
    public Tool CurrentTool;
    private int _currentToolIndex = -1;
    
    private Camera _mainCamera;
    private bool _isInteracting;
    private float _interactionTimer;
    
    public int CurrentToolIndex => _currentToolIndex;

    private Vector3 _playerPosition;
    private GameObject _playerVisual;
    private PlayerController _playerController;
    private float _raycastDistance = 35f;
    
    [Header("UI Feedback")]
    [SerializeField] private GameObject _errorMessagePrefab;
    
    [Header("Interaction")]
    [SerializeField] private GameObject _interactionPromptPrefab;
    private GameObject _currentPrompt;
    private IInteractable _currentInteractable;
    
    private void Start()
    {
        _mainCamera = Camera.main;
        ToolHolder.localPosition += ToolOffset;
        ToolHolder.localRotation *= Quaternion.Euler(ToolRotation);
        _playerController = GetComponent<PlayerController>();
        _playerVisual = _playerController.ModelTransform.gameObject;
        _playerPosition = _playerVisual.transform.position;
        InstantiateTools();

        if (_interactionPromptPrefab != null)
        {
            _currentPrompt = Instantiate(_interactionPromptPrefab);
            _currentPrompt.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (GameManager.Instance.IsGamePaused) return;
        
        HandleToolSelection();
        
        // Handle farming interactions
        if (Input.GetMouseButtonDown(0))
        {
            HandleFarmingInteraction();
        }
        
        // Handle tool usage
        HandleToolUse();
        
        // Handle proximity-based interactions
        HandleProximityInteraction();
    }
    
    // Creates instances of all tool prefabs
    private void InstantiateTools()
    {
        _instantiatedTools = new Tool[ToolPrefabs.Length];
        for (int i = 0; i < ToolPrefabs.Length; i++)
        {
            GameObject toolObj = Instantiate(ToolPrefabs[i], ToolHolder.position, ToolHolder.rotation, ToolHolder);
            _instantiatedTools[i] = toolObj.GetComponent<Tool>();
            toolObj.SetActive(false);
        }
    }
    
    // Handles number key input for tool selection
    private void HandleToolSelection()
    {
        // Unequip with same number key or when selecting an empty slot
        if ((CurrentTool != null && Input.GetKeyDown(KeyCode.Alpha1 + _currentToolIndex)) ||
            (Input.GetKeyDown(KeyCode.Alpha1 + _currentToolIndex) && _currentToolIndex >= _instantiatedTools.Length))
        {
            EquipTool(-1);
            return;
        }

        for (int i = 0; i < GameConstants.MAX_HOTBAR_SLOTS; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                // If trying to select a slot beyond our tools, unequip
                if (i >= _instantiatedTools.Length)
                {
                    EquipTool(-1);
                }
                else
                {
                    EquipTool(i);
                }
            }
        }
    }
    
    // Processes tool usage when clicking
    private void HandleToolUse()
    {
        if (CurrentTool != null)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            
            if (CurrentTool is FarmingTool farmingTool)
            {
                // Left click for planting
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit[] hits = Physics.RaycastAll(ray, _raycastDistance);
                    foreach (var hit in hits)
                    {
                        bool isOnFarmland = ((1 << hit.collider.gameObject.layer) & farmingTool.PlantingLayer.value) != 0;
                        if (isOnFarmland)
                        {
                            CurrentTool.UseTool(hit.point);
                            return;
                        }
                    }
                }
                // Right click for harvesting
                else if (Input.GetMouseButtonDown(1))
                {
                    RaycastHit[] hitsRightClick = Physics.RaycastAll(ray, _raycastDistance);
                    foreach (var hit in hitsRightClick)
                    {
                        bool isOnCropLayer = ((1 << hit.collider.gameObject.layer) & farmingTool.CropLayer.value) != 0;
                        if (isOnCropLayer)
                        {
                            _playerPosition = _playerVisual.transform.position;
                            float distanceXZ = Vector3.Distance(
                                new Vector3(_playerPosition.x, 0, _playerPosition.z),
                                new Vector3(hit.point.x, 0, hit.point.z)
                            );
                            
                            if (distanceXZ <= farmingTool.UseRange)
                            {
                                CurrentTool.UseTool(hit.point);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
    
    // Switches between tools
    private void EquipTool(int index)
    {
        if (index == _currentToolIndex) return;
        
        if (CurrentTool != null)
        {
            CurrentTool.OnUnequip();
        }
        
        _currentToolIndex = index;
        if (index >= 0 && index < _instantiatedTools.Length)
        {
            CurrentTool = _instantiatedTools[index];
            CurrentTool.gameObject.SetActive(true);
            CurrentTool.OnEquip();
        }
        else
        {
            CurrentTool = null;
            _currentToolIndex = -1;
        }
    }
    
    private void HandleFarmingInteraction()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            int hitLayer = hit.collider.gameObject.layer;
            bool isOnFarmland = ((1 << hitLayer) & FarmlandLayer.value) != 0;
            
            if (isOnFarmland)
            {
                // Check if we have a planting-capable tool equipped
                bool hasPlantingTool = false;
                if (CurrentTool is FarmingTool farmingTool)
                {
                    hasPlantingTool = farmingTool.ToolType == FarmingTool.FarmingToolType.Spade || 
                                     farmingTool.ToolType == FarmingTool.FarmingToolType.Hoe;
                }
                
                if (!hasPlantingTool)
                {
                    ShowErrorMessage("Need planting tool!", hit.point);
                    return;
                }
                
                // Check if we have a seed selected
                if (InventorySystem.Instance.GetSelectedSeed() == null)
                {
                    ShowErrorMessage("Select a seed first!", hit.point);
                    return;
                }
            }
        }
    }
    
    private void HandleProximityInteraction()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, InteractionRange, InteractionLayer);
        bool foundInteractable = false;
        
        foreach (var hitCollider in hitColliders)
        {
            IInteractable interactable = hitCollider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                foundInteractable = true;
                _currentInteractable = interactable;
                
                // Show prompt at the interactable object's position
                if (_currentPrompt != null)
                {
                    _currentPrompt.SetActive(true);
                    _currentPrompt.transform.position = hitCollider.transform.position + Vector3.up * 2f;
                }
                
                // Handle interaction when E is pressed
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact(this);
                }
                break;
            }
        }
        
        if (!foundInteractable)
        {
            if (_currentPrompt != null)
            {
                _currentPrompt.SetActive(false);
            }
            _currentInteractable = null;
        }
    }
    
    private void ShowErrorMessage(string message, Vector3 position)
    {
        if (_errorMessagePrefab == null)
        {
            Debug.LogError("Error message prefab is not assigned!");
            return;
        }
        
        GameObject messageObj = Instantiate(_errorMessagePrefab, 
            position + Vector3.up * 1.5f,
            Quaternion.identity);
        
        FloatingMessage floatingMessage = messageObj.GetComponent<FloatingMessage>();
        if (floatingMessage != null)
        {
            floatingMessage.SetMessage(message);
        }
        else
        {
            Debug.LogError("FloatingMessage component not found on prefab!");
        }
    }
    
    private void OnDestroy()
    {
        if (_currentPrompt != null)
        {
            Destroy(_currentPrompt);
        }
    }
} 