using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingMessage : MonoBehaviour
{
    [SerializeField] private TextMeshPro _textMesh;
    [SerializeField] private float _displayTime = 1f;
    [SerializeField] private float _floatSpeed = 1f;
    [SerializeField] private float _fadeSpeed = 1f;
    [SerializeField] private bool _isErrorMessage = false;  // Flag to identify error messages
    
    private void Start()
    {
        if (_textMesh == null)
        {
            _textMesh = GetComponentInChildren<TextMeshPro>();
        }
        
        if (_textMesh == null)
        {
            Debug.LogError("No TextMeshPro component found!");
            return;
        }
        
        // Play error sound if this is an error message
        if (_isErrorMessage)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.ErrorSound);
        }
        
        StartCoroutine(FloatAndFade());
    }
    
    public void SetMessage(string message, bool isError = false)
    {
        if (_textMesh == null)
        {
            Debug.LogError("TextMeshPro component is null!");
            return;
        }
        _textMesh.text = message;
        _isErrorMessage = isError;
    }
    
    private IEnumerator FloatAndFade()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Color startColor = _textMesh.color;
        
        while (elapsed < _displayTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _displayTime;
            
            // Float upward
            transform.position = startPos + Vector3.up * (_floatSpeed * t);
            
            // Fade out
            _textMesh.color = new Color(startColor.r, startColor.g, startColor.b, 
                Mathf.Lerp(1f, 0f, t * _fadeSpeed));
            
            // Face camera
            transform.rotation = Camera.main.transform.rotation;
            
            yield return null;
        }
        
        Destroy(gameObject);
    }
} 