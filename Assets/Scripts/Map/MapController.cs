using UnityEngine;


public class MapController : MonoBehaviour
{
    public GameObject[] levelNodes;      // man1, man2, man3...
    public GameObject[] lockIcons;       // lock1, lock2, lock3...
    public GameObject[] completeIcons;   // xicon1, xicon2...

    public Animator[] lockAnimators;     // Animator của từng lock

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        int unlocked = PlayerPrefs.GetInt("LevelUnlocked", 1);

        for (int i = 0; i < levelNodes.Length; i++)
        {
            bool isUnlocked = i < unlocked;
            bool isCompleted = i < unlocked - 1;

            // Xicon (vẫn giữ nguyên)
            completeIcons[i].SetActive(isCompleted);


            // Nếu đây là node VỪA mới mở (ví dụ màn 2 khi unlocked=2)
            if (i == unlocked - 1 && isUnlocked)
            {
                // KHÔNG ĐƯỢC TẮT!! 
                // Phải để nó bật để Animator còn chạy.
                if (lockAnimators[i] != null)
                {
                    lockAnimators[i].SetTrigger("unlock");
                    //lockIcons[i].SetActive(false);
                }

                

                // → ổ khóa sẽ được TỰ TẮT sau khi animation kết thúc (timeline)
            }
            else
            {
                // Các node đã unlock từ trước → khóa phải tắt
                lockIcons[i].SetActive(!isUnlocked);
            }
        }
    }


    public static void UnlockNextLevel(int level)
    {
        int current = PlayerPrefs.GetInt("LevelUnlocked", 1);

        if (level >= current)
        {
            PlayerPrefs.SetInt("LevelUnlocked", current + 1);
        }
    }
}
