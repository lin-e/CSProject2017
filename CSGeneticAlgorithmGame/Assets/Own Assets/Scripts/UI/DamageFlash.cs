using UnityEngine;
using UnityEngine.UI;

public class DamageFlash : MonoBehaviour
{
    public Color Colour = new Color(0, 0, 0, 0.1f); // the colour the screen should change to
    public float Speed = 5f; // speed of the flash
    bool damaged = false;
    float currentTime = 0f;
    bool lerping = false;
    void Update()
    {
        if (damaged)
        {
            damaged = false; // sets damaged back to false
            lerping = true; // strats lerping
            GetComponent<Image>().color = Colour; // instantly change colour 
        }
        else if (lerping) // if it is lerping (don't start lerp on this iteration as you don't want the colour to fade yet)
        {
            if (currentTime <= Speed) // while current time is less than speed
            {
                currentTime += Time.deltaTime; // increment the timer by the time since hte last physics update
                GetComponent<Image>().color = Color.Lerp(Colour, Color.clear, currentTime / Speed); // linearly interpolates between the colours
            }
            else
            {
                GetComponent<Image>().color = Color.clear; // clears the flash so that it is no longer visible
                lerping = false; // stops lerping
                currentTime = 0; // resets current time
                LIFXLan.ChangeColour("FFFFFF", 96); // resets the colour to white
            }
        }
    }
    public void Flash() // public method to access damage flash
    {
        damaged = true; // sets to damaged so that it will activate on the next update
        LIFXLan.ChangeColour(ColorUtility.ToHtmlStringRGB(Colour), 48); // smart lighting integration - will flash the selected lights whatever colour is specified in the damage flash
    }
}
