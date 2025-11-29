using System.Collections.Generic;
using UnityEngine;

// Class tĩnh, chỉ chứa hàm thực thi hiệu ứng
public static class CardEffectExecutor
{
    // Bổ sung PlayerStats để xử lý HP/Dodge. 
    // Tham số này phải khớp với tham số trong BattleManager.ResolveCards()
    public static void ExecuteEffects(List<CardData> cards, EmometerSystem emometer, StaminaSystem stamina, PlayerStats playerStats)
    {
        int cardsProcessed = 0;
        
        // Cần lưu trạng thái Sợ Hãi để xử lý rủi ro 2 lá sau
        bool fearActive = false;
        
        // Bắt đầu thực thi từng lá bài theo thứ tự (từ trái sang phải)
        foreach (CardData card in cards)
        {
            Debug.Log($"Thực thi lá bài: {card.cardName} (Vị trí: {cardsProcessed + 1})");
            
            // --- XỬ LÝ RỦI RO SỢ HÃI (Trang 8 GDD) ---
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
            
            // --- BƯỚC 1: Dịch chuyển Emometer (luôn áp dụng) ---
            emometer.ShiftEmotion(card.emotionValue);

            bool isPositiveCard = card.emotionType == EmotionType.Funny || card.emotionType == EmotionType.Happy;
            
            // --- KIỂM TRA BURNOUT (Trang 8 GDD) ---
            if (emometer.isBurnedOut)
            {
                if (emometer.isPositiveBurnout && isPositiveCard)
                {
                    // Burn out Tích cực: dùng lá tích cực sẽ không được nhận buff (Heal, Dodge, v.v.)
                    Debug.Log($"Burnout Tích cực: Buff/Heal của {card.cardName} bị vô hiệu hóa.");
                }
                else if (!emometer.isPositiveBurnout && !isPositiveCard)
                {
                    // Burn out Tiêu cực: nhận x2 debuff (chỉ là Debug trong logic này)
                    Debug.Log($"Burnout Tiêu cực: Debuff của {card.cardName} bị nhân đôi.");
                }
            }
            
            // --- BƯỚC 2: THỰC THI HIỆU ỨNG CỤ THỂ ---
            switch (card.emotionType)
            {
                case EmotionType.Funny: // Vui vẻ: Heal 10% máu
                    if (!emometer.isBurnedOut || !emometer.isPositiveBurnout)
                    {
                        playerStats.Heal(playerStats.maxHp * 0.10f);
                    }
                    break;
                case EmotionType.Angry: // Giận dữ: Mất 5% máu
                    // Mất máu (rủi ro) luôn xảy ra
                    playerStats.TakeDamage(playerStats.maxHp * 0.05f);
                    break;
                case EmotionType.Scared: // Sợ hãi: +50% Né đòn
                    if (!emometer.isBurnedOut || !emometer.isPositiveBurnout)
                    {
                        playerStats.dodgeChance = 50f; 
                    }
                    fearActive = true; // Kích hoạt rủi ro cho lá tiếp theo
                    break;
                case EmotionType.Happy: // Hạnh phúc
                    // 0 stamina (Đã xử lý ở Draft)
                    break;
                case EmotionType.Bored: // Buồn bã
                    // Debuff -15% sát thương (Đã được xử lý trong Damage Calculator)
                    break;
            }
            
            cardsProcessed++;
        }
    }
}