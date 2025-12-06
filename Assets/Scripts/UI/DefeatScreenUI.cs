using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Quản lý màn hình thua với options retry hoặc quay về map
/// Attach script này vào DefeatPanel GameObject
/// </summary>
public class DefeatScreenUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI defeatMessageText;
    public Button retryButton;
    
    [Header("Optional Animation")]
    public Animator panelAnimator;
    
    private CanvasGroup canvasGroup;
    private bool hasTransitioned = false;

    void Awake()
    {
        // Get or add CanvasGroup component to THIS GameObject
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            Debug.Log("✅ Added CanvasGroup to DefeatPanel");
        }
        
        // Hide panel initially using CanvasGroup
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        Debug.Log("✅ DefeatScreenUI Awake complete - Panel hidden");
    }

    void Start()
    {
        // Setup retry button
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(OnRetryClicked);
            Debug.Log("✅ Retry button listener added");
        }
        else
        {
            Debug.LogWarning("⚠️ DefeatScreenUI: Retry button not assigned!");
        }
    }

    /// <summary>
    /// Hiển thị màn hình thua
    /// </summary>
    public void ShowDefeatScreen()
    {
        Debug.Log("💀 ShowDefeatScreen() called!");
        
        // IMPORTANT: Activate GameObject first
        gameObject.SetActive(true);
        Debug.Log("✅ DefeatPanel GameObject activated");
        
        // Show using CanvasGroup
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        Debug.Log("✅ DefeatPanel shown (CanvasGroup alpha = 1)");

        // Display default defeat text
        if (defeatMessageText != null)
        {
            defeatMessageText.text = "DEFEAT!";
            Debug.Log($"✅ Defeat message set: {defeatMessageText.text}");
        }
        else
        {
            Debug.LogWarning("⚠️ DefeatMessageText is NULL!");
        }

        // Play animation if available
        if (panelAnimator != null)
        {
            panelAnimator.SetTrigger("Show");
        }

        hasTransitioned = false;
        Debug.Log("💀 Defeat Screen setup complete!");
    }

    /// <summary>
    /// Xử lý khi nhấn nút "Thử lại"
    /// </summary>
    private void OnRetryClicked()
    {
        if (hasTransitioned)
        {
            Debug.LogWarning("⚠️ Already transitioning, ignoring click");
            return;
        }

        hasTransitioned = true;
        Debug.Log("🔄 Retrying battle...");
        
        // Reload current battle scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }



    void OnDestroy()
    {
        // Clean up listener
        if (retryButton != null)
        {
            retryButton.onClick.RemoveListener(OnRetryClicked);
        }
    }
}
