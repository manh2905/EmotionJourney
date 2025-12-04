using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Quản lý màn hình chiến thắng với rewards và nút quay về map
/// Attach script này vào VictoryPanel GameObject
/// </summary>
public class VictoryScreenUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI rewardsText;
    public Button continueButton;
    
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
            Debug.Log("✅ Added CanvasGroup to VictoryPanel");
        }
        
        // Hide panel initially using CanvasGroup
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        Debug.Log("✅ VictoryScreenUI Awake complete - Panel hidden");
    }

    void Start()
    {
        // Setup continue button
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
            Debug.Log("✅ Continue button listener added");
        }
        else
        {
            Debug.LogWarning("⚠️ VictoryScreenUI: Continue button not assigned!");
        }
    }

    /// <summary>
    /// Hiển thị màn hình chiến thắng với rewards
    /// </summary>
    /// <param name="rewardMessage">Text mô tả phần thưởng</param>
    public void ShowVictoryScreen(string rewardMessage = "")
    {
        Debug.Log("🎉 ShowVictoryScreen() called!");
        
        // Show using CanvasGroup
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        Debug.Log("✅ VictoryPanel shown (CanvasGroup alpha = 1)");

        // Display rewards text
        if (rewardsText != null)
        {
            if (string.IsNullOrEmpty(rewardMessage))
            {
                rewardsText.text = "HP: +50\nCARDS: +5 (+1 ATK PER TYPE)";
            }
            else
            {
                rewardsText.text = rewardMessage;
            }
            Debug.Log($"✅ Rewards text set: {rewardsText.text}");
        }
        else
        {
            Debug.LogWarning("⚠️ RewardsText is NULL!");
        }

        // Play animation if available
        if (panelAnimator != null)
        {
            panelAnimator.SetTrigger("Show");
        }

        hasTransitioned = false;
        Debug.Log("🎉 Victory Screen setup complete!");
    }

    /// <summary>
    /// Xử lý khi nhấn nút "Tiếp"
    /// </summary>
    private void OnContinueClicked()
    {
        if (hasTransitioned)
        {
            Debug.LogWarning("⚠️ Already transitioning to Map scene, ignoring click");
            return;
        }

        hasTransitioned = true;
        Debug.Log("🗺️ Loading Map scene...");
        
        // Load Map scene
        SceneManager.LoadScene("Map");
    }

    /// <summary>
    /// Tạo reward message dựa trên level (có thể customize)
    /// </summary>
    public static string GenerateRewardMessage(int level)
    {
        int hpbonus = 50;
        int staminabonus = 1;
        int cardsbonus = 5;
        
        return $"HP: +{hpbonus}\nSTAMINA: +{staminabonus}\nCARDS: +{cardsbonus} (+1 ATK PER TYPE)";
    }

    void OnDestroy()
    {
        // Clean up listener
        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(OnContinueClicked);
        }
    }
}

