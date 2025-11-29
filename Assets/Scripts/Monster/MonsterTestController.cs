using UnityEngine;

public class MonsterTestController : MonoBehaviour
{
    public MonsterBehaviour monster;

    private void Update()
    {
        // Nhấn A = Monster tấn công
        if (Input.GetKeyDown(KeyCode.A))
        {
            int dmg = monster.Attack();
            Debug.Log($"Monster Attack Damage: {dmg}");
        }

        // Nhấn H = Làm Monster bị đánh 10 damage
        if (Input.GetKeyDown(KeyCode.H))
        {
            monster.TakeDamage(10);
            Debug.Log($"Monster take 10 dmg | Current HP: {monster.currentHP}");
        }
    }
}
