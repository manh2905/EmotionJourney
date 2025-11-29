using UnityEngine;

[CreateAssetMenu(fileName = "NewMonsterData", menuName = "Monster System/Monster Data")]
public class MonsterData : ScriptableObject
{
    public string monsterName = "Evil Emotion";
    
    [Header("Stats")]
    public int maxHP = 50;
    public float damage = 5f;
    public int mana = 3; // Mana cần để dùng Special Attack
    public float specialAttackDamage = 10f;
    
    [Header("Animation")]
    public float deathAnimationTime = 2f;
}