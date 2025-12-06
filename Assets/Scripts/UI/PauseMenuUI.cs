using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Quản lý Pause Menu UI với các nút Resume, Sound, Map, Exit
/// Attach script này vào PauseMenuPanel GameObject
/// </summary>
public class PauseMenuUI : MonoBehaviour
{
    [Header("UI References")]
    public Button resumeButton;
    public Button soundButton;
    public Button mapButton;
    public Button guideButton;
    public Button exitButton;
    
    [Header("Guide Panel")]
    public GameObject guidePanel;
    
    [Header("Sound Button Sprites (Optional)")]
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;
    public Image soundButtonImage;
    
    [Header("Optional Animation")]
    public Animator panelAnimator;
    
    private CanvasGroup canvasGroup;
    private bool isSoundOn = true;

    void Awake()
    {
        // Get or add CanvasGroup component
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            Debug.Log("✅ Added CanvasGroup to PauseMenuPanel");
        }
        
        // Hide panel initially
        HideMenu();
        
        // Đảm bảo Guide Panel cũng được ẩn khi khởi tạo
        // Tránh việc Guide Panel hiện lên trước Pause Menu khi nhấn ESC
        if (guidePanel != null)
        {
            guidePanel.SetActive(false);
            Debug.Log("✅ Guide Panel hidden on initialization");
        }
        
        Debug.Log("✅ PauseMenuUI Awake complete");
    }

    void Start()
    {
        // Setup button listeners
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(OnResumeClicked);
        }
        else
        {
            Debug.LogWarning("⚠️ PauseMenuUI: Resume button not assigned!");
        }
        
        if (soundButton != null)
        {
            soundButton.onClick.AddListener(OnSoundToggleClicked);
        }
        else
        {
            Debug.LogWarning("⚠️ PauseMenuUI: Sound button not assigned!");
        }
        
        if (mapButton != null)
        {
            mapButton.onClick.AddListener(OnMapClicked);
        }
        else
        {
            Debug.LogWarning("⚠️ PauseMenuUI: Map button not assigned!");
        }
        
        if (guideButton != null)
        {
            guideButton.onClick.AddListener(OnGuideClicked);
        }
        else
        {
            Debug.LogWarning("⚠️ PauseMenuUI: Guide button not assigned!");
        }
        
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitClicked);
        }
        else
        {
            Debug.LogWarning("⚠️ PauseMenuUI: Exit button not assigned!");
        }
        
        // Initialize sound state
        UpdateSoundButtonVisual();
    }

    /// <summary>
    /// Hiển thị Pause Menu
    /// </summary>
    public void ShowMenu()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        if (panelAnimator != null)
        {
            panelAnimator.SetTrigger("Show");
        }
        
        Debug.Log("⏸️ Pause Menu shown");
    }

    /// <summary>
    /// Ẩn Pause Menu
    /// </summary>
    public void HideMenu()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        // Đảm bảo Guide Panel cũng được ẩn khi đóng Pause Menu
        // Tránh trường hợp Guide Panel vẫn hiển thị sau khi thoát Pause Menu
        if (guidePanel != null && guidePanel.activeSelf)
        {
            guidePanel.SetActive(false);
            Debug.Log("📖 Guide Panel closed with Pause Menu");
        }
        
        Debug.Log("▶️ Pause Menu hidden");
    }

    /// <summary>
    /// Xử lý khi nhấn Resume
    /// </summary>
    private void OnResumeClicked()
    {
        Debug.Log("▶️ Resume game");
        PauseManager.Instance?.ResumeGame();
    }

    /// <summary>
    /// Xử lý khi nhấn Sound Toggle
    /// </summary>
    private void OnSoundToggleClicked()
    {
        isSoundOn = !isSoundOn;
        
        // Toggle sound via SoundManager
        if (SoundManager.Instance != null)
        {
            if (isSoundOn)
            {
                SoundManager.Instance.UnmuteAll();
                Debug.Log("🔊 Sound ON");
            }
            else
            {
                SoundManager.Instance.MuteAll();
                Debug.Log("🔇 Sound OFF");
            }
        }
        
        UpdateSoundButtonVisual();
    }

    /// <summary>
    /// Update sound button sprite
    /// </summary>
    private void UpdateSoundButtonVisual()
    {
        if (soundButtonImage != null && soundOnSprite != null && soundOffSprite != null)
        {
            soundButtonImage.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
        }
    }

    /// <summary>
    /// Xử lý khi nhấn Map (về map scene)
    /// </summary>
    private void OnMapClicked()
    {
        Debug.Log("🗺️ Loading Map scene...");
        
        // Resume time before changing scene
        Time.timeScale = 1f;
        
        SceneManager.LoadScene("Map");
    }

    /// <summary>
    /// Xử lý khi nhấn Guide
    /// </summary>
    private void OnGuideClicked()
    {
        Debug.Log("📖 Opening Guide...");
        
        if (guidePanel != null)
        {
            guidePanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("⚠️ Guide panel not assigned!");
        }
    }
    
    // Method đơn giản để close guide panel - dùng cho Button onClick
    public void CloseGuidePanel()
    {
        if (guidePanel != null)
            guidePanel.SetActive(false);
    }
    
    /// <summary>
    /// Đóng Guide Panel (được gọi từ close button trong guide panel)
    /// </summary>
    public void CloseGuide()
    {
        Debug.Log("📖 Closing Guide...");
        
        if (guidePanel != null)
        {
            guidePanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Xử lý khi nhấn Exit (về main menu)
    /// </summary>
    private void OnExitClicked()
    {
        Debug.Log("🚪 Exiting to Main Menu...");
        
        // Resume time before changing scene
        Time.timeScale = 1f;
        
        // Tên scene main menu của bạn (thường là "MainMenu", "Menu", hoặc "Start")
        SceneManager.LoadScene("MainMenu");
    }

    void OnDestroy()
    {
        // Clean up listeners
        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(OnResumeClicked);
        
        if (soundButton != null)
            soundButton.onClick.RemoveListener(OnSoundToggleClicked);
        
        if (mapButton != null)
            mapButton.onClick.RemoveListener(OnMapClicked);
        
        if (guideButton != null)
            guideButton.onClick.RemoveListener(OnGuideClicked);
        
        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitClicked);
    }
}
