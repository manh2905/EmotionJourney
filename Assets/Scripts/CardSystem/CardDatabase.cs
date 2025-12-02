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
}
