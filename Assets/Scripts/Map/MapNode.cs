using UnityEngine;

public class MapNode : MonoBehaviour
{
    public int level; // màn này là màn số mấy?

    void OnMouseDown()
    {
        int unlocked = PlayerPrefs.GetInt("LevelUnlocked", 1);

        // Chỉ khi nhấn đúng màn hiện tại mới được tính
        if (level == unlocked)
        {
            Debug.Log("Hoàn thành màn: " + level);

            // Mở màn tiếp theo
            PlayerPrefs.SetInt("LevelUnlocked", unlocked + 1);

            // Reload map để update UI
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
        else
        {
            Debug.Log("Chưa thể hoàn thành màn này! Hiện đang ở màn: " + unlocked);
        }
    }

    public void OnClickNode()
    {
        MapController.UnlockNextLevel(level);

        Debug.Log("Clicked level " + level);
        Debug.Log("LevelUnlocked = " + PlayerPrefs.GetInt("LevelUnlocked", 1));

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

}





