using UnityEngine;

public static class MonsterScaler
{
    public static void ScaleMonster(MonsterData data, int level)
    {
        data.maxHP += level * 5;
        data.damage += level * 2;
        data.specialAttackDamage += level * 2;
        data.mana += level / 2;

       
    }
}
