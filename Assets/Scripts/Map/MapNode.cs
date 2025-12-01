using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MapNode : MonoBehaviour
{
    public int level; // màn này là màn số mấy?
    

    void OnMouseDown()
    {
        int unlocked = PlayerPrefs.GetInt("LevelUnlocked", 1);

        // Chỉ khi nhấn đúng màn hiện tại mới được tính
        if (level <= unlocked)
        {
            Debug.Log("Vào màn: " + level);
            BattleLoader.currentLevel = level;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Battle"+level);
        }
        else
        {
            Debug.Log("Màn này chưa mở khóa!");
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





