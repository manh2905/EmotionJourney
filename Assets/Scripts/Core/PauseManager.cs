using UnityEngine;

/// <summary>
/// Quản lý pause state của game, detect ESC key
/// Attach script này vào GameObject trong scene (hoặc GameManager)
/// </summary>
public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    
    [Header("References")]
    public PauseMenuUI pauseMenuUI;
    
    [Header("Settings")]
    public KeyCode pauseKey = KeyCode.Escape;
    
    private bool isPaused = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        Debug.Log("✅ PauseManager initialized");
    }

    void Update()
    {
        // Detect ESC key press
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    /// <summary>
    /// Pause game
    /// </summary>
    public void PauseGame()
    {
        if (isPaused) return;
        
        isPaused = true;
        Time.timeScale = 0f; // Dừng thời gian
        
        if (pauseMenuUI != null)
        {
            pauseMenuUI.ShowMenu();
        }
        else
        {
            Debug.LogError("❌ PauseMenuUI is NULL!");
        }
        
        Debug.Log("⏸️ Game PAUSED");
    }

    /// <summary>
    /// Resume game
    /// </summary>
    public void ResumeGame()
    {
        if (!isPaused) return;
        
        isPaused = false;
        Time.timeScale = 1f; // Tiếp tục thời gian
        
        if (pauseMenuUI != null)
        {
            pauseMenuUI.HideMenu();
        }
        
        Debug.Log("▶️ Game RESUMED");
    }

    /// <summary>
    /// Check if game is currently paused
    /// </summary>
    public bool IsPaused()
    {
        return isPaused;
    }

    void OnDestroy()
    {
        // Reset time scale when destroyed
        Time.timeScale = 1f;
        
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
