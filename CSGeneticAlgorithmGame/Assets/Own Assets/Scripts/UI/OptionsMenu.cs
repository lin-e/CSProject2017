using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class OptionsMenu : MonoBehaviour
{
    public void WebsiteClicked() // event for when website button is pressed
    {
        try // attempt this
        {
            Process.Start("https://project.eugenel.in/"); // launch website in default browser
        } catch { }
    }
    public void Quit() // exit the game
    {
        Application.Quit(); // quit
    }
}
