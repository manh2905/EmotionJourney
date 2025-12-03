using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ValidateSlots();

        foreach (var slot in cardSlots)
        {
            if (slot != null) slot.ClearCard();
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

    /// <summary>
    /// CHỌN CARD → trừ stamina ngay thông qua DraftManager.TrySelectCard
    /// </summary>
    private void TryMoveCardToSlot(CardUI card)
    {
        Debug.Log("DEBUG >>> TryMoveCardToSlot START");
        Debug.Log("INSTANCE DM: " + draftManager.GetInstanceID());
        Debug.Log("INSTANCE SS: " + (draftManager.staminaSystem != null ? draftManager.staminaSystem.GetInstanceID() : -1));


        if (cardSlots == null || cardSlots.Count == 0)
        {
            Debug.LogError("❌ card == NULL");
            return;
        }

        if (card.cardData == null)
        {
            Debug.LogError("❌ card.cardData == NULL (Prefab chưa có CardData)");
            return;
        }

        if (draftManager == null)
        {
            Debug.LogError("❌ draftManager == NULL");
            return;
        }

        if (draftManager.staminaSystem == null)
        {
            Debug.LogError("❌ draftManager.staminaSystem == NULL !!!");
            return;
        }

        Debug.Log("DEBUG >>> TrySelectCard OK, stamina consumed");
        // Clear slot
        Debug.Log("DEBUG >>> Clearing previous slot");
        ClearCardFromAllSlots(card);

        Debug.Log("DEBUG >>> Finding empty slot");

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
            Debug.Log("DEBUG >>> AssignCard");
            emptySlot.AssignCard(card);

            Debug.Log("DEBUG >>> MoveToSlot");
            card.MoveToSlot(emptySlot);
            draftManager.TrySelectCard(card.cardData);
        }
        else
        {
            Debug.LogWarning("⚠ No Empty Slot");
            draftManager.RefundCardStamina(card.cardData);
            return;
        }
        

        
    }


    // Nếu tới đây là stamina đã trừ và Không crash
    // → crash nằm SAU ĐOẠN NÀY


    /// <summary>
    /// Bỏ chọn card → remove khỏi slot + Refund stamina qua DraftManager
    /// </summary>
    private void ReturnCardToHand(CardUI card)
    {
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

    /// <summary>
    /// CONFIRM: chỉ gọi DraftManager.ConfirmDraft() để gây damage.
    /// KHÔNG gọi lại TrySelectCard nữa (tránh trừ stamina 2 lần và NullRef).
    /// </summary>
    private void OnConfirmButtonClicked()
    {
        Debug.Log("🎯 CONFIRM BUTTON CLICKED!");

        if (draftManager == null)
        {
            Debug.LogError("🚨 DraftManager chưa được gán!");
            
        }

        if (draftManager.selectedCards.Count == 0)
        {
            Debug.LogWarning("⚠️ Không có card nào được chọn!");
            
        }

        // Gây damage + Resolve thông qua DraftManager + BattleManager
        draftManager.ConfirmDraft();

        // Xoá card khỏi slot (UI)
        ClearAllSlots();
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
