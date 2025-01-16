using UnityEngine;
using UnityEngine.UI;

public class MarketTabManager : MonoBehaviour
{
    [SerializeField] private GameObject _buyTabContent;
    [SerializeField] private GameObject _sellTabContent;
    [SerializeField] private Button _buyTabButton;
    [SerializeField] private Button _sellTabButton;
    [SerializeField] private Color _selectedTabColor = Color.white;
    [SerializeField] private Color _unselectedTabColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    
    private MarketInventoryManager _inventoryManager;
    
    private void Awake()
    {
        _inventoryManager = GetComponent<MarketInventoryManager>();
    }
    
    public void ShowBuyTab()
    {
        _buyTabContent.SetActive(true);
        _sellTabContent.SetActive(false);
        UpdateTabVisuals(true);
        _inventoryManager.PopulateBuyTab();
    }
    
    public void ShowSellTab()
    {
        _buyTabContent.SetActive(false);
        _sellTabContent.SetActive(true);
        UpdateTabVisuals(false);
        _inventoryManager.PopulateSellTab();
    }
    
    private void UpdateTabVisuals(bool isBuyTab)
    {
        _buyTabButton.image.color = isBuyTab ? _selectedTabColor : _unselectedTabColor;
        _sellTabButton.image.color = isBuyTab ? _unselectedTabColor : _selectedTabColor;
    }
} 