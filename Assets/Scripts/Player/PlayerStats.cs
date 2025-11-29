using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float maxHp = 100;
    private float currentHp;
    
    // Khả năng né đòn (Dodge Chance) cho lá Sợ Hãi
    [HideInInspector]
    public float dodgeChance = 0f; 

    void Start()
    {
        currentHp = maxHp;
    }

    public float GetCurrentHP()
    {
        return currentHp;
    }

    // Hàm nhận sát thương
    public void TakeDamage(float damage)
    {
        // Kiểm tra khả năng né đòn (Dodge)
        if (Random.Range(0f, 100f) < dodgeChance)
        {
            Debug.Log("NGƯỜI CHƠI NÉ ĐÒN THÀNH CÔNG!");
            // Reset dodge chance sau khi kiểm tra
            dodgeChance = 0f; 
            return;
        }

        currentHp -= damage;
        Debug.Log($"Người chơi nhận {damage} sát thương. HP còn lại: {currentHp}");

        if (currentHp <= 0)
        {
            currentHp = 0;
            // Gọi BattleManager.EndBattle(false);
            Debug.Log("GAME OVER!");
        }
        // Reset dodge chance
        dodgeChance = 0f;
    }

    // Hàm hồi máu
    public void Heal(float amount)
    {
        currentHp += amount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        Debug.Log($"Người chơi hồi {amount} HP. HP hiện tại: {currentHp}");
    }
}