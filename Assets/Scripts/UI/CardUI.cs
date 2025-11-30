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
    public TextMeshProUGUI cardNameText;        // Tên lá bài
    public TextMeshProUGUI damageText;          // Sát thương
    public TextMeshProUGUI staminaCostText;     // Chi phí stamina
    public TextMeshProUGUI emotionValueText;    // Giá trị emotion shift (+2, -3...)
    public Image cardBackground;                // Background image
    public Image emotionIcon;                   // Icon cảm xúc (Funny, Angry...)
    
    [Header("Visual States")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;
    public Color disabledColor = Color.gray;
    public Color hoverColor = new Color(1f, 1f, 0.8f);

    [Header("Emotion Colors")]
    public Color funnyColor = new Color(1f, 0.9f, 0.3f);     // Vàng
    public Color boredColor = new Color(0.5f, 0.5f, 0.5f);   // Xám
    public Color scaredColor = new Color(0.6f, 0.4f, 0.8f);  // Tím
    public Color happyColor = new Color(0.3f, 1f, 0.5f);     // Xanh lá
    public Color angryColor = new Color(1f, 0.3f, 0.3f);     // Đỏ

    // State
    private bool isSelected = false;
    private bool isDisabled = false;
    private bool isHovered = false;

    /// <summary>
    /// Khởi tạo UI với CardData
    /// </summary>
    public void SetCardData(CardData data)
    {
        cardData = data;
        UpdateVisuals();
    }

    /// <summary>
    /// Cập nhật tất cả UI elements
    /// </summary>
    private void UpdateVisuals()
    {
        if (cardData == null)
        {
            Debug.LogWarning("CardData is null!");
            return;
        }

        // Card Name
        if (cardNameText != null)
        {
            cardNameText.text = cardData.cardName;
        }

        // Damage
        if (damageText != null)
        {
            damageText.text = cardData.damageValue.ToString();
        }

        // Stamina Cost
        if (staminaCostText != null)
        {
            staminaCostText.text = cardData.staminaCost.ToString();
        }

        // Emotion Value (với dấu +/-)
        if (emotionValueText != null)
        {
            string sign = cardData.emotionValue > 0 ? "+" : "";
            emotionValueText.text = $"{sign}{cardData.emotionValue}";
        }

        // Background color theo emotion type
        UpdateEmotionColor();
    }

    /// <summary>
    /// Cập nhật màu background theo emotion type
    /// </summary>
    private void UpdateEmotionColor()
    {
        if (cardBackground == null || cardData == null) return;

        Color emotionColor = normalColor;

        switch (cardData.emotionType)
        {
            case EmotionType.Funny:
                emotionColor = funnyColor;
                break;
            case EmotionType.Bored:
                emotionColor = boredColor;
                break;
            case EmotionType.Scared:
                emotionColor = scaredColor;
                break;
            case EmotionType.Happy:
                emotionColor = happyColor;
                break;
            case EmotionType.Angry:
                emotionColor = angryColor;
                break;
        }

        // Áp dụng màu với state modifiers
        if (isDisabled)
        {
            cardBackground.color = disabledColor;
        }
        else if (isSelected)
        {
            cardBackground.color = selectedColor;
        }
        else if (isHovered)
        {
            cardBackground.color = Color.Lerp(emotionColor, hoverColor, 0.5f);
        }
        else
        {
            cardBackground.color = emotionColor;
        }
    }

    /// <summary>
    /// Set trạng thái selected
    /// </summary>
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateEmotionColor();
    }

    /// <summary>
    /// Set trạng thái disabled (không đủ stamina...)
    /// </summary>
    public void SetDisabled(bool disabled)
    {
        isDisabled = disabled;
        UpdateEmotionColor();
    }

    /// <summary>
    /// Hover enter effect
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isDisabled)
        {
            isHovered = true;
            UpdateEmotionColor();
            
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
        UpdateEmotionColor();
        
        // Reset scale
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Click handler (gọi từ Button hoặc EventTrigger)
    /// </summary>
    public void OnCardClicked()
    {
        if (isDisabled) return;

        // Toggle selected state
        SetSelected(!isSelected);
        
        Debug.Log($"Card clicked: {cardData.cardName}, Selected: {isSelected}");
        
        // TODO: Notify DraftManager về việc select/deselect
    }
}
