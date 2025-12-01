using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Stats")]
    public string playerName = "Hero";
    public float maxHP = 100;
    public float baseDamage = 10;

    [Header("Stamina Settings")]
    public int maxStamina = 5;

    [Header("Dodge Settings")]
    public float defaultDodgeChance = 0f;

    [Header("Emotion Meter")]
    public int emotionMin = -10;
    public int emotionMax = 10;
    public int emotionSafeMin = -5;
    public int emotionSafeMax = 5;

    //[Header("Level / Progression")]
    //public int level = 1;
    //public int exp = 0;
    //public int expToNextLevel = 100;

    //[Header("Visual")]
    //public Sprite avatar;          // icon nhân vật
    //public RuntimeAnimatorController animatorController; // Animator của player

    [Header("Misc")]
    public float delayAnimationTime = 0.3f;
    public float deathAnimationTime = 0.8f;
}
