using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public GameObject DeathPanel; // holds the panel with UI elements
    public void Die() // event to die
    {
        PauseScreen.Paused = true; // mark as paused, this allows us to freeze functions (it's reset on start anyways)
        DeathPanel.SetActive(true); // activate the panel
        Wait(100, () => { LIFXManager.ChangeColour(ColorUtility.ToHtmlStringRGB(DeathPanel.GetComponent<Image>().color), 48); }); // set the colour to red after 100ms
    }
    void Start() // runs on start
    {
        DeathPanel.SetActive(false); // mark the panel as inactive
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
