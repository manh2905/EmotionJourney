using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Test script để verify button có hoạt động không
/// Gắn vào GameObject chứa Confirm Button
/// </summary>
public class TestConfirmButton : MonoBehaviour
{
    public Button confirmButton;

    void Start()
    {
        if (confirmButton != null)
        {
            // Force enable button
            confirmButton.interactable = true;
            
            // Add test listener
            confirmButton.onClick.AddListener(OnTestClick);
            
            Debug.Log("✅ TestConfirmButton: Button setup complete!");
        }
        else
        {
            Debug.LogError("❌ TestConfirmButton: Button chưa được gán!");
        }
    }

    void OnTestClick()
    {
        Debug.Log("🎯 BUTTON CLICKED! Button hoạt động bình thường!");
    }
}
