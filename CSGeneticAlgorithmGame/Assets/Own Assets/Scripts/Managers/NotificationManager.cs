using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public Text Bottom; // bottom notification
    public void SetBottomText(string s) // public method to set text
    {
        Bottom.text = s; // changes text
    }
}
