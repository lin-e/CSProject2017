using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LIFXManager : MonoBehaviour
{
    public Text UpdateText; // holds the text to update
    public GameObject Button; // holds the main button
    public GameObject LeftButton; // holds left button
    public GameObject RightButton; // same for right
    public static bool Paired = false; // flag for pairing state, defaults to false
    public static List<BulbOptions> Bulbs = new List<BulbOptions>(); // list of bulbs

    int currentIndex = 0; // marks the current index
    void Start()
    {
        UpdateText.text = (Paired) ? "Pairing has already been completed! Please restart the game if you'd like to make changes." : "Smart lighting is an experimental feature to add immersion. Currently only supports LIFX bulbs. Press start to begin!"; // simple inline conditional to decide the message to display (depends on pairing state)
        if (Paired) // if already paired
        {
            Button.SetActive(false); // hide the button
        }
    }
    void BulbDisplay()
    {
        if (currentIndex >= Bulbs.Count) // if we're at the end of the array, or somehow out of bounds
        {
            UpdateText.text = "Pairing process completed!";
            LeftButton.SetActive(false); // disable the two buttons
            RightButton.SetActive(false);
        }
        else
        {
            BulbOptions current = Bulbs[currentIndex]; // fetch the current bulb
            UpdateText.text = "Bulb " + (currentIndex + 1).ToString() + "/" + Bulbs.Count.ToString() // set the text with basic info about the bulb
                + Environment.NewLine + current.Bulb.Label + " - " + current.Bulb.IP;
        }
    }
    public void MainButtonClick()
    {
        UpdateText.text = "Scanning for bulbs, the application may become unresponsive during this period."; // updates the text with a message
        LIFXBulb[] listed = LIFXLan.ListBulbs(); // gets the bulbs from the api
        if (listed.Length > 0) // if there are bulbs detected
        {
            foreach (LIFXBulb single in listed) // iterate through each bulb
            {
                Bulbs.Add(new BulbOptions(single, false)); // add to list
            }
            Paired = true; // mark as paired
            BulbDisplay(); // update display
            Button.SetActive(false); // enable or disable buttons
            LeftButton.SetActive(true);
            RightButton.SetActive(true);
        }
        else // otherwise if none detected
        {
            UpdateText.text = "No bulbs detected! Please ensure you're on the same network as the bulbs."; // give an error message to the user
        }
    }
    public void LeftButtonClick() // enable
    {
        BulbOptions current = Bulbs[currentIndex]; // fetch the current bulb
        current.Enabled = true; // enable said bulb
        RightButtonClick(); // it needs to do  this anyways to go to the next item
    }
    public void RightButtonClick() // skip
    {
        currentIndex++; // increment index
        BulbDisplay(); // update display
    }
    public static void ChangeColour(string hex, int time) // static function to change colour
    {
        List<LIFXBulb> enabled = new List<LIFXBulb>(); // list of enabled bulbs to pass into lifx api
        foreach (BulbOptions b in Bulbs) // iterates through each of the loaded bulbs
        {
            if (b.Enabled) // if the bulb is enabled
            {
                enabled.Add(b.Bulb); // add it to the list
            }
        }
        Debug.Log(enabled.Count);
        LIFXLan.ChangeColour(hex, time, enabled.ToArray()); // change the colour of the enabled bulbs
    }
}
public class BulbOptions
{
    public LIFXBulb Bulb; // the bulb to control
    public bool Enabled; // the bulb's state
    public BulbOptions(LIFXBulb b, bool e) // simple constructor
    {
        Bulb = b; // set based on constructor
        Enabled = e;
    }
}
