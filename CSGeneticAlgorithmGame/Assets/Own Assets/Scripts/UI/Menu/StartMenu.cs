using UnityEngine;

public class StartMenu : MonoBehaviour
{
    void Start()
    {
        LIFXManager.ChangeColour("FFFFFF", 100); // resets the colour to white
        SessionManager.Start(); // start iteration the session manager
        Time.timeScale = 1; // reset timescale
    }
}
