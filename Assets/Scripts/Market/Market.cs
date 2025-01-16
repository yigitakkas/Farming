using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Market : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _marketPanel;
    
    private MarketState _state;
    private MarketTabManager _tabManager;
    private MarketInventoryManager _inventoryManager;
    
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
        if (Input.GetKeyDown(KeyCode.Escape) && _marketPanel.activeSelf)
        {
            CloseMarket();
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
    }
} 