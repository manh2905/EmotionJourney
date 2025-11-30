using UnityEngine;
using TMPro;

/// <summary>
/// Script để test UI trong Play Mode bằng keyboard
/// Attach vào GameObject bất kỳ trong scene để test
/// </summary>
public class UITester : MonoBehaviour
{
    [Header("References - Assign in Inspector")]
    public BattleUI battleUI;
    public PlayerStats playerStats;
    public StaminaSystem staminaSystem;
    public EmometerSystem emometerSystem;
    public MonsterBehaviour currentMonster;

    [Header("Test Settings")]
    public float damageAmount = 10f;
    public int staminaChangeAmount = 1;
    public int emotionChangeAmount = 1;

    [Header("UI Display (Optional)")]
    public TextMeshProUGUI instructionText;

    void Start()
    {
        if (instructionText != null)
        {
            instructionText.text = @"=== UI TESTER ===
[1] -10 HP Player
[2] +10 HP Player
[3] -1 Stamina
[4] +1 Stamina (max 5)
[5] -1 Emotion
[6] +1 Emotion
[7] Reset Stamina
[8] -20 HP Monster
[Q] Show Victory
[E] Show Defeat
[R] Reset Battle UI
[T] Toggle Draft Panel";
        }

        Debug.Log("=== UI TESTER ACTIVE ===");
        Debug.Log("Press keys to test UI:");
        Debug.Log("[1][2] HP | [3][4] Stamina | [5][6] Emotion");
        Debug.Log("[8] Monster Damage | [Q] Victory | [E] Defeat");
    }

    void Update()
    {
        // ========== PLAYER HP TESTS ==========
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            TestPlayerDamage();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            TestPlayerHeal();
        }

        // ========== STAMINA TESTS ==========
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            TestStaminaDecrease();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            TestStaminaIncrease();
        }

        if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
        {
            TestStaminaReset();
        }

        // ========== EMOTION TESTS ==========
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            TestEmotionDecrease();
        }

        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            TestEmotionIncrease();
        }

        // ========== MONSTER HP TEST ==========
        if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
        {
            TestMonsterDamage();
        }

        // ========== UI PANEL TESTS ==========
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TestVictoryScreen();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            TestDefeatScreen();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            TestResetUI();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            TestToggleDraftPanel();
        }
    }

    // ========== TEST METHODS ==========

    void TestPlayerDamage()
    {
        if (playerStats != null)
        {
            playerStats.TakeDamage(damageAmount);
            Debug.Log($"✅ Player took {damageAmount} damage. HP: {playerStats.GetCurrentHP()}/{playerStats.maxHp}");
            
            if (battleUI != null)
            {
                battleUI.ShowPlayerDamageEffect();
            }
        }
        else
        {
            Debug.LogWarning("❌ PlayerStats not assigned!");
        }
    }

    void TestPlayerHeal()
    {
        if (playerStats != null)
        {
            playerStats.Heal(damageAmount);
            Debug.Log($"✅ Player healed {damageAmount} HP. HP: {playerStats.GetCurrentHP()}/{playerStats.maxHp}");
        }
        else
        {
            Debug.LogWarning("❌ PlayerStats not assigned!");
        }
    }

    void TestStaminaDecrease()
    {
        if (staminaSystem != null)
        {
            int currentStamina = staminaSystem.GetCurrentStamina();
            if (currentStamina > 0)
            {
                staminaSystem.ConsumeStamina(staminaChangeAmount);
                Debug.Log($"✅ Stamina decreased. Current: {staminaSystem.GetCurrentStamina()}/{staminaSystem.maxStamina}");
            }
            else
            {
                Debug.Log("⚠️ No stamina left!");
                if (battleUI != null)
                {
                    battleUI.ShowInsufficientStaminaWarning();
                }
            }
        }
        else
        {
            Debug.LogWarning("❌ StaminaSystem not assigned!");
        }
    }

    void TestStaminaIncrease()
    {
        if (staminaSystem != null)
        {
            // Manually increase stamina (need to add this to StaminaSystem if it doesn't exist)
            int currentStamina = staminaSystem.GetCurrentStamina();
            if (currentStamina < staminaSystem.maxStamina)
            {
                // Since there's no AddStamina method, we'll just reset
                Debug.Log("ℹ️ Use [7] to reset stamina to max");
            }
            else
            {
                Debug.Log("⚠️ Stamina already full!");
            }
        }
        else
        {
            Debug.LogWarning("❌ StaminaSystem not assigned!");
        }
    }

    void TestStaminaReset()
    {
        if (staminaSystem != null)
        {
            staminaSystem.ResetStamina();
            Debug.Log($"✅ Stamina reset to max: {staminaSystem.GetCurrentStamina()}/{staminaSystem.maxStamina}");
        }
        else
        {
            Debug.LogWarning("❌ StaminaSystem not assigned!");
        }
    }

    void TestEmotionDecrease()
    {
        if (emometerSystem != null)
        {
            emometerSystem.ShiftEmotion(-emotionChangeAmount);
            Debug.Log($"✅ Emotion decreased. Current: {emometerSystem.CurrentEmotion} | Burnout: {emometerSystem.isBurnedOut}");
        }
        else
        {
            Debug.LogWarning("❌ EmometerSystem not assigned!");
        }
    }

    void TestEmotionIncrease()
    {
        if (emometerSystem != null)
        {
            emometerSystem.ShiftEmotion(emotionChangeAmount);
            Debug.Log($"✅ Emotion increased. Current: {emometerSystem.CurrentEmotion} | Burnout: {emometerSystem.isBurnedOut}");
        }
        else
        {
            Debug.LogWarning("❌ EmometerSystem not assigned!");
        }
    }

    void TestMonsterDamage()
    {
        if (currentMonster != null)
        {
            currentMonster.TakeDamage(Mathf.RoundToInt(damageAmount * 2));
            Debug.Log($"✅ Monster took damage. HP: {currentMonster.GetCurrentHP()}/{currentMonster.GetMaxHP()}");
        }
        else
        {
            Debug.LogWarning("❌ Monster not assigned!");
        }
    }

    void TestVictoryScreen()
    {
        if (battleUI != null)
        {
            battleUI.ShowVictory();
            Debug.Log("✅ Victory screen shown!");
        }
        else
        {
            Debug.LogWarning("❌ BattleUI not assigned!");
        }
    }

    void TestDefeatScreen()
    {
        if (battleUI != null)
        {
            battleUI.ShowDefeat();
            Debug.Log("✅ Defeat screen shown!");
        }
        else
        {
            Debug.LogWarning("❌ BattleUI not assigned!");
        }
    }

    void TestResetUI()
    {
        if (battleUI != null)
        {
            battleUI.Initialize();
            Debug.Log("✅ Battle UI reset/reinitialized!");
        }
        else
        {
            Debug.LogWarning("❌ BattleUI not assigned!");
        }
    }

    private bool isDraftPanelVisible = false;
    void TestToggleDraftPanel()
    {
        if (battleUI != null)
        {
            isDraftPanelVisible = !isDraftPanelVisible;
            battleUI.ShowDraftPanel(isDraftPanelVisible);
            Debug.Log($"✅ Draft panel: {(isDraftPanelVisible ? "SHOWN" : "HIDDEN")}");
        }
        else
        {
            Debug.LogWarning("❌ BattleUI not assigned!");
        }
    }
}
