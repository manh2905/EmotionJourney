using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Controller chính quản lý toàn bộ UI trong Battle Scene
/// Kết nối với các game systems và cập nhật UI real-time
/// </summary>
public class BattleUI : MonoBehaviour
{
    [Header("Game Systems References")]
    public PlayerBehaviour playerStats;
    public StaminaSystem staminaSystem;
    public EmometerSystem emometerSystem;
    public MonsterBehaviour currentMonster;

    [Header("UI Component References")]
    public HPUI playerHPUI;                     // HP bar của player
    public HPUI monsterHPUI;                    // HP bar của monster (optional)
    public StaminaUI staminaUI;
    public EmotionUI emotionUI;

    [Header("UI Panels")]
    public GameObject battlePanel;              // Main battle UI panel
    public GameObject victoryPanel;             // Victory screen
    public GameObject defeatPanel;              // Defeat screen
    public GameObject draftPanel;               // Draft phase panel

    [Header("Monster Info")]
    public TextMeshProUGUI monsterNameText;
    public Image monsterSprite;

    [Header("Turn Indicator")]
    public TextMeshProUGUI turnStatusText;      // "Your Turn", "Enemy Turn"...

    private bool isInitialized = false;

    void Start()
    {
        Debug.Log("🔵 BattleUI.Start() called!");
        Initialize();
    }

    /// <summary>
    /// Khởi tạo toàn bộ Battle UI
    /// </summary>
    public void Initialize()
    {
        Debug.Log("🔵 BattleUI.Initialize() called!");
        
        if (isInitialized)
        {
            Debug.Log("⚠️ BattleUI already initialized, skipping...");
            return;
        }

        // Validate references - WARNING instead of ERROR to allow partial functionality
        bool hasAllReferences = true;
        
        if (playerStats == null)
        {
            Debug.LogWarning("❌ BattleUI: PlayerStats reference missing!");
            hasAllReferences = false;
        }
        
        if (staminaSystem == null)
        {
            Debug.LogWarning("❌ BattleUI: StaminaSystem reference missing!");
            hasAllReferences = false;
        }
        
        if (emometerSystem == null)
        {
            Debug.LogWarning("❌ BattleUI: EmometerSystem reference missing!");
            hasAllReferences = false;
        }

        // Initialize UI components (even if some references are missing)
        InitializePlayerHPUI();
        InitializeStaminaUI();
        InitializeEmotionUI();
        InitializeMonsterUI();

        // Hide victory/defeat panels
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);

        // Show battle panel
        if (battlePanel != null) battlePanel.SetActive(true);

        isInitialized = true;
        
