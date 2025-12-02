using UnityEngine;

/// <summary>
/// Component gắn vào các ô chứa bài (Slot)
/// </summary>
public class CardSlot : MonoBehaviour
{
    public int slotIndex; // 0, 1, 2
    public CardUI currentCard;
    
    public bool IsEmpty => currentCard == null;

    public RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void AssignCard(CardUI card)
    {
        currentCard = card;
    }

    public void ClearCard()
    {
        currentCard = null;
    }
}
