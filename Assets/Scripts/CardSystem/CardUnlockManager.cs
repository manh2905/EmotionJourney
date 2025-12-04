using System.Collections.Generic;
using UnityEngine;

public class CardUnlockManager : MonoBehaviour
{
    public static CardUnlockManager Instance;

    public List<CardData> unlockedCards = new List<CardData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void UnlockCard(CardData card)
    {
        if (!unlockedCards.Contains(card))
        {
            unlockedCards.Add(card);
            Debug.Log("<color=yellow>[UNLOCK] " + card.cardName + "</color>");
        }
    }

    public void UnlockCards(List<CardData> cards)
    {
        foreach (var c in cards)
            UnlockCard(c);
    }
}