        if (hasAllReferences)
        {
            Debug.Log("✅ BattleUI initialized successfully with all references!");
        }
        else
        {
            Debug.LogWarning("⚠️ BattleUI initialized but some references are missing!");
        }
    }

    /// <summary>
    /// Khởi tạo Player HP UI
    /// </summary>
    private void InitializePlayerHPUI()
    {
        if (playerHPUI != null && playerStats != null)
        {
            playerHPUI.SetMaxHP(playerStats.data.maxHP);
            playerHPUI.UpdateHP(playerStats.GetCurrentHP(), playerStats.data.maxHP);
            
            // Subscribe to HP change event để tự động update UI
            playerStats.OnHPChanged.AddListener(OnPlayerHPChanged);
            
            Debug.Log("✅ Player HP UI initialized");
        }
        else
        {
            Debug.LogWarning($"⚠️ Player HP UI not initialized. HPUI: {playerHPUI != null}, PlayerStats: {playerStats != null}");
        }
    }

    /// <summary>
    /// Callback khi Player HP thay đổi
    /// </summary>
    private void OnPlayerHPChanged(float currentHP, float maxHP)
    {
        if (playerHPUI != null)
        {
            playerHPUI.UpdateHP(currentHP, maxHP);
            Debug.Log($"🔄 Player HP UI updated: {currentHP}/{maxHP}");
        }
    }

    /// <summary>
    /// Khởi tạo Stamina UI
    /// </summary>
    private void InitializeStaminaUI()
    {
        if (staminaUI != null && staminaSystem != null)
        {
            staminaUI.Initialize(staminaSystem.maxStamina);
            staminaUI.UpdateStamina(staminaSystem.GetCurrentStamina(), staminaSystem.maxStamina);
            Debug.Log("✅ Stamina UI initialized");
        }
        else
        {
            Debug.LogWarning($"⚠️ Stamina UI not initialized. StaminaUI: {staminaUI != null}, StaminaSystem: {staminaSystem != null}");
        }
    }

    /// <summary>
    /// Khởi tạo Emotion UI
    /// </summary>
    private void InitializeEmotionUI()
    {
        if (emotionUI != null && emometerSystem != null)
        {
            emotionUI.UpdateEmotion(
                emometerSystem.CurrentEmotion,
                emometerSystem.isBurnedOut,
                emometerSystem.isPositiveBurnout
            );
            Debug.Log("✅ Emotion UI initialized");
        }
        else
        {
            Debug.LogWarning($"⚠️ Emotion UI not initialized. EmotionUI: {emotionUI != null}, EmometerSystem: {emometerSystem != null}");
        }
    }

    /// <summary>
    /// Khởi tạo Monster UI
    /// </summary>
    private void InitializeMonsterUI()
    {
        if (currentMonster != null)
        {
            // Update monster name
            if (monsterNameText != null)
            {
                monsterNameText.text = currentMonster.monsterName;
            }

            // Update monster HP if monsterHPUI exists
            if (monsterHPUI != null)
            {
                monsterHPUI.SetMaxHP(currentMonster.GetMaxHP());
                monsterHPUI.UpdateHP(currentMonster.GetCurrentHP(), currentMonster.GetMaxHP());
            }
            Debug.Log("✅ Monster UI initialized");
        }
        else
        {
            Debug.LogWarning("⚠️ Monster UI not initialized. CurrentMonster is null.");
        }
    }

    /// <summary>
    /// Update được gọi mỗi frame để sync UI với game state
    /// TODO: Convert to events for better performance
    /// </summary>
    void Update()
    {
        if (!isInitialized) return;

        // Player HP - Đã dùng Event, không cần poll nữa! ✅

        // Update Stamina (TODO: Convert to event)
        if (staminaUI != null && staminaSystem != null)
        {
            staminaUI.UpdateStamina(staminaSystem.GetCurrentStamina(), staminaSystem.maxStamina);
        }

        // Update Emotion (TODO: Convert to event)
        if (emotionUI != null && emometerSystem != null)
        {
            emotionUI.UpdateEmotion(
                emometerSystem.CurrentEmotion,
                emometerSystem.isBurnedOut,
                emometerSystem.isPositiveBurnout
            );
        }

        // Update Monster HP (TODO: Convert to event)
        if (monsterHPUI != null && currentMonster != null)
        {
            monsterHPUI.UpdateHP(currentMonster.GetCurrentHP(), currentMonster.GetMaxHP());
        }
    }

    // ============ PUBLIC METHODS (Called by BattleManager) ============

    /// <summary>
    /// Hiển thị trạng thái lượt chơi
    /// </summary>
    public void ShowTurnStatus(string status)
    {
        if (turnStatusText != null)
        {
            turnStatusText.text = status;
        }
    }

    /// <summary>
    /// Hiển thị Draft Panel (chọn bài)
    /// </summary>
    public void ShowDraftPanel(bool show)
    {
        if (draftPanel != null)
        {
            draftPanel.SetActive(show);
        }
    }

    /// <summary>
    /// Hiển thị Victory Screen
    /// </summary>
    public void ShowVictory()
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);
        if (battlePanel != null) battlePanel.SetActive(false);
    }

    /// <summary>
    /// Hiển thị Defeat Screen
    /// </summary>
    public void ShowDefeat()
    {
        if (defeatPanel != null) defeatPanel.SetActive(true);
        if (battlePanel != null) battlePanel.SetActive(false);
    }

    /// <summary>
    /// Play damage effect trên Player UI
    /// </summary>
    public void ShowPlayerDamageEffect()
    {
        if (playerHPUI != null)
        {
            playerHPUI.ShowDamageEffect();
        }
    }

    /// <summary>
    /// Play stamina insufficient effect
    /// </summary>
    public void ShowInsufficientStaminaWarning()
    {
        if (staminaUI != null)
        {
            staminaUI.ShowInsufficientStaminaEffect();
        }
    }
}
