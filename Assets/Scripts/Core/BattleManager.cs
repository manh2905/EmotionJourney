using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    // Cần phải gán các tham chiếu này trong Inspector
    [Header("Core Systems")]
    public DeckSystem deckSystem;
    public StaminaSystem staminaSystem;
    public EmometerSystem emometerSystem; 
    public DraftManager draftManager;

    private bool isPlayerTurn = false;

    void Start()
    {
        // Khởi tạo các hệ thống (Đảm bảo các component đã được gán)
        // Ví dụ:
        // deckSystem.InitializeDeck();
        // emometerSystem.Initialize();

        // StartBattle(); // Bắt đầu trận đấu
    }

    public void StartBattle()
    {
        // ... (Logic khởi tạo trận đấu) ...
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        isPlayerTurn = true;
        staminaSystem.ResetStamina(); // Hồi lại Stamina
        
        // REVEAL: Rút bài (luôn đảm bảo 7 lá)
        // deckSystem.RevealAndRefillHand();

        draftManager.StartDraftPhase(); // Bắt đầu giai đoạn chọn bài
        Debug.Log("Lượt của bạn. Hãy chọn 3 lá bài.");
    }

    // PHƯƠNG THỨC MỚI ĐƯỢC GỌI TỪ DRAFTMANAGER (Giải quyết lỗi CS1061)
    public void ProcessPlayerActions(List<CardData> usedCards)
    {
        if (!isPlayerTurn) return;
        
        // BƯỚC RESOLVE
        ResolveCards(usedCards);
        
        // Kết thúc lượt người chơi và chuyển sang lượt Monster
        EndPlayerTurn();
    }

    private void ResolveCards(List<CardData> cards)
    {
        Debug.Log("--- RESOLVE PHASE (Thực thi) ---");
        
        // 1. Áp dụng hiệu ứng Emometer và các hiệu ứng khác
        CardEffectExecutor.ExecuteEffects(cards, emometerSystem, staminaSystem);
        
        // 2. Tính toán Damage tổng
        float totalDamage = CardDamageCalculator.CalculateTotalDamage(cards, emometerSystem);

        // 3. Gây sát thương lên Monster
        // currentMonster.TakeDamage(totalDamage); 

        // 4. Dọn dẹp: Đưa các lá bài đã dùng vào Discard Pile
        deckSystem.DiscardUsedCards(cards); 
    }
    
    public void EndPlayerTurn()
    {
        isPlayerTurn = false;
        Debug.Log("Kết thúc lượt người chơi. Chuyển sang lượt Monster.");
        
        // Gọi Monster Attack (Sẽ làm ở bước sau)
        // MonsterAttack();
        
        // Sau khi Monster Attack xong, gọi lại StartPlayerTurn() để lặp lại vòng chiến đấu.
        // StartPlayerTurn(); 
    }
}