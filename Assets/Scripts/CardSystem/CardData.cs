using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "Card System/Card Data")]
public class CardData : ScriptableObject
{
    // 1. Dữ liệu Cơ bản
    public string cardName;

    // 2. Dữ liệu Cảm xúc (Emotion)
    // Các cảm xúc chính: Vui vẻ, Buồn bã, Sợ hãi, Hạnh phúc, Giận dữ [cite: 120]
    public EmotionType emotionType; // Enum: Positive, Negative, or a specific type (e.g., Joy, Anger)
    public int emotionValue; // Chỉ số Emometer (thay đổi Thanh Cảm xúc) [cite: 131]

    // 3. Dữ liệu Chiến đấu (Combat)
    public int damageValue; // Chỉ số Sát thương cơ bản [cite: 132]

    // 4. Chi phí Stamina
    public int staminaCost; // Stamina yêu cầu [cite: 93, 99]

    // Cần thêm các trường khác cho hiệu ứng đặc biệt nếu cần
    // public CardEffectType effectType; 

}

// Enum để phân loại cảm xúc
public enum EmotionType { Funny, Bored, Scared, Happy, Angry } 
// Nên đặt Enum này ở file riêng (ví dụ: Core/Enums.cs)