using UnityEngine;

/// <summary>
/// Debug script để kiểm tra tất cả các tham chiếu component cần thiết cho hệ thống battle
/// Gắn vào GameObject chứa BattleManager
/// </summary>
public class BattleSystemDebugger : MonoBehaviour
{
    [Header("Main Components")]
    public BattleManager battleManager;
    public DraftManager draftManager;
    public BattleCardManager battleCardManager;
    
    void Start()
    {
        Debug.Log("=== BATTLE SYSTEM DEBUGGER ===");
        ValidateBattleManager();
        ValidateDraftManager();
        ValidateBattleCardManager();
        Debug.Log("=== END DEBUGGER ===");
    }
    
    void ValidateBattleManager()
    {
        Debug.Log("\n--- Checking BattleManager ---");
        if (battleManager == null)
        {
            Debug.LogError("❌ BattleManager chưa được gán!");
            return;
        }
        
        Debug.Log($"✅ BattleManager: {battleManager.name}");
        Debug.Log($"  DeckSystem: {(battleManager.deckSystem != null ? "✅" : "❌ NULL")}");
        Debug.Log($"  StaminaSystem: {(battleManager.staminaSystem != null ? "✅" : "❌ NULL")}");
        Debug.Log($"  EmometerSystem: {(battleManager.emometerSystem != null ? "✅" : "❌ NULL")}");
        Debug.Log($"  DraftManager: {(battleManager.draftManager != null ? "✅" : "❌ NULL")}");
        Debug.Log($"  PlayerStats: {(battleManager.playerStats != null ? "✅" : "❌ NULL")}");
        Debug.Log($"  CurrentMonster: {(battleManager.currentMonster != null ? "✅" : "❌ NULL")}");
        Debug.Log($"  BattleUI: {(battleManager.battleUI != null ? "✅" : "❌ NULL")}");
    }
    
    void ValidateDraftManager()
    {
        Debug.Log("\n--- Checking DraftManager ---");
        if (draftManager == null)
        {
            Debug.LogError("❌ DraftManager chưa được gán!");
            return;
        }
        
        Debug.Log($"✅ DraftManager: {draftManager.name}");
        Debug.Log($"  DeckSystem: {(draftManager.deckSystem != null ? "✅" : "❌ NULL")}");
        Debug.Log($"  StaminaSystem: {(draftManager.staminaSystem != null ? "✅" : "❌ NULL")}");
        Debug.Log($"  BattleManager: {(draftManager.battleManager != null ? "✅" : "❌ NULL - QUAN TRỌNG!")}");
        
        if (draftManager.battleManager == null)
        {
            Debug.LogError("🚨 CRITICAL: DraftManager.battleManager = NULL! Đây là lý do nút Confirm không tính sát thương!");
        }
    }
    
    void ValidateBattleCardManager()
    {
        Debug.Log("\n--- Checking BattleCardManager ---");
        if (battleCardManager == null)
        {
            Debug.LogError("❌ BattleCardManager chưa được gán!");
            return;
        }
        
        Debug.Log($"✅ BattleCardManager: {battleCardManager.name}");
        Debug.Log($"  HandZone: {(battleCardManager.handZone != null ? "✅" : "❌ NULL")}");
        Debug.Log($"  CardSlots: {(battleCardManager.cardSlots != null && battleCardManager.cardSlots.Count > 0 ? $"✅ ({battleCardManager.cardSlots.Count} slots)" : "❌ NULL or Empty")}");
        Debug.Log($"  CardPrefab: {(battleCardManager.cardPrefab != null ? "✅" : "❌ NULL")}");
        Debug.Log($"  CardDatabase: {(battleCardManager.cardDatabase != null ? "✅" : "❌ NULL")}");
        Debug.Log($"  DraftManager: {(battleCardManager.draftManager != null ? "✅" : "❌ NULL")}");
        Debug.Log($"  ConfirmButton: {(battleCardManager.confirmButton != null ? "✅" : "❌ NULL")}");
    }
}
