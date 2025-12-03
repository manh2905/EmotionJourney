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

    [Header("Combatants")]
    public PlayerBehaviour playerStats;
    public MonsterBehaviour currentMonster;

    [Header("UI")]
    public BattleUI battleUI;

    private bool isPlayerTurn = false;

    // ==========================
    // START
    // ==========================
    void Start()
    {
        if (!ValidateReferences()) return;
        StartBattle();
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
        return true;
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

        // 2️⃣ Rút bài / refill hand
        deckSystem.RevealAndRefillHand();

        // 3️⃣ Bắt đầu giai đoạn chọn bài
        draftManager.StartDraftPhase();

        // 4️⃣ UI
        battleUI?.ShowDraftPanel(true);
        battleUI?.ShowTurnStatus("Lượt của bạn - Chọn bài để tấn công!");
    }

    // ==========================
    // XỬ LÝ SAU KHI BẤM CONFIRM
    // ==========================
    public void ProcessPlayerActions(List<CardData> usedCards)
    {
        if (!isPlayerTurn) return;

        Debug.Log("=== PLAYER RESOLVE PHASE ===");

        // 1️⃣ Resolve effects lên player
        ResolveCards(usedCards);

        // 2️⃣ Check monster chết chưa
        if (currentMonster.IsDead())
        {
            EndBattle(true);
            return;
        }

        // 3️⃣ Kết thúc lượt player → Monster Attack
        EndPlayerTurn();
    }

    // ==========================
    // PLAYER RESOLVE
    // ==========================
    private void ResolveCards(List<CardData> cards)
    {
        // 1) Resolve effect: heal, buff, emotion…
        CardEffectExecutor.ExecuteEffects(cards, emometerSystem, staminaSystem, playerStats);

        // 2) Calculate Damage
        float totalDamage = CardDamageCalculator.CalculateTotalDamage(cards, emometerSystem);

        playerStats.Attack(totalDamage);

        // 3) Apply damage to monster
        currentMonster.TakeDamage(Mathf.RoundToInt(totalDamage));

        Debug.Log($"💥 Player gây {totalDamage} damage!");

        // 4) Discard used cards
        deckSystem.DiscardUsedCards(cards);
    }

    // ==========================
    // KẾT THÚC LƯỢT PLAYER
    // ==========================
    private void EndPlayerTurn()
    {
        isPlayerTurn = false;

        Debug.Log("───▶ END PLAYER TURN → MONSTER ATTACK");
        MonsterAttack();

        if (playerStats.GetCurrentHP() <= 0)
        {
            EndBattle(false);
            return;
        }

        if (!currentMonster.IsDead())
        {
            StartPlayerTurn(); // turn mới
        }
    }

    // ==========================
    // MONSTER ATTACK
    // ==========================
    private void MonsterAttack()
    {
        Debug.Log("=== MONSTER TURN ===");

        battleUI?.ShowTurnStatus("Lượt của Monster!");

        float dmg = currentMonster.Attack();
        playerStats.TakeDamage(dmg);

        battleUI?.ShowPlayerDamageEffect();

        Debug.Log($"👹 Monster gây {dmg} damage lên người chơi!");

        if (playerStats.GetCurrentHP() <= 0)
        {
            EndBattle(false);
        }
    }

    // ==========================
    // KẾT THÚC TRẬN ĐẤU
    // ==========================
    private void EndBattle(bool playerWon)
    {
        if (playerWon)
        {
            Debug.Log("🎉 Chiến thắng!");
            battleUI?.ShowVictory();

            MapController.UnlockNextLevel(BattleLoader.currentLevel);
            SceneManager.LoadScene("Map");
        }
        else
        {
            Debug.Log("💀 Thất bại!");
            battleUI?.ShowDefeat();
        }
    }
}
