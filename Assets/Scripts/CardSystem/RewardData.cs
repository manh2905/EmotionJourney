using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardData", menuName = "Card System/Reward Per Level")]
public class RewardData : ScriptableObject
{
    public int level;                          // Màn số mấy
    public List<CardData> rewardCards;         // 5 lá bài mở khóa
}
