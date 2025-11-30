using UnityEngine;

// Đảm bảo kế thừa từ MonoBehaviour để gắn vào Game Object
public class StaminaSystem : MonoBehaviour
{
    public int maxStamina = 5; // Ví dụ: Max Stamina là 5
    private int currentStamina;

    void Start()
    {
        currentStamina = maxStamina;
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
        // Logic cập nhật UI Stamina sẽ được thêm vào sau
        Debug.Log($"Đã tiêu hao {cost} Stamina. Hiện tại: {currentStamina}/{maxStamina}");
    }

    // Hồi lại Stamina (Reset per turn)
    public void ResetStamina()
    {
        currentStamina = maxStamina;
        Debug.Log("Stamina đã hồi đầy.");
        // Logic cập nhật UI Stamina sẽ được thêm vào sau
    }

    // Thêm Stamina (for testing purposes)
    public void AddStamina(int amount)
    {
        currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
        Debug.Log($"Đã thêm {amount} Stamina. Hiện tại: {currentStamina}/{maxStamina}");
        // Logic cập nhật UI Stamina sẽ được thêm vào sau
    }
}