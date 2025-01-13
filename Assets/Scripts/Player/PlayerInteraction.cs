using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float InteractionRange = 2f;
    public LayerMask InteractionLayer;
    
    [Header("Tool Settings")]
    public Transform ToolHolder;
    public GameObject[] ToolPrefabs;
    private Tool[] _instantiatedTools;
    public Tool CurrentTool;
    private int _currentToolIndex = -1;
    
    private Camera _mainCamera;
    private bool _isInteracting;
    private float _interactionTimer;
    
    private void Start()
    {
        _mainCamera = Camera.main;
        InstantiateTools();
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
    
    private void Update()
    {
        HandleToolSelection();
        HandleToolUse();
        HandleInteraction();
    }
    
    // Handles number key input for tool selection
    private void HandleToolSelection()
    {
        for (int i = 0; i < _instantiatedTools.Length && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                EquipTool(i);
            }
        }
    }
    
    // Processes tool usage when clicking
    private void HandleToolUse()
    {
        if (CurrentTool != null && Input.GetMouseButtonDown(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            
            if (CurrentTool is PlantingTool plantingTool)
            {
                RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
                
                foreach (var hit in hits)
                {
                    bool isOnFarmland = ((1 << hit.collider.gameObject.layer) & plantingTool.PlantingLayer.value) != 0;
                    
                    if (isOnFarmland)
                    {
                        CurrentTool.UseTool(hit.point);
                        return;
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
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, InteractionRange, InteractionLayer))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact(this);
                }
                
                Crop crop = hit.collider.GetComponent<Crop>();
                if (crop != null && crop.IsReadyToHarvest())
                {
                    StartHarvesting(crop);
                }
            }
        }
    }
    
    // Initiates the harvesting process for a crop
    private void StartHarvesting(Crop crop)
    {
        crop.Harvest();
    }
} 