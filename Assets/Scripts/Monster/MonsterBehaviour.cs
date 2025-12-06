using System.Collections;
using UnityEngine;


public class MonsterBehaviour : MonoBehaviour
{
    public MonsterData data;
    public Animator animator;
    public float currentHP;
    public int currentMana;
    public int turnCounter;
    public bool haveSpecialATK = false;

    private void Start()
    {
        currentHP = data.maxHP;
        currentMana = 0;
        turnCounter = 0;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;
        Debug.Log($"<color=red>HP của quái: {currentHP}</color>");


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

    // ============ PUBLIC GETTERS FOR UI SYSTEM ============
    
    /// <summary>
    /// Lấy HP hiện tại của Monster (cho UI)
    /// </summary>
    public float GetCurrentHP()
    {
        return currentHP;
    }

    /// <summary>
    /// Lấy Max HP của Monster (cho UI)
    /// </summary>
    public float GetMaxHP()
    {
        return data != null ? data.maxHP : 0;
    }

    /// <summary>
    /// Lấy tên Monster (cho UI)
    /// </summary>
    public string monsterName
    {
        get { return data != null ? data.monsterName : "Unknown Monster"; }
    }

    // ========================================================

    /// <summary>
    /// Lấy Mana hiện tại của Monster (cho UI)
    /// </summary>
    public int GetCurrentMana()
    {
        return currentMana;
    }

    /// <summary>
    /// Lấy Max Mana của Monster (cho UI)
    /// </summary>
    public int GetMaxMana()
    {
        return data != null ? data.mana : 0;
    }


    // Hàm tấn công chính
    public float  Attack()
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
        if (haveSpecialATK)
        {
            animator.SetTrigger("specialTrigger");
        }
        else
        {
            animator.SetTrigger("attackTrigger");
        }
        

        Debug.Log($"{data.monsterName} tung đòn đặc biệt!!!");
        

        currentMana = 0; // reset mana
        return data.specialAttackDamage;
    }
}
