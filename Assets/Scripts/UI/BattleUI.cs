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
        Initialize();
    }

    /// <summary>
    /// Khởi tạo toàn bộ Battle UI
    /// </summary>
    public void Initialize()
    {
        if (isInitialized) return;

        // Validate references
        if (playerStats == null || staminaSystem == null || emometerSystem == null)
        {
            Debug.LogError("BattleUI: Missing game system references!");
            return;
        }

        // Initialize UI components
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
        Debug.Log("BattleUI initialized successfully!");
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
        }
    }

    /// <summary>
    /// Update được gọi mỗi frame để sync UI với game state
    /// (Alternative: Sử dụng Events thay vì Update)
    /// </summary>
    void Update()
    {
        if (!isInitialized) return;

        // Update Player HP
        if (playerHPUI != null && playerStats != null)
        {
            playerHPUI.UpdateHP(playerStats.GetCurrentHP(), playerStats.data.maxHP);
        }

        // Update Stamina
        if (staminaUI != null && staminaSystem != null)
        {
            staminaUI.UpdateStamina(staminaSystem.GetCurrentStamina(), staminaSystem.maxStamina);
        }

        // Update Emotion
        if (emotionUI != null && emometerSystem != null)
        {
            emotionUI.UpdateEmotion(
                emometerSystem.CurrentEmotion,
                emometerSystem.isBurnedOut,
                emometerSystem.isPositiveBurnout
            );
        }

        // Update Monster HP
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
