using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý tương tác bài trong Battle (Hand <-> Slots)
/// </summary>
public class BattleCardManager : MonoBehaviour
{
    public static BattleCardManager Instance { get; private set; }

    [Header("Zones")]
    public RectTransform handZone;
    public List<CardSlot> cardSlots;


    [Header("Setup")]
    public GameObject cardPrefab;
    public CardDatabase cardDatabase;
    
    private const int MAX_HAND_SIZE = 7;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        RefillHand();
    }

    public void RefillHand()
    {
        if (cardPrefab == null || handZone == null || cardDatabase == null) return;

        int currentCards = handZone.childCount; // Assuming only cards are children
        // Note: If you have placeholders or other objects, you might need a more robust count.
        // For now, we assume direct children are cards.

        int cardsNeeded = MAX_HAND_SIZE - currentCards;

        for (int i = 0; i < cardsNeeded; i++)
        {
            var cardEntry = cardDatabase.GetRandomCard();
            if (cardEntry.data != null)
            {
                SpawnCard(cardEntry);
            }
        }
    }

    private void SpawnCard(CardDatabase.CardEntry entry)
    {
        GameObject cardObj = Instantiate(cardPrefab, handZone);
        CardUI cardUI = cardObj.GetComponent<CardUI>();
        if (cardUI != null)
        {
            cardUI.Initialize(entry.data, entry.art);
        }
    }

    /// <summary>
    /// Xử lý khi click vào một lá bài
    /// </summary>
    public void OnCardClicked(CardUI card)
    {
        if (card.isInSlot)
        {
            ReturnCardToHand(card);
        }
        else
        {
            TryMoveCardToSlot(card);
        }
    }

    private void TryMoveCardToSlot(CardUI card)
    {
        // Tìm slot trống đầu tiên
        CardSlot emptySlot = null;
        foreach (var slot in cardSlots)
        {
            if (slot.IsEmpty)
            {
                emptySlot = slot;
                break;
            }
        }

        if (emptySlot != null)
        {
            // Move to slot
            emptySlot.AssignCard(card);
            card.MoveToSlot(emptySlot);
        }
        else
        {
            Debug.Log("No empty slots available!");
        }
    }

    private void ReturnCardToHand(CardUI card)
    {
        // Tìm slot hiện tại của card để clear
        foreach (var slot in cardSlots)
        {
            if (slot.currentCard == card)
            {
                slot.ClearCard();
                break;
            }
        }

        // Move back to hand
        card.ReturnToHand(handZone);
    }
}
