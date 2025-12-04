using System.Collections.Generic;
using UnityEngine;


public class MapController : MonoBehaviour
{
    public GameObject[] levelNodes;      // man1, man2, man3...
    public GameObject[] lockIcons;       // lock1, lock2, lock3...
    public GameObject[] completeIcons;   // xicon1, xicon2...
   
    public Animator[] lockAnimators;     // Animator của từng lock
    public CardDatabase cardDB;
    public List<RewardData> rewardList;
    

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("a1"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("a2"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("a3"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("a4"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("a5"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("h1"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("h2"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("h3"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("h4"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("h5"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("b1"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("b2"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("b3"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("b4"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("b5"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("s1"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("s2"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("s3"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("s4"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("s5"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("f1"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("f2"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("f3"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("f4"));
        CardUnlockManager.Instance.UnlockCard(cardDB.GetCardByID("f5"));


        int unlocked = PlayerPrefs.GetInt("LevelUnlocked", 1);

        for (int i = 0; i < levelNodes.Length; i++)
        {
            bool isUnlocked = i < unlocked;
            bool isCompleted = i < unlocked - 1;

            // Xicon (vẫn giữ nguyên)
            completeIcons[i].SetActive(isCompleted);


            if (isCompleted)
            {
                RewardData data = rewardList.Find(r => r.level == i + 1);
                // level = 1,2,3... nhưng i = 0,1,2 → phải +1

                if (data != null)
                {
                    Debug.Log($"[MAP] Unlocking reward for LEVEL {i + 1}");
                    CardUnlockManager.Instance.UnlockCards(data.rewardCards);
                }
            }


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
