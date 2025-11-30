using UnityEngine;

public class MonsterTestController : MonoBehaviour
{
    public MonsterBehaviour monster;      // Kéo Monster từ Scene vào đây
    public PlayerBehaviour player;        // Kéo Player từ Scene vào đây

    private void Update()
    {
        // ============================
        // 1. MONSTER ATTACK PLAYER (A)
        // ============================
        if (Input.GetKeyDown(KeyCode.A))
        {
            float dmg = monster.Attack();

            Debug.Log("<color=red>[MONSTER ATTACK]</color>");
            Debug.Log($"Monster gây {dmg} damage");

            player.TakeDamage(dmg);

            Debug.Log($"Player HP: {player.GetCurrentHP()}");
        }

        // ============================
        // 2. PLAYER ATTACK MONSTER (J)
        // ============================
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("<color=green>[PLAYER ATTACK]</color>");

            // Player chọn 1 animation attack ngẫu nhiên
            player.Attack();

            // Damage cơ bản (có thể mở rộng với card bonus)
            float dmg = player.data.baseDamage;

            monster.TakeDamage((float)dmg);

            Debug.Log($"Player gây {dmg} damage");
            Debug.Log($"Monster HP: {monster.currentHP}");
        }

        // ============================
        // 3. PLAYER BUFF NÉ ĐÒN (K)
        // ============================
        if (Input.GetKeyDown(KeyCode.K))
        {
            player.SetDodgeChance(100f); // buff né 50%
            Debug.Log("<color=yellow>Player được buff 50% né đòn!</color>");
        }

        // ============================
        // 4. MONSTER BỊ ĐÁNH TEST (H)
        // ============================
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    monster.TakeDamage(10);
        //    Debug.Log($"Monster bị đánh 10 damage | HP: {monster.currentHP}");
        //}
    }
}
