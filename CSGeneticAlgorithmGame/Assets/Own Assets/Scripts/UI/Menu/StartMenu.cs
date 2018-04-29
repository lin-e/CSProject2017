using UnityEngine;

public class StartMenu : MonoBehaviour
{
    void Start()
    {
        SessionManager.Start(); // start iteration the session manager
        Time.timeScale = 1; // reset timescale
    }
}
