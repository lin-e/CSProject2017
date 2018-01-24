using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FPSCounter : MonoBehaviour
{
    //https://gist.github.com/mstevenson/5103365
    // full credit goes to the above, his code is slightly modified to work with my hud
    public Text text;
    float count;
    IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            count = 1 / Time.deltaTime;
            text.text = Mathf.Round(count).ToString() + " FPS";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
