using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "MonsterData/Monster")]
public class MonsterData : ScriptableObject
{
    [Header("Stats")]
    public string monsterName;
    public int maxHP;
    public int damage;
    public int mana; // Có thể dùng trong skill sau này
    public int specialAttackDamage;

    //[Header("Rewards")]
    //public List<CardData> rewardCards;
}
