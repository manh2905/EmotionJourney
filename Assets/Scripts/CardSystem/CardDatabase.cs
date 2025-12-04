using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "Card System/Card Database")]
public class CardDatabase : ScriptableObject
{
    [System.Serializable]
    public struct CardEntry
    {
        public string id;
        public CardData data;
        public Sprite art;
    }

    public List<CardEntry> allCards;

    public CardEntry GetRandomCard()
    {
        if (allCards == null || allCards.Count == 0) return default;
        return allCards[Random.Range(0, allCards.Count)];
    }
    public CardEntry GetRandomUnlockedCard()
    {
        List<CardEntry> unlocked = new List<CardEntry>();

        foreach (var entry in allCards)
        {
            if (CardUnlockManager.Instance.unlockedCards.Contains(entry.data))
            {
                unlocked.Add(entry);
            }
        }

        if (unlocked.Count == 0)
        {
            Debug.LogWarning(" Không có card nào được mở khóa!");
            return default;
        }

        return unlocked[Random.Range(0, unlocked.Count)];
    }
    public CardData GetCardByID(string id)
    {
        var entry = allCards.Find(c => c.id == id);
        return entry.data;
    }

}
