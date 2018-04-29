using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoginMenu : MonoBehaviour
{
    public InputField UsernameField; // contains the username
    public InputField PasswordField; // contains the password
    public Text UpdateField; // contains the message to update
    public InputField[] TextFields; // contains the input boxes

    private int selectedIndex = 0; // contains the currently selected index
    void Start()
    {
        if (SessionManager.Authenticated) // if currently authenticated
        {
            UpdateField.text = ("signed in as " + SessionManager.Credentials[0]).ToUpper(); // displays the currently authenticated user
            UsernameField.text = SessionManager.Credentials[0]; // set username
            PasswordField.text = SessionManager.Credentials[1]; // set password
        }
        TextFields[0].Select(); // selects the first text box
    }
    void Update() // this mainly allows for tabbing between input fields
    {
        for (int i = 0; i < TextFields.Length; i++) // iterates through each item in the array, using an index as it will be used later
        {
            if (TextFields[i].isFocused) // checks if the element is focused
            {
                selectedIndex = i; // set the currently selected index to the index of the current item
                break; // exit the iteration, useful if we decide to add more elements
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab)) // if we pressed the tab key
        {
            selectedIndex = (selectedIndex + 1) % TextFields.Length; // increment the selected index by 1, and use the modulo operator to wrap around the array
            TextFields[selectedIndex].Select(); // select the item
        }
    }
    public void Login()
    {
        UpdateField.text = "signing in".ToUpper(); // change the text field
        StartCoroutine(LoginWorker()); // do this in the background to not lock the UI
    }
    IEnumerator LoginWorker()
    {
        bool done = false; // flag for being done
        while (!done) // do this when not complete
        {
            yield return null; // resume after next update
            dynamic result = SessionManager.Authenticate(UsernameField.text.ToLower(), PasswordField.text); // logs in with the session manager
            string newMessage = ""; // the new notification message
            if ((int)result.status == 1) // if the login was a success
            {
                newMessage = "successfully logged in"; // set to a success message
                SessionManager.Credentials = new string[] { UsernameField.text.ToLower(), PasswordField.text }; // store the credentials
            }
            else
            {
                newMessage = (string)result.content; // use the resposne
            }
            UpdateField.text = newMessage.ToUpper(); // change the text field
            done = true; // flag as complete
        }
    }
}
