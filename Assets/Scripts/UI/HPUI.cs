using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Quản lý hiển thị HP của Player trên UI
/// </summary>
public class HPUI : MonoBehaviour
{
    [Header("UI Components")]
    public Slider hpSlider;                     // Slider hiển thị HP bar
    public Image fillImage;                     // Image fill của HP bar (optional - để đổi màu)
    public TextMeshProUGUI hpText;             // Text hiển thị "50/100"
    
    [Header("Visual Settings")]
    public Color normalColor = Color.green;     // Màu HP bình thường
    public Color warningColor = Color.yellow;   // Màu HP thấp (< 50%)
    public Color criticalColor = Color.red;     // Màu HP rất thấp (< 25%)
    public float smoothSpeed = 5f;              // Tốc độ smooth khi HP thay đổi

    private float targetValue;                  // Giá trị HP target (để smooth)

    void Start()
    {
        hpSlider = GetComponent<Slider>();
        // Khởi tạo giá trị ban đầu
        if (hpSlider != null)
        {
            targetValue = hpSlider.value;
        }
    }

    void Update()
    {
        // Smooth transition cho HP bar
        if (hpSlider != null && Mathf.Abs(hpSlider.value - targetValue) > 0.01f)
        {
            hpSlider.value = Mathf.Lerp(hpSlider.value, targetValue, Time.deltaTime * smoothSpeed);
        }
    }

    /// <summary>
    /// Set giá trị HP tối đa
    /// </summary>
    public void SetMaxHP(float maxHP)
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = maxHP;
            targetValue = maxHP;
        }
        
        UpdateHPText(maxHP, maxHP);
        UpdateHPColor(1f); // 100% HP
    }

    /// <summary>
    /// Cập nhật HP hiện tại
    /// </summary>
    public void UpdateHP(float currentHP, float maxHP)
    {
        if (hpSlider != null)
        {
            targetValue = currentHP;
            float percentage = currentHP / maxHP;
            UpdateHPColor(percentage);
        }
        
        UpdateHPText(currentHP, maxHP);
    }

    /// <summary>
    /// Cập nhật text hiển thị HP
    /// </summary>
    private void UpdateHPText(float current, float max)
    {
        if (hpText != null)
        {
            hpText.text = $"{Mathf.RoundToInt(current)}/{Mathf.RoundToInt(max)}";
        }
    }

    /// <summary>
    /// Thay đổi màu HP bar dựa trên % HP còn lại
    /// </summary>
    private void UpdateHPColor(float percentage)
    {
        if (fillImage == null) return;

        if (percentage >= 0.5f)
        {
            fillImage.color = normalColor;
        }
        else if (percentage >= 0.25f)
        {
            fillImage.color = warningColor;
        }
        else
        {
            fillImage.color = criticalColor;
        }
    }

    /// <summary>
    /// Hiển thị damage effect (flash màu đỏ - optional)
    /// </summary>
    public void ShowDamageEffect()
    {
        // TODO: Implement damage flash effect
        // Ví dụ: StartCoroutine(DamageFlash());
    }
}
