using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

public static class SessionManager // by using a static class we have variables that can be accessed from all scenes
{
    public static string EndPoint = "https://project.eugenel.in/api/";
    public static int RenewTime = 2000;
    public static WebRequestFactory Factory = new WebRequestFactory(null, new CookieContainer()); // create a new request manager
    public static string Token = ""; // stores the current active token
    public static bool Running = false; // toggles the iteration
    public static bool Authenticated = false; // if the user has a valid session
    public static Thread ActiveThread = null; // the current thread used for token renewal
    public static string[] Credentials = { "", "" }; // stores the login details
    public static dynamic Authenticate(string user, string pass) // method to authenticate the user
    {
        try // attempt to run the code in the child scope
        {
            string json = "{\"username\":\"" + user + "\",\"md5\":\"" + pass.Hash("md5") + "\"}"; // create the raw json payload
            string response = EndpointRequest("auth.php", json); // use the function to get a very simple request response
            dynamic result = DynamicJson.Deserialize(response); // deserialize the string
            if ((int)result.status == 1) // if the request was successful
            {
                Token = (string)result.content; // set the token to be the response
                Authenticated = true; // mark as authenticated
            }
            else
            {
                Authenticated = false; // remove authentication flag
            }
            return result; // return the decoded content
        }
        catch // if it errors out (most likely due to connection)
        {
            Authenticated = false;
            return DynamicJson.Deserialize("{\"status\":0,\"content\":\"Error connecting to server\"}"); // return a generic error
        }
    }
    public static dynamic Renew() // renew the token, very similar to auth, but instead of using the credentials it uses the token
    {
        try // attempt to run the code in the child scope
        {
            string json = "{\"token\":\"" + Token + "\"}"; // create the raw json payload
            string response = EndpointRequest("renew.php", json); // use the function to get a very simple request response
            dynamic result = DynamicJson.Deserialize(response); // deserialize the string
            if ((int)result.status == 1) // if the request was successful
            {
                Token = (string)result.content; // set the token to be the response
                Authenticated = true;
            }
            else
            {
                Authenticated = false;
            }
            return result; // return the decoded content
        }
        catch // if it errors out (most likely due to connection)
        {
            Authenticated = false;
            return DynamicJson.Deserialize("{\"status\":0,\"content\":\"Error connecting to server\"}"); // return a generic error
        }
    }
    public static string EndpointRequest(string api, string json) // function to clean up code and allow for easier requests to the endpoints
    {
        string payload = "data=" + WebUtility.UrlEncode(json); // encode the payload for upload
        HttpWebRequest auth = Factory.WebRequestPOST( // create a new request from the factory
            EndPoint + api, // set the url to be auth.php
            new string[][] { }, // no headers needed
            Encoding.ASCII.GetBytes(payload), // convert the payload to bytes
            "application/x-www-form-urlencoded" // specify that it is a form (with json payload)
            );
        using (HttpWebResponse response = (HttpWebResponse)auth.GetResponse()) // automatic disposal
        {
            return new StreamReader(response.GetResponseStream()).ReadToEnd(); // read the stream and return
        }
    }
    public static void Start() // runs iterations
    {
        if (Running) // if the program is already running
        {
            return; // don't create a new thread;
        }
        Running = true; // mark running as true
        ActiveThread = new Thread(() => // create new thread
        {
            while (Running) // does this while running
            {
                if (Authenticated) // only runs if the current user is authenticated
                {
                    dynamic result = Renew(); // renew the session
                    if ((int)result.status == 0) // if failed to renew
                    {
                        Debug.Log("SESSION MANAGER [RENEW]: " + (string)result.content); // log to console
                    } 
                }
                Thread.Sleep(RenewTime); // delay
            }
        });
        ActiveThread.Start(); // start the thread
    }
    public static void Stop() // method to stop threads
    {
        Running = false; // flags as not running
        ActiveThread = null; // marks the thread as null
    }
}