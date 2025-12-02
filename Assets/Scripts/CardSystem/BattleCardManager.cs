using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Quản lý tương tác bài trong Battle (Hand <-> Slots)
/// Kết nối UI với DraftManager để xử lý logic
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
    
    [Header("Logic Connection")]
    public DraftManager draftManager;
    public UnityEngine.UI.Button confirmButton;
    
    private const int MAX_HAND_SIZE = 7;
    private const int MAX_DRAFT_SLOTS = 3;

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
        ValidateSlots(); // Ensure slots are valid first

        foreach (var slot in cardSlots)
        {
            if (slot != null)
            {
                slot.ClearCard();
            }
        }

        RefillHand();

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            UpdateConfirmButton();
        }
    }

    private void ValidateSlots()
    {
        if (cardSlots == null) cardSlots = new List<CardSlot>();
        cardSlots.RemoveAll(s => s == null);
        
        if (cardSlots.Count == 0)
        {
            var found = GetComponentsInChildren<CardSlot>();
            if (found.Length > 0) cardSlots.AddRange(found);
        }
    }

    public void RefillHand()
    {
        if (cardPrefab == null || handZone == null || cardDatabase == null)
        {
            Debug.LogError("❌ Missing references! CardPrefab, HandZone, or CardDatabase is NULL!");
            return;
        }

        int currentCards = handZone.childCount;
        int cardsNeeded = MAX_HAND_SIZE - currentCards;

        Debug.Log($"📦 Spawning {cardsNeeded} cards...");

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
            Debug.Log($"🎴 Spawned card: {entry.data.cardName}, Sprite: {(entry.art != null ? entry.art.name : "NULL")}");
        }
        else
        {
            Debug.LogError("❌ CardUI component not found on spawned card!");
        }
    }

    public void OnCardClicked(CardUI card)
    {
        if (IsCardInAnySlot(card))
        {
            ReturnCardToHand(card);
        }
        else
        {
            TryMoveCardToSlot(card);
        }
        
        UpdateConfirmButton();
    }

    private void TryMoveCardToSlot(CardUI card)
    {
        if (cardSlots == null || cardSlots.Count == 0) return;

        ClearCardFromAllSlots(card);

        CardSlot emptySlot = null;
        foreach (var slot in cardSlots)
        {
            if (slot != null && slot.IsEmpty)
            {
                emptySlot = slot;
                break;
            }
        }

        if (emptySlot != null)
        {
            emptySlot.AssignCard(card);
            card.MoveToSlot(emptySlot);
        }
        else
        {
            Debug.Log("⚠️ No empty slots available!");
        }
    }

    private void ReturnCardToHand(CardUI card)
    {
        //if (cardSlots != null)
        //{
        //    foreach (var slot in cardSlots)
        //    {
        //        if (slot != null && slot.currentCard == card)
        //        {
        //            slot.ClearCard();
        //            break;
        //        }
        //    }
        //}
        ClearCardFromAllSlots(card);
        if (draftManager != null && card.cardData != null)
        {
            draftManager.RefundCardStamina(card.cardData);
        }
        
        card.ReturnToHand(handZone);
        Debug.Log($"⬅️ Card returned to hand");
    }
    
    private int GetCardsInSlotsCount()
    {
        int count = 0;
        
        if (cardSlots != null)
        {
            foreach (var slot in cardSlots)
            {
                if (slot != null && !slot.IsEmpty)
                {
                    count++;
                }
            }
        }
        
        return count;
    }
    
    private void UpdateConfirmButton()
    {
        if (confirmButton != null)
        {
            int cardsInSlots = GetCardsInSlotsCount();
            confirmButton.interactable = cardsInSlots > 0;
            
            var buttonText = confirmButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = $"Confirm ({cardsInSlots}/{MAX_DRAFT_SLOTS})";
            }
            
            Debug.Log($"🔘 Button: Cards={cardsInSlots}, Interactable={confirmButton.interactable}");
        }
    }
    
    private void OnConfirmButtonClicked()
    {
        Debug.Log("🎯 CONFIRM BUTTON CLICKED!");
        
        List<CardData> selectedCards = new List<CardData>();
        
        if (cardSlots != null)
        {
            foreach (var slot in cardSlots)
            {
                if (slot != null && !slot.IsEmpty && slot.currentCard != null)
                {
                    selectedCards.Add(slot.currentCard.cardData);
                }
            }
        }
        
        if (selectedCards.Count == 0)
        {
            Debug.LogWarning("⚠️ Không có card nào được chọn!");
            return;
        }
        
        if (draftManager != null)
        {
            bool allValid = true;
            List<CardData> validatedCards = new List<CardData>();
            
            foreach (var cardData in selectedCards)
            {
                if (draftManager.TrySelectCard(cardData))
                {
                    validatedCards.Add(cardData);
                }
                else
                {
                    allValid = false;
                    break;
                }
            }
            
            if (allValid && validatedCards.Count > 0)
            {
                draftManager.ConfirmDraft();
                ClearAllSlots();
            }
            else
            {
                Debug.LogWarning("❌ Validation thất bại!");
            }
        }
        else
        {
            Debug.LogError("🚨 DraftManager chưa được gán!");
        }
    }

    private bool IsCardInAnySlot(CardUI card)
    {
        if (cardSlots == null) return false;

        foreach (var slot in cardSlots)
        {
            if (slot != null && slot.currentCard == card)
            {
                return true;
            }
        }

        return false;
    }

    private void ClearCardFromAllSlots(CardUI card)
    {
        if (cardSlots == null) return;

        foreach (var slot in cardSlots)
        {
            if (slot != null && slot.currentCard == card)
            {
                slot.ClearCard();
            }
        }

        card.isInSlot = false;
    }

    private void ClearAllSlots()
    {
        if (cardSlots != null)
        {
            foreach (var slot in cardSlots)
            {
                if (slot != null && !slot.IsEmpty)
                {
                    if (slot.currentCard != null)
                    {
                        Destroy(slot.currentCard.gameObject);
                    }
                    slot.ClearCard();
                }
            }
        }
        
        UpdateConfirmButton();
    }
}