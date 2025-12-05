using UnityEngine;
using TMPro;
using System.Collections;
public class MessageLogSystem : MonoBehaviour
{
    public static MessageLogSystem Instance;

    public TextMeshProUGUI logText;

    private void Awake()
    {
        Instance = this;
        logText.text = "";
    }

    public void AddMessage(string msg)
    {
        logText.text += "• " + msg + "\n";
    }

    public void ClearLog()
    {
        logText.text = "";
    }
    public void ShowTemporaryMessage(string msg, float duration = 2f)
    {
        StartCoroutine(ShowTempRoutine(msg, duration));
    }

    private IEnumerator ShowTempRoutine(string msg, float duration)
    {
        string fullMsg = "• " + msg + "\n";
        logText.text += fullMsg;

        yield return new WaitForSeconds(duration);

        // Xóa đúng dòng vừa thêm
        logText.text = logText.text.Replace(fullMsg, "");
    }
}
