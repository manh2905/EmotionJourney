using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// Debug script để tìm lỗi UI không nhận click
/// Gắn vào GameObject bất kỳ trong scene
/// </summary>
public class UIClickDebugger : MonoBehaviour
{
    [Header("Target Button")]
    public Button targetButton;
    
    [Header("Debug Settings")]
    public bool autoFindButton = true;
    public bool logMousePosition = false;
    public Color highlightColor = Color.yellow;
    
    private EventSystem eventSystem;
    private GraphicRaycaster raycaster;
    private Canvas canvas;
    
    void Start()
    {
        Debug.Log("╔═══════════════════════════════════════╗");
        Debug.Log("║     UI CLICK DEBUGGER STARTED         ║");
        Debug.Log("╚═══════════════════════════════════════╝");
        
        if (autoFindButton && targetButton == null)
        {
            FindButton();
        }
        
        CheckUISystemComponents();
        
        if (targetButton != null)
        {
            CheckButtonComponents();
        }
        
        Debug.Log("═══════════════════════════════════════");
        Debug.Log("💡 TIP: Click chuột trong game để see raycast results");
    }
    
    void FindButton()
    {
        GameObject buttonObj = GameObject.Find("Confirm_btn");
        if (buttonObj == null) buttonObj = GameObject.Find("ConfirmButton");
        if (buttonObj == null) buttonObj = GameObject.Find("Confirm");
        
        if (buttonObj != null)
        {
            targetButton = buttonObj.GetComponent<Button>();
            if (targetButton != null)
            {
                Debug.Log($"✅ Auto-found button: {targetButton.name}");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Không tìm thấy button. Vui lòng gán manually.");
        }
    }
    
    void CheckUISystemComponents()
    {
        Debug.Log("\n--- CHECKING UI SYSTEM COMPONENTS ---");
        
        // Check EventSystem
        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("❌ CRITICAL: EventSystem KHÔNG TỒN TẠI!");
            Debug.LogError("   → FIX: GameObject → UI → Event System");
        }
        else
        {
            Debug.Log($"✅ EventSystem: {eventSystem.name}");
            Debug.Log($"   ├─ Enabled: {eventSystem.enabled}");
            Debug.Log($"   └─ Current Selected: {(eventSystem.currentSelectedGameObject != null ? eventSystem.currentSelectedGameObject.name : "None")}");
        }
        
        // Check Canvas & GraphicRaycaster
        if (targetButton != null)
        {
            canvas = targetButton.GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("❌ CRITICAL: Button không nằm trong Canvas!");
            }
            else
            {
                Debug.Log($"✅ Canvas: {canvas.name}");
                Debug.Log($"   ├─ Render Mode: {canvas.renderMode}");
                Debug.Log($"   ├─ Sorting Order: {canvas.sortingOrder}");
                
                raycaster = canvas.GetComponent<GraphicRaycaster>();
                if (raycaster == null)
                {
                    Debug.LogError("❌ CRITICAL: Canvas thiếu Graphic Raycaster!");
                    Debug.LogError("   → FIX: Add Component → Graphic Raycaster");
                }
                else
                {
                    Debug.Log($"✅ Graphic Raycaster: Present");
                    Debug.Log($"   └─ Ignore Reversed Graphics: {raycaster.ignoreReversedGraphics}");
                }
            }
        }
    }
    
    void CheckButtonComponents()
    {
        Debug.Log("\n--- CHECKING BUTTON COMPONENTS ---");
        
        Debug.Log($"Button: {targetButton.name}");
        Debug.Log($"├─ GameObject Active: {targetButton.gameObject.activeInHierarchy}");
        Debug.Log($"├─ Enabled: {targetButton.enabled}");
        Debug.Log($"├─ Interactable: {targetButton.interactable}");
        
        // Check RectTransform
        RectTransform rectTransform = targetButton.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Debug.Log($"├─ RectTransform:");
            Debug.Log($"│  ├─ Size: {rectTransform.rect.width} x {rectTransform.rect.height}");
            Debug.Log($"│  ├─ Position: {rectTransform.position}");
            Debug.Log($"│  └─ Scale: {rectTransform.localScale}");
        }
        
        // Check Image/Graphic components (for raycast target)
        Graphic[] graphics = targetButton.GetComponents<Graphic>();
        if (graphics.Length == 0)
        {
            Debug.LogWarning("⚠️ Button không có Image/Text component!");
        }
        else
        {
            Debug.Log($"├─ Graphic Components: {graphics.Length}");
            foreach (var graphic in graphics)
            {
                Debug.Log($"│  ├─ {graphic.GetType().Name}");
                Debug.Log($"│  │  ├─ Raycast Target: {graphic.raycastTarget}");
                Debug.Log($"│  │  └─ Color Alpha: {graphic.color.a}");
                
                if (!graphic.raycastTarget)
                {
                    Debug.LogError($"│  │  ❌ RAYCAST TARGET = FALSE! Button sẽ không nhận click!");
                }
            }
        }
        
        // Check for blocking elements
        Debug.Log($"└─ Checking for UI blockers...");
        CheckForBlockingElements();
    }
    
    void CheckForBlockingElements()
    {
        if (targetButton == null || canvas == null) return;
        
        // Get all canvases
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        
        bool hasBlockers = false;
        foreach (var otherCanvas in allCanvases)
        {
            if (otherCanvas.sortingOrder > canvas.sortingOrder)
            {
                Debug.LogWarning($"   ⚠️ Canvas '{otherCanvas.name}' (order: {otherCanvas.sortingOrder}) ở trên button canvas!");
                hasBlockers = true;
            }
        }
        
        if (!hasBlockers)
        {
            Debug.Log("   ✅ Không có canvas nào block button");
        }
    }
    
    void Update()
    {
        // Detect mouse click và log raycast results
        if (Input.GetMouseButtonDown(0))
        {
            LogRaycastAtMousePosition();
        }
        
        if (logMousePosition)
        {
            Debug.Log($"Mouse Pos: {Input.mousePosition}");
        }
    }
    
    void LogRaycastAtMousePosition()
    {
        if (eventSystem == null) return;
        
        Debug.Log($"\n🖱️ MOUSE CLICK tại: {Input.mousePosition}");
        
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Input.mousePosition;
        
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        
        if (results.Count == 0)
        {
            Debug.LogWarning("❌ KHÔNG CÓ UI ELEMENT nào bị hit!");
            Debug.LogWarning("   → EventSystem hoặc GraphicRaycaster có vấn đề");
        }
        else
        {
            Debug.Log($"✅ Hit {results.Count} UI elements:");
            for (int i = 0; i < results.Count; i++)
            {
                var result = results[i];
                string icon = (i == 0) ? "🎯" : "  ";
                Debug.Log($"{icon} [{i}] {result.gameObject.name}");
                Debug.Log($"      ├─ Layer: {LayerMask.LayerToName(result.gameObject.layer)}");
                Debug.Log($"      ├─ Sorting Order: {result.sortingOrder}");
                
                Button btn = result.gameObject.GetComponent<Button>();
                if (btn != null)
                {
                    Debug.Log($"      └─ Has Button: {(btn == targetButton ? "✅ TARGET BUTTON!" : "Other button")}");
                    
                    if (btn == targetButton)
                    {
                        if (!btn.interactable)
                        {
                            Debug.LogError("      ❌ BUTTON NOT INTERACTABLE!");
                        }
                    }
                }
            }
            
            // Check nếu button bị che
            if (targetButton != null)
            {
                bool buttonWasHit = false;
                int buttonIndex = -1;
                
                for (int i = 0; i < results.Count; i++)
                {
                    if (results[i].gameObject == targetButton.gameObject)
                    {
                        buttonWasHit = true;
                        buttonIndex = i;
                        break;
                    }
                }
                
                if (!buttonWasHit)
                {
                    Debug.LogWarning("⚠️ Target button KHÔNG ĐƯỢC HIT bởi raycast!");
                }
                else if (buttonIndex > 0)
                {
                    Debug.LogWarning($"⚠️ Target button bị che bởi {buttonIndex} elements khác!");
                    Debug.LogWarning($"   Blocker: {results[0].gameObject.name}");
                }
                else
                {
                    Debug.Log("✅ Target button là element đầu tiên được hit! (GOOD)");
                }
            }
        }
    }
    
    // Context menu tests
    [ContextMenu("🔍 Re-check All Systems")]
    public void RecheckSystems()
    {
        Start();
    }
    
    [ContextMenu("🎯 Highlight Target Button")]
    public void HighlightButton()
    {
        if (targetButton != null)
        {
            var img = targetButton.GetComponent<Image>();
            if (img != null)
            {
                img.color = highlightColor;
                Debug.Log("✅ Button highlighted in yellow");
            }
        }
    }
    
    [ContextMenu("📊 Force Raycast Test")]
    public void ForceRaycastTest()
    {
        Debug.Log("=== FORCE RAYCAST TEST ===");
        LogRaycastAtMousePosition();
    }
}
