using UnityEngine;
using TMPro;

public class BurnoutIndicator : MonoBehaviour
{
    public static BurnoutIndicator Instance;

    public TextMeshProUGUI burnoutText;
    public float flashSpeed = 4f;

    private bool isFlashing = false;
    private Color flashColor;

    private void Awake()
    {
        Instance = this;
        HideBurnout();
    }

    private void Update()
    {
        if (isFlashing)
        {
            float alpha = Mathf.Abs(Mathf.Sin(Time.time * flashSpeed));
            burnoutText.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
        }
    }

    public void ShowPositiveBurnout()
    {
        burnoutText.text = "BURNOUT";
        flashColor = Color.green;
        isFlashing = true;
    }

    public void ShowNegativeBurnout()
    {
        burnoutText.text = "BURNOUT";
        flashColor = Color.red;
        isFlashing = true;
    }

    public void HideBurnout()
    {
        isFlashing = false;
        burnoutText.text = "";
        burnoutText.color = new Color(1, 1, 1, 0); // ẩn hoàn toàn
    }
}
