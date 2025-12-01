using UnityEngine;
using UnityEngine.Events;

// Đảm bảo kế thừa từ MonoBehaviour để gắn vào Game Object
public class StaminaSystem : MonoBehaviour
{
    public int maxStamina = 5; // Ví dụ: Max Stamina là 5
    private int currentStamina;

    // ============================
    // EVENTS - Để notify UI khi Stamina thay đổi
    // ============================
    [System.Serializable]
    public class StaminaChangedEvent : UnityEvent<int, int> { } // (currentStamina, maxStamina)
    
    public StaminaChangedEvent OnStaminaChanged = new StaminaChangedEvent();

    void Start()
    {
        currentStamina = maxStamina;
        // Trigger initial event
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    public int GetCurrentStamina()
    {
        return currentStamina;
    }

    // Kiểm tra xem người chơi có đủ Stamina để sử dụng lá bài không
    public bool CanUseCard(int cost)
    {
        return currentStamina >= cost;
    }

    // Tiêu hao Stamina
    public void ConsumeStamina(int cost)
    {
        currentStamina -= cost;
        Debug.Log($"Đã tiêu hao {cost} Stamina. Hiện tại: {currentStamina}/{maxStamina}");
        
        // Trigger event để update UI
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    // Hồi lại Stamina (Reset per turn)
    public void ResetStamina()
    {
        currentStamina = maxStamina;
        Debug.Log("Stamina đã hồi đầy.");
        
        // Trigger event để update UI
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    // Thêm Stamina (for testing purposes)
    public void AddStamina(int amount)
    {
        currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
        Debug.Log($"Đã thêm {amount} Stamina. Hiện tại: {currentStamina}/{maxStamina}");
        
        // Trigger event để update UI
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }
}