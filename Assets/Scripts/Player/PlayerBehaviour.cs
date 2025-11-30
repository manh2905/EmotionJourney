using System.Collections;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public PlayerData data;           // Gắn PlayerData vào đây
    public Animator animator;

    private float currentHp;
    private int currentStamina;

    private float dodgeChance;        // giá trị runtime
    private bool isDead = false;

    private void Start()
    {
        animator = GetComponent<Animator>();

        // KHỞI TẠO CHỈ SỐ DỰA TRÊN PLAYERDATA
        currentHp = data.maxHP;
        currentStamina = data.maxStamina;
        dodgeChance = data.defaultDodgeChance;
    }

    public float GetCurrentHP() => currentHp;
    public float GetCurrentStamina() => currentStamina;

    // ============================
    // 1. NHẬN DAMAGE
    // ============================
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        // Kiểm tra né đòn
        if (TryDodge())
        {
            Debug.Log("<color=yellow>Player né đòn thành công!</color>");
            StartCoroutine(delayAni("dodgeTrigger", data.deathAnimationTime));
            return;
        }

        // Nhận damage thật sự
        currentHp -= damage;
        Debug.Log($"Player nhận {damage} sát thương. HP còn lại: {currentHp}");

        StartCoroutine(delayAni("hitTrigger", data.deathAnimationTime));

        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }
    }

    // ============================
    // 2. NÉ ĐÒN
    // ============================
    private bool TryDodge()
    {
        if (dodgeChance <= 0) return false;

        float rnd = Random.Range(0f, 100f);

        if (rnd < dodgeChance)
        {
            dodgeChance = data.defaultDodgeChance; // reset = mặc định
            return true;
        }

        // Nếu thất bại cũng reset
        dodgeChance = data.defaultDodgeChance;
        return false;
    }

    public void SetDodgeChance(float percent)
    {
        dodgeChance = percent;
        Debug.Log($"Player được buff né đòn: {percent}%");
    }

    // ============================
    // 3. CHẾT
    // ============================
    private void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetTrigger("dieTrigger");

        Debug.Log("<color=red>PLAYER DEAD</color>");
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(data.deathAnimationTime);

        // Tùy game design:
        // Destroy(gameObject);
        // SceneManager.LoadScene("GameOver");
    }

    // ============================
    // 4. ATTACK
    // ============================
    public void Attack(float bonusDamage = 0)
    {
        if (isDead) return;

        int randomAttack = Random.Range(1, 4); // 1 – 3

        switch (randomAttack)
        {
            case 1:
                animator.SetTrigger("attack1Trigger");
                break;

            case 2:
                animator.SetTrigger("attack2Trigger");
                break;

            case 3:
                animator.SetTrigger("attack3Trigger");
                break;
        }

        float totalDamage = data.baseDamage + bonusDamage;

        Debug.Log($"Player Attack {randomAttack} gây {totalDamage} sát thương");

        // BattleManager sẽ xử lý Monster.TakeDamage(totalDamage)
    }


    // ============================
    // 5. STAMINA
    // ============================
    public void ResetStamina()
    {
        currentStamina = data.maxStamina;
    }

    public bool SpendStamina(int amount)
    {
        if (currentStamina < amount) return false;

        currentStamina -= amount;
        return true;
    }

    // ============================
    // 6. HEAL
    // ============================
    public void Heal(float amount)
    {
        if (isDead) return;

        currentHp += amount;
        currentHp = Mathf.Clamp(currentHp, 0, data.maxHP);

        Debug.Log($"Player hồi {amount} HP. HP hiện tại: {currentHp}");
    }

    private IEnumerator delayAni(string trigger, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        animator.SetTrigger(trigger);
        
        
    }

    
}
