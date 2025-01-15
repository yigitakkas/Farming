using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float InteractionRange = 2f;
    public LayerMask InteractionLayer;
    
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
    private void Start()
    {
        _mainCamera = Camera.main;
        ToolHolder.localPosition += ToolOffset;
        ToolHolder.localRotation *= Quaternion.Euler(ToolRotation);
        _playerController = GetComponent<PlayerController>();
        _playerVisual = _playerController.ModelTransform.gameObject;
        _playerPosition = _playerVisual.transform.position;
        InstantiateTools();

    }
    
    private void Update()
    {
        HandleToolSelection();
        HandleToolUse();
        HandleInteraction();
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
    
    // Handles direct interactions with objects when no tool is equipped
    private void HandleInteraction()
    {
        if (CurrentTool == null && Input.GetMouseButtonDown(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, _raycastDistance);
            
            foreach (var hit in hits)
            {
                float distance = hit.distance;
                if (((1 << hit.collider.gameObject.layer) & InteractionLayer) != 0)
                {
                    _playerPosition = _playerVisual.transform.position;
                    float distanceXZ = Vector3.Distance(
                        new Vector3(_playerPosition.x, 0, _playerPosition.z),
                        new Vector3(hit.point.x, 0, hit.point.z)
                    );
                    
                    if (distanceXZ <= InteractionRange)
                    {
                        IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                        if (interactable != null)
                        {
                            interactable.Interact(this);
                            return;
                        }
                    }
                }
            }
        }
    }
} 