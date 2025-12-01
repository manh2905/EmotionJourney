using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterTestController : MonoBehaviour
{
    public MonsterBehaviour monster;
    public PlayerBehaviour player;

    private void Update()
    {
        // MONSTER ATTACK PLAYER (A)
        if (Input.GetKeyDown(KeyCode.A))
        {
            float dmg = monster.Attack();
            Debug.Log($"Monster gây {dmg} damage");

            player.TakeDamage(dmg);
            Debug.Log($"Player HP: {player.GetCurrentHP()}");

            CheckEndBattle();
        }

        // PLAYER ATTACK MONSTER (J)
        if (Input.GetKeyDown(KeyCode.J))
        {
            player.Attack();

            float dmg = player.data.baseDamage;
            monster.TakeDamage(dmg);

            Debug.Log($"Player gây {dmg} damage");
            Debug.Log($"Monster HP: {monster.currentHP}");

            CheckEndBattle();
        }

        // BUFF DODGE (K)
        if (Input.GetKeyDown(KeyCode.K))
        {
            player.SetDodgeChance(100f);
            Debug.Log("Player buff né 100%");
        }
    }

    // ============================
    // KIỂM TRA KẾT THÚC TRẬN ĐẤU
    // ============================
    private void CheckEndBattle()
    {
        // PLAYER DIE
        if (player.GetCurrentHP() <= 0)
        {
            Debug.Log("<color=red>PLAYER DIE → THUA CUỘC</color>");
            EndBattle(false);
        }

        // MONSTER DIE
        if (monster.IsDead())
        {
            Debug.Log("<color=green>MONSTER DIE → CHIẾN THẮNG</color>");
            EndBattle(true);
        }
    }

    // ============================
    // END BATTLE (WIN / LOSE)
    // ============================
    private void EndBattle(bool playerWon)
    {
        if (playerWon)
        {
            // MỞ KHÓA MÀN TIẾP THEO
            MapController.UnlockNextLevel(BattleLoader.currentLevel);

            Debug.Log("<color=yellow>ĐÃ MỞ KHÓA MÀN TIẾP THEO!</color>");
        }

        // Load lại Map Scene
        Debug.Log("<color=cyan>Quay lại Map...</color>");
        SceneManager.LoadScene("Map");
    }
}
