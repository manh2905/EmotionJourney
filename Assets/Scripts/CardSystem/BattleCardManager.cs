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

    [Header("Audio")]
    public AudioClip cardClickSound;
    
    private const int MAX_HAND_SIZE = 7;
    private const int MAX_DRAFT_SLOTS = 3;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeReferences();
        ValidateSlots(); 

        foreach (var slot in cardSlots)
        {
            if (slot != null) slot.ClearCard();
        }

        RefillHand();

        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            UpdateConfirmButton();
        }
    }

    private void InitializeReferences()
    {
        
        if (handZone == null)
        {
            GameObject handCanvas = GameObject.Find("HandZone_Canvas");
            if (handCanvas != null)
            {
                Debug.Log("HandZone_Canvas found!");
                handZone = handCanvas.GetComponent<RectTransform>();
            }
        }

        // Auto-find CardSlots if missing
        if (cardSlots == null || cardSlots.Count == 0)
        {
            GameObject slotContainer = GameObject.Find("CardSlot"); // The parent container
            if (slotContainer != null)
            {
                var slots = slotContainer.GetComponentsInChildren<CardSlot>();
                if (slots != null && slots.Length > 0)
                {
                    cardSlots = new List<CardSlot>(slots);
                }
            }
            
            // Fallback: Find all in scene
            if (cardSlots == null || cardSlots.Count == 0)
            {
                cardSlots = new List<CardSlot>(FindObjectsOfType<CardSlot>());
            }
            
            // Sort by sibling index to ensure order
            if (cardSlots != null)
            {
                cardSlots.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
            }
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
        if (SoundManager.Instance != null && cardClickSound != null)
        {
            SoundManager.Instance.PlaySFX(cardClickSound);
        }
        if (IsCardInAnySlot(card))
        {
            ReturnCardToHand(card);
        }
        else if (!draftManager.staminaSystem.CanUseCard(card.cardData.staminaCost))
        {
            Debug.LogWarning($"❌ Không đủ Stamina để dùng: {card.cardData.cardName} (Cần {card.cardData.staminaCost})");
            //battleUI?.ShowInsufficientStaminaWarning(); // Nếu có
            return;
            
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

        
        // Clear slot
        
        ClearCardFromAllSlots(card);



        CardSlot emptySlot = null;

        // Nếu là lá SỢ HÃI → CHỈ CHO PHÉP slot đầu tiên (index 0)
        if (card.cardData.emotionType == EmotionType.Scared)
        {
            CardSlot firstSlot = cardSlots[0]; // slot đầu

            if (firstSlot.IsEmpty)
            {
                emptySlot = firstSlot;
            }
            else
            {
                Debug.LogWarning("❌ Lá Sợ Hãi chỉ được đặt tại vị trí đầu tiên!");
                draftManager.RefundCardStamina(card.cardData);
                card.ReturnToHand(handZone);
                return;
            }
        }
        else
        {
            // Các lá bình thường → tìm slot trống bất kỳ
            foreach (var slot in cardSlots)
            {
                if (slot != null && slot.IsEmpty)
                {
                    emptySlot = slot;
                    break;
                }
            }
        }

        // Không tìm thấy slot
        if (emptySlot == null)
        {
            Debug.LogWarning("⚠ Không tìm thấy slot phù hợp!");
            draftManager.RefundCardStamina(card.cardData);
            return;
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
