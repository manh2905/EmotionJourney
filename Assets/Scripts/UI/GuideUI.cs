using UnityEngine;
using TMPro;

/// <summary>
/// Quản lý nội dung hiển thị trong Guide Panel
/// Attach script này vào GuidePanel GameObject trong scene Battle1
/// </summary>
public class GuideUI : MonoBehaviour
{
    [Header("Guide Content")]
    public TextMeshProUGUI guideTitle;
    public TextMeshProUGUI guideContent;

    private void OnEnable()
    {
        // Hiển thị nội dung guide khi panel được mở
        DisplayGuideContent();
    }

    private void DisplayGuideContent()
    {
        if (guideTitle != null)
        {
            guideTitle.text = "GUIDE";
        }

        if (guideContent != null)
        {
            guideContent.text = GetGuideText();
        }
    }

    private string GetGuideText()
    {
        return @"<b><size=28>CÁC LÁ BÀI</size></b>

<b><color=#FFD700>Ý nghĩa các chỉ số trên lá bài:</color></b>

• <b>Damage (Sát thương):</b> Lượng sát thương gây cho đối thủ
• <b>Emotion Value (Giá trị cảm xúc):</b> Thay đổi thanh Emometer của bạn
• <b>Stamina Cost (Chi phí thể lực):</b> Số stamina cần để sử dụng lá bài

<b><color=#FFD700>Các loại cảm xúc:</color></b>

• <b><color=#FFFF00>Funny (Vui vẻ)</color></b> - Cảm xúc tích cực
• <b><color=#4169E1>Bored (Buồn bã)</color></b> - Cảm xúc tiêu cực
• <b><color=#9370DB>Scared (Sợ hãi)</color></b> - Cảm xúc tiêu cực
• <b><color=#FF69B4>Happy (Hạnh phúc)</color></b> - Cảm xúc tích cực
• <b><color=#FF4500>Angry (Giận dữ)</color></b> - Cảm xúc tiêu cực


<b><size=28>TRẠNG THÁI BURNOUT</size></b>

<b><color=#FFD700>Thanh Emometer:</color></b>
Thanh cảm xúc dao động từ <b>-10 đến +10</b>

• <b><color=#00FF00>Vùng an toàn:</color></b> Từ -5 đến +5
• <b><color=#FFA500>Vùng cảnh báo:</color></b> Từ -10 đến -5 hoặc +5 đến +10

<b><color=#FFD700>Burnout là gì?</color></b>

Khi thanh cảm xúc đạt <b>±10</b>, bạn sẽ rơi vào trạng thái <b>BURNOUT</b>:

• <b><color=#00FF00>BURNOUT TÍCH CỰC (+10):</color></b>
  Cảm xúc quá cao, có thể gây ảnh hưởng tiêu cực

• <b><color=#FF4500>BURNOUT TIÊU CỰC (-10):</color></b>
  Cảm xúc quá thấp, dễ bị tổn thương

<b><color=#FFD700>⚠️ Lưu ý:</color></b> Hãy cân bằng việc sử dụng các lá bài để giữ cảm xúc ở vùng an toàn!";
    }
}
