using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Market : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _marketPanel;
    public GameObject MarketPanel => _marketPanel;
    
    private MarketState _state;
    private MarketTabManager _tabManager;
    private MarketInventoryManager _inventoryManager;
    
    public static event System.Action OnHandlingEsc;
    
    private void Awake()
    {
        _state = GetComponent<MarketState>();
        _tabManager = GetComponent<MarketTabManager>();
        _inventoryManager = GetComponent<MarketInventoryManager>();
        
        gameObject.layer = LayerMask.NameToLayer("Market");
    }
    
    private void Start()
    {
        if (_marketPanel != null)
        {
            _marketPanel.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (_marketPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            OnHandlingEsc?.Invoke();
            CloseMarket();
            return;
        }
    }
    
    public void Interact(PlayerInteraction player)
    {
        OpenMarket();
    }
    
    public void OpenMarket()
    {
        _marketPanel.SetActive(true);
        _state.Open();
        _tabManager.ShowBuyTab();
    }
    
    public void CloseMarket()
    {
        _marketPanel.SetActive(false);
        _state.Close();
        GameManager.Instance.ResumeGame();
        TooltipUI.Instance.HideTooltip();
    }
} 