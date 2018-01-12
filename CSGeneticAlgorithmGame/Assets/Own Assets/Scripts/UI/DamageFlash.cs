using UnityEngine;
using UnityEngine.UI;

public class DamageFlash : MonoBehaviour
{
    public Color Colour = new Color(0, 0, 0, 0.1f);
    public float Speed = 5f;
    bool damaged = false;
    float currentTime = 0f;
    bool lerping = false;
    void Update()
    {
        if (damaged)
        {
            damaged = false;
            lerping = true;
            GetComponent<Image>().color = Colour;
        }
        else
        {
            if (lerping)
            {
                if (currentTime <= Speed)
                {
                    currentTime += Time.deltaTime;
                    GetComponent<Image>().color = Color.Lerp(Colour, Color.clear, currentTime / Speed);
                }
                else
                {
                    GetComponent<Image>().color = Color.clear;
                    lerping = false;
                    currentTime = 0;
                    LIFXLan.ChangeColour("FFFFFF", 96);
                }
            }
        }
    }
    public void Flash()
    {
        damaged = true;
        LIFXLan.ChangeColour(ColorUtility.ToHtmlStringRGB(Colour), 48);
    }
}
