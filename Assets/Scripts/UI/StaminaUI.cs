using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Quản lý hiển thị Stamina của Player trên UI
/// Hỗ trợ cả icon-based (energy orbs) và slider-based display
/// </summary>
public class StaminaUI : MonoBehaviour
{
    [Header("Display Mode")]
    public bool useIconMode = false;             // true = hiển thị bằng icons, false = slider

    [Header("Icon Mode Components")]
    public GameObject staminaIconPrefab;        // Prefab của 1 stamina icon (ví dụ: energy orb)
    public Transform iconContainer;             // Parent transform chứa các icons
    public Sprite fullStaminaSprite;            // Sprite khi stamina đầy
    public Sprite emptyStaminaSprite;           // Sprite khi stamina rỗng

    [Header("Slider Mode Components")]
    public Slider staminaSlider;                // Slider hiển thị stamina
    public Image fillImage;                     // Fill image của slider
    public Color fullColor = new Color(0.3f, 0.8f, 1f);  // Màu xanh dương
    public Color emptyColor = Color.gray;       // Màu xám khi hết stamina

    [Header("Text Display")]
    public TextMeshProUGUI staminaText;        // Text hiển thị "3/5"

    private List<Image> staminaIcons = new List<Image>();  // Danh sách icons (icon mode)
    private int maxStamina;
    private int currentStamina;

    /// <summary>
    /// Khởi tạo hệ thống Stamina UI
    /// </summary>
    public void Initialize(int max)
    {
        maxStamina = max;
        currentStamina = max;

        if (useIconMode)
        {
            SetupIconMode();
        }
        else
        {
            SetupSliderMode();
        }

        UpdateStaminaDisplay();
    }

    /// <summary>
    /// Setup icon-based display (energy orbs)
    /// </summary>
    private void SetupIconMode()
    {
        if (iconContainer == null || staminaIconPrefab == null) return;

        // Clear existing icons
        foreach (Transform child in iconContainer)
        {
            Destroy(child.gameObject);
        }
        staminaIcons.Clear();

        // Tạo icons theo maxStamina
        for (int i = 0; i < maxStamina; i++)
        {
            GameObject iconObj = Instantiate(staminaIconPrefab, iconContainer);
            Image iconImage = iconObj.GetComponent<Image>();
            if (iconImage != null)
            {
                staminaIcons.Add(iconImage);
            }
        }
    }

    /// <summary>
    /// Setup slider-based display
    /// </summary>
    private void SetupSliderMode()
    {
        if (staminaSlider == null) return;

        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = maxStamina;
    }

    /// <summary>
    /// Cập nhật hiển thị Stamina
    /// </summary>
    public void UpdateStamina(int current, int max)
    {
        currentStamina = current;
        maxStamina = max;
        UpdateStaminaDisplay();
    }

    /// <summary>
    /// Update visual display
    /// </summary>
    private void UpdateStaminaDisplay()
    {
        Debug.Log($"📊 UpdateStaminaDisplay called - Mode: {(useIconMode ? "ICON" : "SLIDER")}, Current: {currentStamina}/{maxStamina}");
        
        if (useIconMode)
        {
            Debug.Log($"   Icon Mode - Icons count: {staminaIcons.Count}");
            UpdateIconDisplay();
        }
        else
        {
            Debug.Log($"   Slider Mode - Slider: {(staminaSlider != null ? "OK" : "NULL")}");
            UpdateSliderDisplay();
        }

        UpdateStaminaText();
    }

    /// <summary>
    /// Cập nhật icons (icon mode)
    /// </summary>
    private void UpdateIconDisplay()
    {
        for (int i = 0; i < staminaIcons.Count; i++)
        {
            if (i < currentStamina)
            {
                // Icon đầy
                staminaIcons[i].sprite = fullStaminaSprite;
                staminaIcons[i].color = Color.white;
            }
            else
            {
                // Icon rỗng
                staminaIcons[i].sprite = emptyStaminaSprite;
                staminaIcons[i].color = emptyColor;
            }
        }
    }

    /// <summary>
    /// Cập nhật slider (slider mode)
    /// </summary>
    private void UpdateSliderDisplay()
    {
        if (staminaSlider == null) return;

        staminaSlider.value = currentStamina;

        if (fillImage != null)
        {
            float percentage = (float)currentStamina / maxStamina;
            fillImage.color = Color.Lerp(emptyColor, fullColor, percentage);
        }
    }

    /// <summary>
    /// Cập nhật text
    /// </summary>
    private void UpdateStaminaText()
    {
        if (staminaText != null)
        {
            staminaText.text = $"{currentStamina}/{maxStamina}";
        }
    }

    /// <summary>
    /// Show effect khi không đủ stamina
    /// </summary>
    public void ShowInsufficientStaminaEffect()
    {
        // TODO: Implement shake/flash effect
        Debug.Log("Không đủ Stamina!");
    }
}
