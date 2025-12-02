using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// Quản lý hiển thị UI của 1 lá bài
/// Hiển thị: Card Name, Emotion Type, Damage, Stamina Cost
/// </summary>
public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Card Data")]
    public CardData cardData;                   // Dữ liệu của lá bài này

    [Header("UI Components")]
    public Image cardImage;

    // State
    private bool isSelected = false;
    private bool isDisabled = false;
    private bool isHovered = false;

    /// <summary>
    /// Khởi tạo UI với CardData và Image
    /// </summary>
    public void Initialize(CardData data, Sprite art)
    {
        cardData = data;
        if (cardImage != null && art != null)
        {
            cardImage.sprite = art;
        }
    }

    /// <summary>
    /// Set trạng thái selected
    /// </summary>
    public void SetSelected(bool selected)
    {
        isSelected = selected;
    }

    /// <summary>
    /// Set trạng thái disabled (không đủ stamina...)
    /// </summary>
    public void SetDisabled(bool disabled)
    {
        isDisabled = disabled;
    }

    /// <summary>
    /// Hover enter effect
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isDisabled)
        {
            isHovered = true;
            // Optional: Scale up effect
            transform.localScale = Vector3.one * 1.1f;
        }
    }

    /// <summary>
    /// Hover exit effect
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        // Reset scale
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Click handler (gọi từ Button hoặc EventTrigger)
    /// </summary>
    public void OnCardClicked()
    {
        if (isDisabled) return;

        // Delegate to BattleCardManager
        if (BattleCardManager.Instance != null)
        {
            BattleCardManager.Instance.OnCardClicked(this);
        }
        else
        {
            // Fallback old behavior if Manager not present
            SetSelected(!isSelected);
            Debug.Log($"Card clicked: {cardData.cardName}, Selected: {isSelected}");
        }
    }

    // ============ MOVEMENT LOGIC ============
    
    public bool isInSlot = false;
    private bool isMoving = false;
    private bool isReturning = false;
    private Vector3 targetPosition;
    private float moveSpeed = 10f;
    
    private int originalSiblingIndex;
    private GameObject placeholder;
    private LayoutElement layoutElement;

    private void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = gameObject.AddComponent<LayoutElement>();
        }
    }

    public void MoveToSlot(CardSlot slot)
    {
        if (!isInSlot)
        {
            originalSiblingIndex = transform.GetSiblingIndex();
        }

        isInSlot = true;
        isMoving = true;
        isReturning = false;
        targetPosition = slot.transform.position;
        transform.SetParent(slot.transform); // Optional: reparent to slot
        
        // Reset rotation/scale if needed
        //transform.rotation = Quaternion.identity;
    }

    public void ReturnToHand(RectTransform handZone)
    {
        isInSlot = false;
        isMoving = true;
        isReturning = true;

        // Create placeholder to reserve space in layout
        CreatePlaceholder(handZone);

        transform.SetParent(handZone);
        layoutElement.ignoreLayout = true;
        
        // Reset rotation/scale if needed
        transform.rotation = Quaternion.identity;
    }

    private void CreatePlaceholder(RectTransform handZone)
    {
        if (placeholder != null) Destroy(placeholder);

        placeholder = new GameObject("CardPlaceholder");
        placeholder.transform.SetParent(handZone);
        placeholder.transform.SetSiblingIndex(originalSiblingIndex);
        
        // Copy layout properties
        LayoutElement placeholderLE = placeholder.AddComponent<LayoutElement>();
        placeholderLE.preferredWidth = layoutElement.preferredWidth;
        placeholderLE.preferredHeight = layoutElement.preferredHeight;
        placeholderLE.flexibleWidth = layoutElement.flexibleWidth;
        placeholderLE.flexibleHeight = layoutElement.flexibleHeight;
    }

    private void Update()
    {
        if (isMoving)
        {
            if (isReturning && placeholder != null)
            {
                targetPosition = placeholder.transform.position;
            }

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
            
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
                
                if (isReturning)
                {
                    OnReturnedToHand();
                }
            }
        }
    }

    private void OnReturnedToHand()
    {
        isReturning = false;
        layoutElement.ignoreLayout = false;
        transform.SetSiblingIndex(originalSiblingIndex);
        
        if (placeholder != null)
        {
            Destroy(placeholder);
        }
    }
}
