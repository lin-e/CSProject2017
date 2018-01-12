using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public Text Bottom;
    public void SetBottomText(string s)
    {
        Bottom.text = s;
    }
}
