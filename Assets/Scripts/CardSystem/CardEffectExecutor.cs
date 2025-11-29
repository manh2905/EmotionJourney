using System.Collections.Generic;
using UnityEngine;

// Class tĩnh, chỉ chứa hàm thực thi hiệu ứng
public static class CardEffectExecutor
{
    // Cần truyền các hệ thống ảnh hưởng
    // Note: Cần thêm PlayerStats để xử lý HP/Dodge. Ta dùng Debug.Log tạm thời.
    public static void ExecuteEffects(List<CardData> cards, EmometerSystem emometer, StaminaSystem stamina)
    {
        int cardsProcessed = 0;
        
        // Cần lưu trạng thái Sợ Hãi để xử lý rủi ro 2 lá sau
        bool fearActive = false;
        
        // Bắt đầu thực thi từng lá bài theo thứ tự
        foreach (CardData card in cards)
        {
            Debug.Log($"Thực thi lá bài: {card.cardName} (Vị trí: {cardsProcessed + 1})");
            
            // --- XỬ LÝ RỦI RO SỢ HÃI ---
            if (fearActive && cardsProcessed > 0)
            {
                // Nếu lá trước là Sợ Hãi, có 50% lá hiện tại bị lỗi
                if (Random.value < 0.5f) // 50% cơ hội lỗi
                {
                    Debug.Log($"Rủi ro Sợ Hãi kích hoạt! Lá {card.cardName} (Vị trí {cardsProcessed + 1}) không thi triển.");
                    cardsProcessed++;
                    continue; // Bỏ qua lá bài này
                }
            }
            fearActive = false; // Reset sau khi kiểm tra
            
            // --- KIỂM TRA BURNOUT VÀ ÁP DỤNG HIỆU ỨNG CƠ BẢN ---
            
            // 1. Dịch chuyển Emometer (luôn áp dụng)
            emometer.ShiftEmotion(card.emotionValue);

            // 2. Áp dụng các hiệu ứng không liên quan đến Damage
            // Giả định Player có một class PlayerStats để quản lý HP, Dodge, v.v.
            
            bool isPositiveCard = card.emotionType == EmotionType.Funny || card.emotionType == EmotionType.Happy;
            
            if (emometer.isBurnedOut)
            {
                if (emometer.isPositiveBurnout && isPositiveCard)
                {
                    // Burn out Tích cực: dùng lá tích cực sẽ không được nhận buff (Heal, Dodge, v.v.)
                    Debug.Log($"Burnout Tích cực: Buff/Heal của {card.cardName} bị vô hiệu hóa.");
                }
                else if (!emometer.isPositiveBurnout && !isPositiveCard)
                {
                    // Burn out Tiêu cực: nhận x2 debuff (chỉ áp dụng cho debuff nếu có, ở đây ta dùng Debug)
                    Debug.Log($"Burnout Tiêu cực: Debuff của {card.cardName} bị nhân đôi.");
                }
                // Damage giảm 50% đã được xử lý trong CardDamageCalculator
            }
            
            // --- THỰC THI HIỆU ỨNG CỤ THỂ ---
            switch (card.emotionType)
            {
                case EmotionType.Funny: // Vui vẻ
                    // Heal 10% máu tối đa nhân vật
                    Debug.Log("Hiệu ứng Vui vẻ: [PLACEHOLDER] Tự hồi 10% HP.");
                    break;
                case EmotionType.Angry: // Giận dữ
                    // Mất 5% máu khi thi triển
                    Debug.Log("Hiệu ứng Giận dữ: [PLACEHOLDER] Tự mất 5% HP.");
                    break;
                case EmotionType.Scared: // Sợ hãi
                    // +50% khả năng né đòn của nhân vật
                    Debug.Log("Hiệu ứng Sợ hãi: [PLACEHOLDER] +50% Né đòn trong lượt này.");
                    fearActive = true; // Kích hoạt rủi ro cho lá tiếp theo
                    break;
                case EmotionType.Happy: // Hạnh phúc
                    // Không mất stamina (Đã được xử lý trong DraftManager, không cần làm lại ở đây)
                    break;
                case EmotionType.Bored: // Buồn bã
                    // Debuff -15% sát thương (Đã được xử lý trong Damage Calculator)
                    break;
            }
            
            cardsProcessed++;
        }
    }
}