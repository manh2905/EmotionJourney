using UnityEngine;
using System.Collections.Generic;

// Đây là lớp điều phối chính, quản lý vòng lặp chiến đấu (turn-based)
public class BattleManager : MonoBehaviour
{
    // Cần phải gán các tham chiếu này trong Inspector
    [Header("Core Systems")]
    public DeckSystem deckSystem;
    public StaminaSystem staminaSystem;
    public EmometerSystem emometerSystem; 
    public DraftManager draftManager;

    [Header("Combatants")]
    public PlayerBehaviour playerStats; // HP, Dodge của người chơi
    public MonsterBehaviour currentMonster; // HP, ATK của quái vật

    private bool isPlayerTurn = false;

    void Start()
    {
        // Kiểm tra các tham chiếu đã được gán chưa (rất quan trọng)
        if (deckSystem == null || staminaSystem == null || emometerSystem == null || draftManager == null || playerStats == null || currentMonster == null)
        {
            Debug.LogError("LỖI KHỞI TẠO: Một số hệ thống cốt lõi hoặc Combatants (Player/Monster) chưa được gán trong Inspector của BattleManager.");
            enabled = false; 
            return;
        }

        StartBattle(); // Bắt đầu trận đấu
    }

    public void StartBattle()
    {
        // Khởi tạo các hệ thống quan trọng
        deckSystem.InitializeDeck();
        emometerSystem.Initialize(); 
        
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        // Kiểm tra điều kiện thắng/thua trước khi bắt đầu lượt mới
        if (playerStats.GetCurrentHP() <= 0) return; 
        if (currentMonster.IsDead()) return; 

        isPlayerTurn = true;
        staminaSystem.ResetStamina(); 
        
        // REVEAL: Rút bài (luôn đảm bảo 7 lá)
        deckSystem.RevealAndRefillHand();

        draftManager.StartDraftPhase(); 
        Debug.Log("Lượt của bạn. Hãy chọn 3 lá bài.");
    }

    // Phương thức được gọi từ DraftManager sau khi người chơi CONFIRM DRAFT
    public void ProcessPlayerActions(List<CardData> usedCards)
    {
        if (!isPlayerTurn) return;
        
        // BƯỚC RESOLVE
        ResolveCards(usedCards);
        
        // Kiểm tra thắng ngay lập tức (trước khi Monster Attack)
        if (currentMonster.IsDead())
        {
            EndBattle(true);
            return;
        }

        // Kết thúc lượt người chơi và chuyển sang lượt Monster
        EndPlayerTurn();
    }

    private void ResolveCards(List<CardData> cards)
    {
        Debug.Log("--- RESOLVE PHASE (Thực thi) ---");
        
        // 1. Áp dụng hiệu ứng Emometer và các hiệu ứng khác (Heal, Mất máu)
        // Cần truyền PlayerStats để CardEffectExecutor xử lý HP
        CardEffectExecutor.ExecuteEffects(cards, emometerSystem, staminaSystem, playerStats);
        
        // 2. Tính toán Damage tổng 
        float totalDamage = CardDamageCalculator.CalculateTotalDamage(cards, emometerSystem);

        // 3. Gây sát thương lên Monster
        currentMonster.TakeDamage(Mathf.RoundToInt(totalDamage));
 
        Debug.Log($"Sát thương cuối cùng lên Monster: {totalDamage}");

        // 4. Dọn dẹp: Đưa các lá bài đã dùng vào Discard Pile
        deckSystem.DiscardUsedCards(cards); 
    }
    
    public void EndPlayerTurn()
    {
        isPlayerTurn = false;
        Debug.Log("Kết thúc lượt người chơi. Chuyển sang lượt Monster.");
        
        // Gọi Monster Attack (D)
        Attack();
        
        // Bắt đầu lại vòng chiến đấu nếu chưa kết thúc
        if (!currentMonster.IsDead()
        && playerStats.GetCurrentHP() > 0)
        {
            Debug.Log("--------------------------------------");
            StartPlayerTurn(); 
        }
    }

   private void Attack()
{
    Debug.Log("--- LƯỢT CỦA MONSTER ---");

    float damage = currentMonster.Attack();     // Monster trả damage
    playerStats.TakeDamage(damage);           // Player nhận damage

    Debug.Log($"Monster gây {damage} sát thương lên Player!");

    if (playerStats.GetCurrentHP() <= 0)
    {
        EndBattle(false);
    }
}


    private void EndBattle(bool playerWon)
    {
        if (playerWon)
        {
            Debug.Log("--- CHIẾN THẮNG! (Monster đã bị đánh bại) ---");
            // Logic nhận thưởng, chuyển Scene Map
        }
        else
        {
            Debug.Log("--- BẠN ĐÃ THUA CUỘC (HP = 0) ---");
            // Logic Game Over
        }
    }
}