using System.Collections.Generic;
using UnityEngine;

// Class tĩnh, chỉ chứa hàm tính toán
public static class CardDamageCalculator
{
    // Tham số cần thiết: Danh sách bài đã chọn và trạng thái Emometer
    public static float CalculateTotalDamage(List<CardData> cards, EmometerSystem emometer)
    {
        float baseDamage = 0f;
        float damageMultiplier = 1.0f;
        float angerDamage = 0f; 
        bool hasSadness = false; // Cờ theo dõi nếu lá Buồn bã (Bored) được dùng để áp dụng Debuff -15% Damage

        // 1. Tính Damage gốc và xác định các lá bài Buff/Debuff
        foreach (CardData card in cards)
        {
            baseDamage += card.damageValue;
            
            if (card.emotionType == EmotionType.Angry)
            {
                // Lưu lại sát thương gốc của lá Giận dữ để Buff riêng
                angerDamage += card.damageValue;
            }
            if (card.emotionType == EmotionType.Bored)
            {
                hasSadness = true;
            }
        }
        
        // 2. Áp dụng Buff (Giận dữ: +50% sát thương chỉ số trên lá Giận dữ)
        if (angerDamage > 0)
        {
            baseDamage += angerDamage * 0.5f;
            Debug.Log($"Damage Buff Giận dữ (+{angerDamage * 0.5f}) đã được áp dụng.");
        }

        // 3. Áp dụng Debuff (Buồn bã: Giảm 15% sát thương tổng)
        if (hasSadness)
        {
            damageMultiplier *= (1f - 0.15f); // Damage giảm 15%
            Debug.Log("Debuff Buồn bã (-15%) đã được áp dụng.");
        }
        
        // 4. Áp dụng Burnout Debuff (Tất cả Dmg từ các lá bài cảm xúc giảm 50%)
        if (emometer.isBurnedOut)
        {
            damageMultiplier *= 0.5f;
            Debug.Log("CẢNH BÁO: Burnout! Damage giảm 50%.");
        }

        float totalDamage = baseDamage * damageMultiplier;

        return totalDamage;
    }
}