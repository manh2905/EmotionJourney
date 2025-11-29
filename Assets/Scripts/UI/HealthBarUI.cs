using UnityEngine;
using UnityEngine.UI;
public class HealthBarUI : MonoBehaviour
{
    public Slider slider;
    public void setMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        // How to minus Health
        //currentHealth -= damage;
        //HealthBarUI.setHealth(currentHealth);
    }
    public void setHealth(int health)
    {
        slider.value = health;
    }
    
}
