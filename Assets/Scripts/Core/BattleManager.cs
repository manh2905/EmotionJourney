using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [Header("Core Systems")]
    public DeckSystem deckSystem;
    public StaminaSystem staminaSystem;
    public EmometerSystem emometerSystem;
    public DraftManager draftManager;
    public BattleCardManager cardManager;

    [Header("Combatants")]
    public PlayerBehaviour playerStats;
    public MonsterBehaviour currentMonster;

    [Header("UI")]
    public BattleUI battleUI;

    [Header("Audio")]
    public AudioClip backgroundMusic;
    public AudioClip playerAttackSound;
    public AudioClip monsterAttackSound;

    private bool isPlayerTurn = false;

    // ==========================
    // START
    // ==========================
    void Start()
    {
        Debug.Log("🔥 BattleManager START CALLED!");
        if (!ValidateReferences()) return;
        StartBattle();
    }
    void Awake()
    {
        Debug.Log("🔥 BattleManager AWAKE CALLED");
    }

    private bool ValidateReferences()
    {
        if (deckSystem == null ||
            staminaSystem == null ||
            emometerSystem == null ||
            draftManager == null ||
            playerStats == null ||
            currentMonster == null)
        {
            Debug.LogError("❌ BattleManager: Missing references!");
            enabled = false;
            return false;
        }

        StartBattle(); // Bắt đầu trận đấu

        if (SoundManager.Instance != null && backgroundMusic != null)
        {
            SoundManager.Instance.PlayMusic(backgroundMusic);
        }
    }

    // ==========================
    // BẮT ĐẦU TRẬN ĐẤU
    // ==========================
    public void StartBattle()
    {
        deckSystem.InitializeDeck();
        emometerSystem.Initialize();
        battleUI?.Initialize();

        StartPlayerTurn();
    }

    // ==========================
    // BẮT ĐẦU LƯỢT NGƯỜI CHƠI
    // ==========================
    public void StartPlayerTurn()
    {
        if (playerStats.GetCurrentHP() <= 0) return;
        if (currentMonster.IsDead()) return;

        Debug.Log("───▶ START PLAYER TURN");

        isPlayerTurn = true;

        // 1️⃣ Reset Stamina
        staminaSystem.ResetStamina();

        cardManager.RefillHand();

        // 2️⃣ Fill hand to 7 cards
        deckSystem.RevealAndRefillHand();

        // 3️⃣ Reset draft
        draftManager.StartDraftPhase();



        // 4️⃣ UI
        battleUI?.ShowDraftPanel(true);
        battleUI?.ShowTurnStatus("Lượt của bạn - Chọn bài để tấn công!");
    }

    // ==========================
    // SAU KHI NHẤN CONFIRM
    // ==========================
    public void ProcessPlayerActions(List<CardData> usedCards)
    {
        if (!isPlayerTurn) return;

        Debug.Log("=== PLAYER RESOLVE PHASE ===");

        // 1. Player resolve effects & damage
        ResolveCards(usedCards);

        // 2. Check monster death
        if (currentMonster.IsDead())
        {
            EndBattle(true);
            return;
        }

        // 3. Kết thúc lượt player (có delay)
        StartCoroutine(EndPlayerTurnCoroutine());
    }

    // ==========================
    // RESOLVE PLAYER ACTIONS
    // ==========================
    private void ResolveCards(List<CardData> cards)
    {
        // Effects (heal, emotion…)
        CardEffectExecutor.ExecuteEffects(cards, emometerSystem, staminaSystem, playerStats);

        // Damage calculation
        float totalDamage = CardDamageCalculator.CalculateTotalDamage(cards, emometerSystem);

        playerStats.Attack(Mathf.RoundToInt(totalDamage));


        // Apply damage
        // Apply damage
        currentMonster.TakeDamage(Mathf.RoundToInt(totalDamage));

        if (SoundManager.Instance != null && playerAttackSound != null)
        {
            SoundManager.Instance.PlaySFX(playerAttackSound);
        }
 
        Debug.Log($"Sát thương cuối cùng lên Monster: {totalDamage}");

        Debug.Log($"💥 Player gây {totalDamage} damage lên quái!");

        // Discard cards
        deckSystem.DiscardUsedCards(cards);
    }

    // ==========================
    // END PLAYER TURN + DELAY
    // ==========================
    private IEnumerator EndPlayerTurnCoroutine()
    {
        isPlayerTurn = false;

        Debug.Log("───▶ END PLAYER TURN");

        // Delay trước khi quái tấn công
        yield return new WaitForSeconds(1f);

        // Monster Attack
        yield return StartCoroutine(MonsterAttackCoroutine());

        // Player chết?
        if (playerStats.GetCurrentHP() <= 0)
        {
            EndBattle(false);
            yield break;
        }

        // Delay trước turn mới
        yield return new WaitForSeconds(1f);

        // Start next turn
        StartPlayerTurn();
    }

    float damage = currentMonster.Attack();     // Monster trả damage

    if (SoundManager.Instance != null && monsterAttackSound != null)
    {
        SoundManager.Instance.PlaySFX(monsterAttackSound);
    }
    playerStats.TakeDamage(damage);           // Player nhận damage
    
    // Show damage effect
    if (battleUI != null)
    // ==========================
    // MONSTER ATTACK (with delay)
    // ==========================
    private IEnumerator MonsterAttackCoroutine()
    {
        Debug.Log("=== MONSTER TURN ===");
        battleUI?.ShowTurnStatus("Monster đang tấn công!");

        // Delay nhỏ để UI update
        yield return new WaitForSeconds(0.7f);

        float dmg = currentMonster.Attack();
        playerStats.TakeDamage(dmg);

        battleUI?.ShowPlayerDamageEffect();

        Debug.Log($"👹 Monster gây {dmg} damage!");

        // THUA TRẬN
        if (playerStats.GetCurrentHP() <= 0)
        {
            EndBattle(false);
            yield break;
        }
    }

    // ==========================
    // END BATTLE
    // ==========================
    private void EndBattle(bool playerWon)
    {
        if (playerWon)
        {
            Debug.Log("🎉 Bạn đã thắng!");
            battleUI?.ShowVictory();

            MapController.UnlockNextLevel(BattleLoader.currentLevel);
            SceneManager.LoadScene("Map");
        }
        else
        {
            Debug.Log("💀 Bạn đã thua!");
            battleUI?.ShowDefeat();
        }
    }
}
