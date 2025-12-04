using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Debug script để kiểm tra tất cả thuộc tính của VictoryPanel
/// Attach vào VictoryPanel để debug
/// </summary>
public class VictoryPanelDebugger : MonoBehaviour
{
    void Update()
    {
        // Press V key để debug
        if (Input.GetKeyDown(KeyCode.V))
        {
            DebugPanel();
        }
    }

    public void DebugPanel()
    {
        Debug.Log("========== VICTORY PANEL DEBUG ==========");
        
        // 1. GameObject status
        Debug.Log($"GameObject Active: {gameObject.activeInHierarchy}");
        Debug.Log($"GameObject Name: {gameObject.name}");
        
        // 2. RectTransform
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Debug.Log($"RectTransform - Local Scale: {rectTransform.localScale}");
            Debug.Log($"RectTransform - Size Delta: {rectTransform.sizeDelta}");
            Debug.Log($"RectTransform - Anchored Position: {rectTransform.anchoredPosition}");
        }
        
        // 3. CanvasGroup
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            Debug.Log($"CanvasGroup - Alpha: {canvasGroup.alpha}");
            Debug.Log($"CanvasGroup - Interactable: {canvasGroup.interactable}");
            Debug.Log($"CanvasGroup - BlocksRaycasts: {canvasGroup.blocksRaycasts}");
        }
        else
        {
            Debug.LogWarning("No CanvasGroup found!");
        }
        
        // 4. Image Component
        Image image = GetComponent<Image>();
        if (image != null)
        {
            Debug.Log($"Image - Color: {image.color}");
            Debug.Log($"Image - Alpha: {image.color.a}");
            Debug.Log($"Image - Enabled: {image.enabled}");
            Debug.Log($"Image - Raycast Target: {image.raycastTarget}");
        }
        else
        {
            Debug.LogWarning("No Image component found!");
        }
        
        // 5. Canvas Renderer
        CanvasRenderer canvasRenderer = GetComponent<CanvasRenderer>();
        if (canvasRenderer != null)
        {
            Debug.Log($"CanvasRenderer - Cull Transparent Mesh: {canvasRenderer.cullTransparentMesh}");
        }
        
        // 6. Children status
        Debug.Log($"Children count: {transform.childCount}");
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Debug.Log($"  Child {i}: {child.name} - Active: {child.gameObject.activeInHierarchy}");
        }
        
        Debug.Log("========================================");
    }
}
