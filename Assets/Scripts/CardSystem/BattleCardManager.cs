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
        Debug.Log("🎮 BattleCardManager.Start() called");
        
        RefillHand();
        
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            UpdateConfirmButton();
            Debug.Log("✅ Confirm button setup complete");
        }
        else
        {
            Debug.LogError("❌ Confirm button is NULL!");
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
        Debug.Log("Da vao Spawn");
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
        if (card.isInSlot)
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
        CardSlot emptySlot = null;
        
        foreach (var slot in cardSlots)
        {
            
            if (slot.IsEmpty)
            {
                Debug.Log("Tét");
                emptySlot = slot;
                break;
            }
        }
        
        if (emptySlot != null)
        {
            emptySlot.AssignCard(card);
            card.MoveToSlot(emptySlot);
            Debug.Log($"➡️ Card moved to slot {emptySlot.slotIndex}");
        }
        else
        {
            Debug.Log("📦 Không còn slot trống!");
        }
    }

    private void ReturnCardToHand(CardUI card)
    {
        foreach (var slot in cardSlots)
        {
            if (slot.currentCard == card)
            {
                slot.ClearCard();
                break;
            }
        }
        
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
        foreach (var slot in cardSlots)
        {
            if (!slot.IsEmpty)
            {
                count++;
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
            
            Debug.Log($"🔘 Button state: Cards={cardsInSlots}, Interactable={confirmButton.interactable}");
        }
    }
    
    private void OnConfirmButtonClicked()
    {
        Debug.Log("🎯 CONFIRM BUTTON CLICKED!");
        
        List<CardData> selectedCards = new List<CardData>();
        
        foreach (var slot in cardSlots)
        {
            if (!slot.IsEmpty && slot.currentCard != null)
            {
                selectedCards.Add(slot.currentCard.cardData);
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
                Debug.LogWarning("❌ Validation thất bại! Không đủ stamina hoặc vi phạm game rules.");
            }
        }
        else
        {
            Debug.LogError("🚨 DraftManager chưa được gán trong Inspector!");
        }
    }
    
    private void ClearAllSlots()
    {
        foreach (var slot in cardSlots)
        {
            if (!slot.IsEmpty)
            {
                if (slot.currentCard != null)
                {
                    Destroy(slot.currentCard.gameObject);
                }
                slot.ClearCard();
            }
        }
        
        UpdateConfirmButton();
    }
}