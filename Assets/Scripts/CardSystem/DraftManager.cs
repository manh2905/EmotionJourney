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

    // Phương thức chính được gọi khi người chơi nhấp vào một lá bài (hoặc gọi qua UI)
    public bool TrySelectCard(CardData cardToSelect)
    {
        // 1. Kiểm tra giới hạn 3 lá
        if (selectedCards.Count >= MAX_SELECTION)
        {
            Debug.Log("Lỗi: Đã chọn đủ 3 lá bài. Vui lòng xác nhận.");
            return false;
        }
        
        // Tránh chọn lại lá bài đã có trong danh sách 3 lá (nếu cần)
        if (selectedCards.Contains(cardToSelect))
        {
             // Thường thì nên có hàm Deselect để hủy chọn
             Debug.Log("Lỗi: Lá bài đã được chọn.");
             return false;
        }

        // 2. TÍCH HỢP STAMINA: Kiểm tra chi phí
        if (!staminaSystem.CanUseCard(cardToSelect.staminaCost))
        {
            Debug.Log($"Thất bại: Không đủ Stamina ({staminaSystem.GetCurrentStamina()}) để dùng lá bài {cardToSelect.cardName} (Cost: {cardToSelect.staminaCost}).");
            // Đây là nơi UI cần làm xám (disable) lá bài.
            return false;
        }

        // 3. Tiến hành chọn bài và tiêu hao Stamina
        selectedCards.Add(cardToSelect);
        staminaSystem.ConsumeStamina(cardToSelect.staminaCost); 

        // 4. Cần có logic để hiển thị lá bài đã chọn trên UI (Card Selected Zone)

        Debug.Log($"Đã chọn lá {cardToSelect.cardName}. Đã chọn {selectedCards.Count}/{MAX_SELECTION}.");
        return true;
    }
    
    // Phương thức được gọi khi người chơi nhấn nút "Xác nhận/Confirm"
    public void ConfirmDraft()
    {
        if (selectedCards.Count == 0)
        {
            Debug.Log("Lỗi: Vui lòng chọn ít nhất một lá bài.");
            return;
        }
        
        Debug.Log($"Xác nhận Draft với {selectedCards.Count} lá. Chuyển sang Resolve.");

        // Chuyển quyền điều khiển lại cho Battle Manager để xử lý Resolve
        // Gửi danh sách các lá bài đã chọn để xử lý
        battleManager.ProcessPlayerActions(selectedCards);

        // Sau khi gửi đi, DraftManager tự xóa danh sách để chuẩn bị cho lượt sau
        // selectedCards.Clear() sẽ được gọi khi BattleManager kết thúc resolve/cleanup.
    }
}