using UnityEngine;
using UnityEngine.EventSystems;

public class CardAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("Components")]
    private Animator anim;

    [Header("Settings")]
    public GameObject highlightObj;

    private bool isSelected = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (highlightObj != null) highlightObj.SetActive(false);
    }

    // --- 1. HOVER (Khi chuột vào) ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Animator found? " + (anim != null));

        // Cứ chuột vào là nhấc lên (bất kể đang chọn hay không)
        if (anim != null)
        {
            anim.SetTrigger("OnHover");
        }
    }

    // --- 2. EXIT (Khi chuột ra) - PHẦN QUAN TRỌNG ĐÃ SỬA ---
    public void OnPointerExit(PointerEventData eventData)
    {
        // Chỉ tụt xuống khi LÁ BÀI CHƯA ĐƯỢC CHỌN
        // Nếu đang chọn (isSelected == true) thì không làm gì cả -> Nó sẽ giữ nguyên vị trí nhấc lên
        if (anim != null && !isSelected)
        {
            anim.SetTrigger("OnExit");
        }
    }

    // --- 3. CLICK (Chọn/Bỏ chọn) - ĐÃ SỬA ---
    public void OnPointerDown(PointerEventData eventData)
    {
        // Đảo trạng thái
        isSelected = !isSelected;

        // Bật/Tắt viền sáng
        if (highlightObj != null) highlightObj.SetActive(isSelected);

        if (anim != null)
        {
            if (isSelected)
            {
                // Nếu vừa được chọn -> Đảm bảo nó ở trạng thái nhấc lên
                // (Thường thì chuột đang ở đây nên nó đã lên rồi, nhưng gọi thêm cho chắc chắn)
                anim.SetTrigger("OnHover");

            }
            else
            {
                // Nếu vừa BỎ CHỌN -> Bắt buộc tụt xuống ngay lập tức
                anim.SetTrigger("OnExit");
            }
        }

        Debug.Log(isSelected ? "Đã chọn (Giữ nguyên vị trí)" : "Bỏ chọn (Tụt xuống)");
    }
}