using UnityEngine;

public class IconGenerator : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera RenderCamera;
    public int Resolution = 512;
    public Color BackgroundColor = Color.clear;
    
    [Header("Model Settings")]
    public Vector3 ModelRotation = new Vector3(0, -45f, 0);
    public float CameraDistance = 2f;
    public float ModelScale = 1f;
    
    public Sprite GenerateIcon(GameObject prefab)
    {
        // Setup render texture
        RenderTexture rt = new RenderTexture(Resolution, Resolution, 24);
        RenderTexture.active = rt;
        RenderCamera.targetTexture = rt;
        
        // Clear background
        RenderCamera.backgroundColor = BackgroundColor;
        RenderCamera.clearFlags = CameraClearFlags.SolidColor;
        
        // Instantiate model
        GameObject temp = Instantiate(prefab);
        temp.transform.rotation = Quaternion.Euler(ModelRotation);
        temp.transform.localScale = Vector3.one * ModelScale;
        
        // Position camera and model
        RenderCamera.transform.position = temp.transform.position + Vector3.back * CameraDistance;
        RenderCamera.transform.LookAt(temp.transform.position);
        
        // Render to texture
        RenderCamera.Render();
        
        // Read pixels to texture2D
        Texture2D tex = new Texture2D(Resolution, Resolution, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, Resolution, Resolution), 0, 0);
        tex.Apply();
        
        // Clean up
        RenderCamera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);
        DestroyImmediate(temp);
        
        // Create sprite
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, Resolution, Resolution), new Vector2(0.5f, 0.5f));
        return sprite;
    }
} 