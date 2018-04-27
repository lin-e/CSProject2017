using System.Net;
using System.IO;
using System.Text;

public static class SessionManager // by using a static class we have variables that can be accessed from all scenes
{
    public static string EndPoint = "https://project.eugenel.in/api/";
    public static WebRequestFactory Factory = new WebRequestFactory(null, new CookieContainer()); // create a new request manager
    public static string Token = ""; // stores the current active token
    public static dynamic Authenticate(string user, string pass) // method to authenticate the user
    {
        try // attempt to run the code in the child scope
        {
            string payload = "data=" + WebUtility.UrlEncode("{\"username\":\"" + user + "\",\"md5\":\"" + pass.Hash("md5") + "\"}"); // encode the payload for upload
            HttpWebRequest auth = Factory.WebRequestPOST( // create a new request from the factory
                EndPoint + "auth.php", // set the url to be auth.php
                new string[][] { }, // no headers needed
                Encoding.ASCII.GetBytes(payload), // convert the payload to bytes
                "application/x-www-form-urlencoded" // specify that it is a form (with json payload)
                );
            using (HttpWebResponse response = (HttpWebResponse)auth.GetResponse()) // automatic disposal
            {
                dynamic result = DynamicJson.Deserialize(new StreamReader(response.GetResponseStream()).ReadToEnd()); // deserialize the string after reading it from stream
                if ((int)result.status == 1) // if the request was successful
                {
                    Token = (string)result.content; // set the token to be the response
                }
                return result; // return the decoded content
            }
        }
        catch // if it errors out (most likely due to connection)
        {
            return DynamicJson.Deserialize("{\"status\":0,\"content\":\"Error connecting to server\"}"); // return a generic error
        }
    }
}