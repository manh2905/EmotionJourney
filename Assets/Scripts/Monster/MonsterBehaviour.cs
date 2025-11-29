using UnityEngine;

public class MonsterBehaviour : MonoBehaviour
{
    public MonsterData data;

    public int currentHP;
    public int currentMana;
    public int turnCounter;

    private void Start()
    {
        currentHP = data.maxHP;
        currentMana = 0;
        turnCounter = 0;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;
    }

    public bool IsDead()
    {
        return currentHP <= 0;
    }

    // Hàm tấn công chính
    public int Attack()
    {
        turnCounter++;

        // Nếu đủ mana -> dùng Skill đặc biệt
        if (currentMana >= data.mana)
        {
            return SpecialAttack();
        }

        // Không đủ mana -> đánh thường
        return NormalAttack();
    }

    // Đánh thường
    private int NormalAttack()
    {
        currentMana += 1; // +1 mana mỗi lần đánh
        Debug.Log($"{data.monsterName} đánh thường! (+1 mana: {currentMana}/{data.mana})");

        return data.damage;
    }

    // Đòn đánh đặc biệt
    private int SpecialAttack()
    {
        Debug.Log($"{data.monsterName} tung đòn đặc biệt!!!");

        currentMana = 0; // reset mana
        return data.specialAttackDamage;
    }
}
