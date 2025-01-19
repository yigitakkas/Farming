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
    public int CurrentToolIndex { get => _currentToolIndex; set => _currentToolIndex = value; }
    
    private Camera _mainCamera;
    private bool _isInteracting;
    private float _interactionTimer;
    
    private Vector3 _playerPosition;
    private GameObject _playerVisual;
    private PlayerController _playerController;
    private float _raycastDistance = 35f;
    
    [Header("UI Feedback")]
    [SerializeField] private GameObject _errorMessagePrefab;
    
    [Header("Interaction")]
    [SerializeField] private GameObject _marketPromptPrefab;
    private GameObject _currentPrompt;
    private IInteractable _currentInteractable;
    
    private bool CanInteract => !InventorySystem.Instance.IsInventoryOpen && 
                               !MarketState.Instance.IsOpen;
    
    private void Start()
    {
        _mainCamera = Camera.main;
        ToolHolder.localPosition += ToolOffset;
        ToolHolder.localRotation *= Quaternion.Euler(ToolRotation);
        _playerController = GetComponent<PlayerController>();
        _playerVisual = _playerController.ModelTransform.gameObject;
        _playerPosition = _playerVisual.transform.position;
        
        // Initialize tools from inventory instead of ToolPrefabs
        InstantiateToolsFromInventory();

        if (_marketPromptPrefab != null)
        {
            _currentPrompt = Instantiate(_marketPromptPrefab);
            _currentPrompt.SetActive(false);
        }

        // Subscribe to inventory changes
        InventorySystem.Instance.OnInventoryChanged += HandleInventoryChanged;
    }
    
    private void Update()
    {
        if (!CanInteract) return;
        
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
        // Don't allow tool use during animations
        if (_playerController.IsPlayingFarmingAnimation)
            return;

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
                // Right click for harvesting/watering
                else if (Input.GetMouseButtonDown(1))
                {
                    RaycastHit[] hitsRightClick = Physics.RaycastAll(ray, _raycastDistance);
                    foreach (var hit in hitsRightClick)
                    {
                        bool isOnCropLayer = ((1 << hit.collider.gameObject.layer) & farmingTool.CropLayer.value) != 0;
                        if (isOnCropLayer)
                        {
                            CurrentTool.UseTool(hit.point);
                            return;
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
                    hasPlantingTool = farmingTool.ToolType == FarmingTool.FarmingToolType.Planting || 
                                     farmingTool.ToolType == FarmingTool.FarmingToolType.Cultivator;
                }
                
                if (!hasPlantingTool)
                {
                    ShowFloatingError("Need planting tool!");
                    return;
                }
                
                // Check if we have a seed selected
                if (InventorySystem.Instance.GetSelectedSeed() == null)
                {
                    ShowFloatingError("Select a seed first!");
                    return;
                }
            }
        }
    }
    
    private void HandleProximityInteraction()
    {
        // Don't allow interactions during animations
        if (_playerController.IsPlayingFarmingAnimation)
            return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, InteractionRange, InteractionLayer);
        bool foundInteractable = false;
        
        foreach (var hitCollider in hitColliders)
        {
            IInteractable interactable = hitCollider.GetComponent<IInteractable>();
            if (interactable != null && !(interactable is Home))
            {
                foundInteractable = true;
                _currentInteractable = interactable;
                
                // Show appropriate prompt based on interactable type
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
    
    private void ShowFloatingError(string message)
    {
        if (_errorMessagePrefab != null)
        {
            GameObject messageObj = Instantiate(_errorMessagePrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
            FloatingMessage floatingMessage = messageObj.GetComponent<FloatingMessage>();
            if (floatingMessage != null)
            {
                floatingMessage.SetMessage(message, true);
            }
        }
    }
    
    private void OnDestroy()
    {
        if (_currentPrompt != null)
        {
            Destroy(_currentPrompt);
        }
        
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnInventoryChanged -= HandleInventoryChanged;
        }
    }
    
    private void HandleInventoryChanged()
    {
        var tools = InventorySystem.Instance.Items.FindAll(item => 
            item.Type == InventoryItem.ItemType.Tool);
        
        // Only reinstantiate if the number of tools changed
        if (_instantiatedTools == null || _instantiatedTools.Length != tools.Count)
        {
            // Destroy all current tool instances
            if (_instantiatedTools != null)
            {
                foreach (var tool in _instantiatedTools)
                {
                    if (tool != null)
                    {
                        Destroy(tool.gameObject);
                    }
                }
            }
            
            // Reinstantiate tools from current inventory
            InstantiateToolsFromInventory();
        }
        
        // Reset current tool only if it was removed
        if (CurrentTool != null)
        {
            bool toolStillExists = tools.Exists(item => item.ItemId == CurrentTool.ToolId);
            
            if (!toolStillExists)
            {
                CurrentTool = null;
                _currentToolIndex = -1;
            }
        }
    }
    
    private void InstantiateToolsFromInventory()
    {
        var tools = InventorySystem.Instance.Items.FindAll(item => 
            item.Type == InventoryItem.ItemType.Tool);
        
        _instantiatedTools = new Tool[tools.Count];
        
        for (int i = 0; i < tools.Count; i++)
        {
            GameObject prefab = InventorySystem.Instance.GetToolPrefab(tools[i].ItemId);
            if (prefab != null)
            {
                GameObject toolObj = Instantiate(prefab, ToolHolder.position, ToolHolder.rotation, ToolHolder);
                _instantiatedTools[i] = toolObj.GetComponent<Tool>();
                toolObj.SetActive(false);
            }
        }
    }
} 