using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public GameObject DeathPanel; // holds the panel with UI elements
    public GameObject WinPanel; // same with above but for game winning
    public Text WinText; // the text on the win UI
    private DateTime start = new DateTime(1970, 1, 1); // the start time for the game (set to be when terrain generator is finished), defaulted to unix epoch
    public void Die() // event to die
    {
        PauseScreen.Paused = true; // mark as paused, this allows us to freeze functions (it's reset on start anyways)
        DeathPanel.SetActive(true); // activate the panel
        Wait(100, () => { LIFXManager.ChangeColour(ColorUtility.ToHtmlStringRGB(DeathPanel.GetComponent<Image>().color), 48); }); // set the colour to red after 100ms
    }
    public void Win()
    {
        int time = (int)Math.Round((DateTime.UtcNow - start).TotalSeconds); // save the current time
        PauseScreen.Paused = true; // same as for death; freeze events
        WinPanel.SetActive(true); // show the panel
        LIFXManager.ChangeColour(ColorUtility.ToHtmlStringRGB(WinPanel.GetComponent<Image>().color), 48); // same code (pull green colour)
        if (SessionManager.Authenticated) // if the user is currently logged in
        {
            WinText.text = "saving score".ToUpper(); // tell the user the score is being saved
            WinText.text = SessionManager.SaveScore(time).ToUpper(); // update with result of score save
        }
        else
        {
            WinText.text = "please login to save score".ToUpper(); // tell the user the score isn't being saved
        }
    }
    void Update()
    {
        int delta = (int)Math.Round((DateTime.UtcNow - start).TotalSeconds); // calculate the time difference in seconds
        GetComponent<NotificationManager>().SetBottomText("Current time: " + delta.ToString() + " seconds"); // update the current time
    }
    public void StartTimer()
    {
        start = DateTime.UtcNow; // use utc time for start
    }
    void Start() // runs on start
    {
        DeathPanel.SetActive(false); // mark the panel as inactive
        WinPanel.SetActive(false);
    }
    public void Wait(int ms, Action action)
    {
        StartCoroutine(doWait(ms / 1000f, action)); // thread.sleep causes issues with ui freeze
    }
    IEnumerator doWait(float time, Action callback)
    {
        yield return new WaitForSecondsRealtime(time); // same fix as above
        callback(); // do this once the time is done
    }
}
