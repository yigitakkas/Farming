using UnityEngine;
using UnityEngine.UI;

public class MarketTabManager : MonoBehaviour
{
    public static MarketTabManager Instance { get; private set; }
    
    [Header("Tab References")]
    [SerializeField] private GameObject _buyTabContent;
    [SerializeField] private GameObject _sellTabContent;
    [SerializeField] private Button _buyTabButton;
    [SerializeField] private Button _sellTabButton;
    
    [Header("Visual Settings")]
    [SerializeField] private Color _selectedTabColor = Color.white;
    [SerializeField] private Color _unselectedTabColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    
    [SerializeField] private MarketInventoryManager _marketInventory;
    
    private bool _isBuyTabActive = true;
    public bool IsBuyTabActive => _isBuyTabActive;
    
    private void Awake()
    {
        Instance = this;
        if (_marketInventory == null)
        {
            _marketInventory = GetComponent<MarketInventoryManager>();
        }
    }
    
    public void ShowBuyTab()
    {
        _isBuyTabActive = true;
        _buyTabContent.SetActive(true);
        _sellTabContent.SetActive(false);
        UpdateTabVisuals(true);
        _marketInventory.PopulateBuyTab();
    }
    
    public void ShowSellTab()
    {
        _isBuyTabActive = false;
        _buyTabContent.SetActive(false);
        _sellTabContent.SetActive(true);
        UpdateTabVisuals(false);
        _marketInventory.PopulateSellTab();
    }
    
    private void UpdateTabVisuals(bool isBuyTab)
    {
        _buyTabButton.image.color = isBuyTab ? _selectedTabColor : _unselectedTabColor;
        _sellTabButton.image.color = isBuyTab ? _unselectedTabColor : _selectedTabColor;
    }
} 