using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    public static bool Paused = false; // flag that can be accessed from any part of the game to check state
    public static bool LockCursor = true; // whether the cursor should be locked

    GameObject panel; // stores the panel
    void Start()
    {
        Paused = false; // reset paused state
        panel = gameObject.transform.GetChild(0).gameObject; // gets the only child of this component (which should be the ui)
        panel.SetActive(false); // hide it on start
    }
    void Update() // runs each frame
    {
        if (Paused) // if the game is currently paused or the player is dead
        {
            Cursor.lockState = CursorLockMode.None; // unlock cursor so user can interact with UI
        }
        else
        {
            if (LockCursor) // if cursor lock is enabled
            {
                Cursor.lockState = CursorLockMode.Locked; // lock the cursor
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape)) // check if key is down
        {
            if (Paused) // if currently paused
            {
                Unpause(); // run unpause code
            }
            else
            {
                Pause(); // same for pause
            }
        }
    }
    public void Unpause() // must be public to be used by button
    {
        Paused = false;
        panel.SetActive(false); // hide the panel
        Time.timeScale = 1f; // set to regular speed
    }
    void Pause()
    {
        Paused = true;
        panel.SetActive(true); // make the panel visible
        Time.timeScale = 0f; // stop time
    }
    public void Quit() // exit the game
    {
        Application.Quit(); // quit
    }
}