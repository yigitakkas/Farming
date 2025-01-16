using UnityEngine;
using TMPro;

public class InteractionPrompt : MonoBehaviour
{
    [SerializeField] private TextMeshPro _promptText;
    [SerializeField] private string _keyText = "[E]";
    
    private void Start()
    {
        if (_promptText == null)
            _promptText = GetComponentInChildren<TextMeshPro>();
            
        _promptText.text = _keyText;
        gameObject.SetActive(false);
    }
    
    private void Update()
    {
        // Always face camera
        if (Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
} 