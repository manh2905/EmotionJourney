using System.Collections.Generic;
using UnityEngine;

public class DraftManager : MonoBehaviour
{
    [Header("Dependencies")]
    public DeckSystem deckSystem;
    public StaminaSystem staminaSystem;
    public BattleManager battleManager;

    [HideInInspector]
    public List<CardData> selectedCards = new List<CardData>();

    private const int MAX_SELECTION = 3;

    public void StartDraftPhase()
    {
        Debug.Log("DraftManager Awake: " + GetInstanceID());

        Debug.Log("Draft stamina object: " + staminaSystem.gameObject.name);
        selectedCards.Clear();
    }

    /// <summary>
    /// Được gọi khi NGƯỜI CHƠI CHỌN CARD vào slot.
    /// Kiểm tra giới hạn + stamina, NẾU OK thì:
    /// - Add vào selectedCards
    /// - Trừ stamina NGAY
    /// </summary>
    public bool TrySelectCard(CardData cardToSelect)
    {
        // Chống NullRef
        if (cardToSelect == null)
        {
            Debug.LogError("❌ DraftManager.TrySelectCard: cardToSelect == NULL!");
            return false;
        }

        if (staminaSystem == null)
        {
            Debug.LogError("❌ DraftManager.TrySelectCard: staminaSystem == NULL! Quên gán trong Inspector?");
            return false;
        }

        // 1. Giới hạn 3 lá
        if (selectedCards.Count >= MAX_SELECTION)
        {
            Debug.Log("⚠ Đã chọn đủ 3 lá.");
            return false;
        }

        // 2. Kiểm tra stamina hiện tại
        if (!staminaSystem.CanUseCard(cardToSelect.staminaCost))
        {
            Debug.Log(
                $"❌ Không đủ Stamina! Hiện tại: {staminaSystem.GetCurrentStamina()}, " +
                $"Cần: {cardToSelect.staminaCost} cho {cardToSelect.cardName}");
            return false;
        }

        // 3. Chấp nhận chọn: add + trừ stamina
        selectedCards.Add(cardToSelect);
        staminaSystem.ConsumeStamina(cardToSelect.staminaCost);

        Debug.Log(
            $"✅ Chọn {cardToSelect.cardName} (Cost: {cardToSelect.staminaCost}). " +
            $"Tổng: {selectedCards.Count}/{MAX_SELECTION}. Stamina còn: {staminaSystem.GetCurrentStamina()}");

        return true;
    }

    /// <summary>
    /// Bỏ chọn card → hoàn lại stamina
    /// </summary>
    public void RefundCardStamina(CardData card)
    {
        if (card == null)
        {
            Debug.LogError("❌ RefundCardStamina: card == NULL");
            return;
        }

        if (staminaSystem == null)
        {
            Debug.LogError("❌ RefundCardStamina: staminaSystem == NULL");
            return;
        }

        if (selectedCards.Contains(card))
        {
            selectedCards.Remove(card);
            staminaSystem.RefundStamina(card.staminaCost);
            Debug.Log(
                $"♻ Hoàn {card.staminaCost} stamina cho {card.cardName}. " +
                $"Stamina hiện tại: {staminaSystem.GetCurrentStamina()}");
        }
    }

    /// <summary>
    /// Khi bấm Confirm: KHÔNG trừ thêm stamina nữa,
    /// chỉ gửi selectedCards sang BattleManager để gây damage.
    /// </summary>
    public void ConfirmDraft()
    {
        if (selectedCards.Count == 0)
        {
            Debug.LogWarning("⚠ Không có card nào được chọn!");
            return;
        }

        Debug.Log($"🎯 Confirm Draft với {selectedCards.Count} lá. Sang Resolve Phase...");

        battleManager.ProcessPlayerActions(selectedCards);

        // Dọn list cho lượt sau
        selectedCards.Clear();
    }
}
