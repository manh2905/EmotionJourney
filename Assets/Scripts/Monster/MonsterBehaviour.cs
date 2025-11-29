using System.Collections;
using UnityEngine;


public class MonsterBehaviour : MonoBehaviour
{
    public MonsterData data;
    public Animator animator;
    public int currentHP;
    public int currentMana;
    public int turnCounter;

    private void Start()
    {
        currentHP = data.maxHP;
        currentMana = 0;
        turnCounter = 0;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;

        animator.SetTrigger("hitTrigger");

        if (IsDead())
        {
            StartCoroutine(DieRoutine());
        }
    }

    private IEnumerator DieRoutine()
    {
        animator.SetTrigger("dieTrigger");
        yield return new WaitForSeconds(data.deathAnimationTime); // chờ animation xong
        Destroy(gameObject);
    }

    public bool IsDead()
    {
        return currentHP <= 0;
    }

    // Hàm tấn công chính
    public float Attack()
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
    private float NormalAttack()
    {
        currentMana += 1; // +1 mana mỗi lần đánh

        animator.SetTrigger("attackTrigger");

        Debug.Log($"{data.monsterName} đánh thường! (+1 mana: {currentMana}/{data.mana})");

        return data.damage;
    }

    // Đòn đánh đặc biệt
    private float SpecialAttack()
    {
        animator.SetTrigger("attackTrigger");
        animator.SetTrigger("specialTrigger");

        Debug.Log($"{data.monsterName} tung đòn đặc biệt!!!");
        

        currentMana = 0; // reset mana
        return data.specialAttackDamage;
    }
}
