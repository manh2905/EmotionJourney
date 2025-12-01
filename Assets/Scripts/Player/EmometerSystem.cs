using UnityEngine;
using UnityEngine.Events;

public class EmometerSystem : MonoBehaviour
{
    private const int MIN_EMO = -10;
    private const int MAX_EMO = 10;
    // Mức an toàn được đề cập trong GDD là -5 đến +5, nhưng logic Burnout chỉ kích hoạt tại -10/+10.

    private int currentEmotion = 0; // Mức cân bằng là 0
    public bool isBurnedOut = false;
    public bool isPositiveBurnout = false;
    
    public int CurrentEmotion => currentEmotion; // Getter công khai cho UI và logic khác
    public bool IsInSafetyZone => currentEmotion > -5 && currentEmotion < 5; // Dữ liệu tham khảo

    // ============================
    // EVENTS - Để notify UI khi Emotion thay đổi
    // ============================
    [System.Serializable]
    public class EmotionChangedEvent : UnityEvent<int, bool, bool> { } // (currentEmotion, isBurnedOut, isPositiveBurnout)
    
    public EmotionChangedEvent OnEmotionChanged = new EmotionChangedEvent();

    public void Initialize()
    {
        currentEmotion = 0;
        isBurnedOut = false;
        isPositiveBurnout = false;
        Debug.Log("Emometer đã được khởi tạo: 0.");
        
        // Trigger initial event
        OnEmotionChanged?.Invoke(currentEmotion, isBurnedOut, isPositiveBurnout);
    }

    // Thay đổi cảm xúc mỗi khi dùng bài (được gọi từ CardEffectExecutor)
    public void ShiftEmotion(int value)
    {
        currentEmotion += value;
        // Giới hạn thanh cảm xúc từ -10 đến +10
        currentEmotion = Mathf.Clamp(currentEmotion, MIN_EMO, MAX_EMO); 

        CheckBurnoutStatus();
        Debug.Log($"Emotion Shift: {currentEmotion}. Burnout Active: {isBurnedOut}");
        
        // Trigger event để update UI
        OnEmotionChanged?.Invoke(currentEmotion, isBurnedOut, isPositiveBurnout);
    }

    // Kiểm tra và kích hoạt/thoát khỏi Burnout
    private void CheckBurnoutStatus()
    {
        if (currentEmotion >= MAX_EMO || currentEmotion <= MIN_EMO)
        {
            if (!isBurnedOut)
            {
                isBurnedOut = true;
                // Nếu >= 10, là Burnout Tích cực
                isPositiveBurnout = (currentEmotion >= MAX_EMO); 
                Debug.Log($"!!! BURNOUT KÍCH HOẠT !!! Loại: {(isPositiveBurnout ? "Tích cực (+10)" : "Tiêu cực (-10)")}");
            }
        }
        else if (isBurnedOut)
        {
            // Thoát khỏi Burnout khi cảm xúc nằm giữa -10 và +10
            isBurnedOut = false;
            isPositiveBurnout = false;
            Debug.Log("Burnout kết thúc. Quay về Cân bằng.");
        }
    }
}