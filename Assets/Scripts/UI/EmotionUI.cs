using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Quản lý hiển thị Emometer (Thanh cảm xúc) trên UI
/// Hỗ trợ 2 modes: Slider và Scale (với mũi tên)
/// Range: -10 (Tiêu cực) đến +10 (Tích cực), 0 là trung bình
/// </summary>
public class EmotionUI : MonoBehaviour
{
    [Header("Display Mode")]
    public bool useScaleMode = true;            // true = dùng cân với mũi tên, false = slider

    [Header("Scale Mode - Components")]
    public Transform arrowTransform;            // Transform của mũi tên (sẽ rotate)
    public Image scaleBackground;               // Image của cái cân (background)
    public float minAngle = -90f;               // Góc tương ứng với -10 (tiêu cực max)
    public float maxAngle = 90f;                // Góc tương ứng với +10 (tích cực max)
    public float rotationSpeed = 5f;            // Tốc độ smooth rotation

    [Header("Common Components")]
    public TextMeshProUGUI valueText;          // Text hiển thị giá trị (-5, 0, +7...)
    public TextMeshProUGUI statusText;         // Text hiển thị trạng thái (Cân bằng, Burnout...)
    public GameObject burnoutWarningUI;         // Warning khi gần burnout

    [Header("Color Settings")]
    public Color negativeColor = new Color(0.8f, 0.2f, 0.2f);  // Đỏ (negative)
    public Color neutralColor = new Color(0.5f, 0.5f, 0.5f);   // Xám (neutral)
    public Color positiveColor = new Color(0.2f, 0.8f, 0.3f);  // Xanh lá (positive)
    public Color burnoutColor = new Color(1f, 0.5f, 0f);       // Cam (burnout warning)

    private const int MIN_EMOTION = -10;
    private const int MAX_EMOTION = 10;
    private float targetAngle;                  // Target rotation angle (scale mode)
    private int currentValue = 0;

    void Start()
    {
        if (useScaleMode)
        {
            // Initialize scale mode
            targetAngle = 0f; // Start at neutral (0 degrees)
            if (arrowTransform != null)
            {
                arrowTransform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
        
        if (burnoutWarningUI != null)
        {
            burnoutWarningUI.SetActive(false);
        }
    }

    void Update()
    {
        // Smooth rotation for scale mode
        if (useScaleMode && arrowTransform != null)
        {
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            arrowTransform.localRotation = Quaternion.Lerp(
                arrowTransform.localRotation, 
                targetRotation, 
                Time.deltaTime * rotationSpeed
            );
        }
    }

    /// <summary>
    /// Cập nhật giá trị Emometer
    /// </summary>
    public void UpdateEmotion( int value, bool isBurnedOut, bool isPositiveBurnout)
    {
        // Clamp giá trị
        value = Mathf.Clamp(value, MIN_EMOTION, MAX_EMOTION);
        currentValue = value;

        if (useScaleMode)
        {
            UpdateScaleMode(value, isBurnedOut);
        }

        // Update text value (chung cho cả 2 modes)
        if (valueText != null)
        {
            string sign = value > 0 ? "+" : "";
            valueText.text = $"{sign}{value}";
        }

        // Update status text
        UpdateStatusText(value, isBurnedOut, isPositiveBurnout);

        // Show/hide burnout warning
        UpdateBurnoutWarning(isBurnedOut);
    }

    /// <summary>
    /// Update cho Scale Mode (mũi tên quay)
    /// </summary>
    private void UpdateScaleMode(int value, bool isBurnedOut)
    {
        // Chuyển đổi emotion value (-10 to +10) sang góc quay
        // -10 -> minAngle (ví dụ: -90°)
        //   0 -> 0°
        // +10 -> maxAngle (ví dụ: +90°)
        
        float normalizedValue = (float)(value - MIN_EMOTION) / (MAX_EMOTION - MIN_EMOTION); // 0 to 1
        targetAngle = Mathf.Lerp(minAngle, maxAngle, normalizedValue);

        // Update arrow color (nếu có Image component)
        if (arrowTransform != null)
        {
            Image arrowImage = arrowTransform.GetComponent<Image>();
            if (arrowImage != null)
            {
                arrowImage.color = GetColorForValue(value, isBurnedOut);
            }
        }

        // Update scale background color (optional)
        if (scaleBackground != null)
        {
            scaleBackground.color = GetColorForValue(value, isBurnedOut);
        }
    }


    /// <summary>
    /// Lấy màu tương ứng với giá trị emotion
    /// </summary>
    private Color GetColorForValue(int value, bool isBurnedOut)
    {
        if (isBurnedOut)
        {
            return burnoutColor;
        }

        // Gradient từ negative -> neutral -> positive
        float normalizedValue = (float)(value - MIN_EMOTION) / (MAX_EMOTION - MIN_EMOTION);
        
        if (normalizedValue < 0.5f)
        {
            // Negative to Neutral (-10 to 0)
            return Color.Lerp(negativeColor, neutralColor, normalizedValue * 2f);
        }
        else
        {
            // Neutral to Positive (0 to +10)
            return Color.Lerp(neutralColor, positiveColor, (normalizedValue - 0.5f) * 2f);
        }
    }

    /// <summary>
    /// Cập nhật text trạng thái
    /// </summary>
    private void UpdateStatusText(int value, bool isBurnedOut, bool isPositiveBurnout)
    {
        if (statusText == null) return;

        if (isBurnedOut)
        {
            statusText.text = isPositiveBurnout ? "BURNOUT TÍCH CỰC!" : "BURNOUT TIÊU CỰC!";
            statusText.color = burnoutColor;
        }
        else if (value > 5)
        {
            statusText.text = "Tích cực cao";
            statusText.color = positiveColor;
        }
        else if (value < -5)
        {
            statusText.text = "Tiêu cực cao";
            statusText.color = negativeColor;
        }
        else
        {
            statusText.text = "Cân bằng";
            statusText.color = neutralColor;
        }
    }

    /// <summary>
    /// Hiển thị/ẩn cảnh báo burnout
    /// </summary>
    private void UpdateBurnoutWarning(bool isBurnedOut)
    {
        if (burnoutWarningUI != null)
        {
            burnoutWarningUI.SetActive(isBurnedOut);
        }
    }

    /// <summary>
    /// Effect khi emotion thay đổi (optional animation)
    /// </summary>
    public void PlayEmotionShiftEffect(int shiftAmount)
    {
        // Optional: Add shake/pulse effect
        if (useScaleMode && arrowTransform != null)
        {
            // TODO: Có thể thêm shake effect
        }
        
        Debug.Log($"Emotion shifted by {shiftAmount}");
    }
}
