using System.Collections.Generic;
using UnityEngine;

public class DraftManager : MonoBehaviour
{
    [Header("Dependencies")]
    // Tham chiếu sẽ được gán trong Inspector trên Game Manager
    public DeckSystem deckSystem;
    public StaminaSystem staminaSystem;
    public BattleManager battleManager; 

    // Danh sách 3 lá bài đã được người chơi chọn
    [HideInInspector] // Ẩn khỏi Inspector vì nó chỉ là biến tạm thời trong lượt
    public List<CardData> selectedCards = new List<CardData>();
    private const int MAX_SELECTION = 3; 

    // Hàm được gọi khi bắt đầu một lượt Draft mới
    public void StartDraftPhase()
    {
        selectedCards.Clear();
        // Giả định rằng DeckSystem.RevealAndRefillHand() đã được gọi trước đó bởi BattleManager
    }

    /// <summary>
    /// Phương thức chính được gọi khi người chơi nhấp vào một lá bài (hoặc gọi qua UI)
    /// Được gọi từ BattleCardManager.OnConfirmButtonClicked()
    /// </summary>
    public bool TrySelectCard(CardData cardToSelect)
    {
        // 1. Kiểm tra giới hạn 3 lá
        if (selectedCards.Count >= MAX_SELECTION)
        {
            Debug.Log("Lỗi: Đã chọn đủ 3 lá bài.");
            return false;
        }
        
        // Note: Không cần check duplicate vì BattleCardManager đảm bảo mỗi slot chỉ có 1 card

        // 2. TÍCH HỢP STAMINA: Kiểm tra chi phí
        if (!staminaSystem.CanUseCard(cardToSelect.staminaCost))
        {
            Debug.Log($"❌ Không đủ Stamina! Hiện tại: {staminaSystem.GetCurrentStamina()}, Cần: {cardToSelect.staminaCost} cho {cardToSelect.cardName}");
            return false;
        }

        // 3. Tiến hành chọn bài và tiêu hao Stamina
        selectedCards.Add(cardToSelect);
        staminaSystem.ConsumeStamina(cardToSelect.staminaCost); 

        Debug.Log($"✅ Đã chọn {cardToSelect.cardName} (Cost: {cardToSelect.staminaCost}). Tổng: {selectedCards.Count}/{MAX_SELECTION}. Stamina còn: {staminaSystem.GetCurrentStamina()}");
        return true;
    }
    
    /// <summary>
    /// Hoàn lại stamina khi player return card về hand
    /// Được gọi từ BattleCardManager.ReturnCardToHand()
    /// </summary>
    public void RefundCardStamina(CardData card)
    {
        if (selectedCards.Contains(card))
        {
            selectedCards.Remove(card);
            staminaSystem.RefundStamina(card.staminaCost);
            Debug.Log($"♻️ Hoàn lại {card.staminaCost} stamina cho {card.cardName}. Stamina hiện tại: {staminaSystem.GetCurrentStamina()}");
        }
    }
    
    /// <summary>
    /// Phương thức được gọi khi người chơi nhấn nút "Xác nhận/Confirm"
    /// Được gọi từ BattleCardManager.OnConfirmButtonClicked()
    /// </summary>
    public void ConfirmDraft()
    {
        if (selectedCards.Count == 0)
        {
            Debug.LogWarning("⚠️ Không có card nào được chọn!");
            return;
        }
        
        Debug.Log($"🎯 Xác nhận Draft với {selectedCards.Count} lá bài. Chuyển sang Resolve Phase...");

        // Chuyển quyền điều khiển lại cho Battle Manager để xử lý Resolve
        battleManager.ProcessPlayerActions(selectedCards);

        // Clear danh sách để chuẩn bị cho lượt sau
        selectedCards.Clear();
    }
}