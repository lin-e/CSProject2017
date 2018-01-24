using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour
{
    public float LerpTime = 0.05f;

    float progress = 0;
    bool progressLerping = false;
    float progressLerp;
    float oldProgressLerp;
    float desiredProgressLerp;
    float currentProgressTime;
    Image loadingBar;
    public void Start()
    {
        for (int i = 0; i < transform.childCount; i++) // iterate through each child object
        {
            Transform t = transform.GetChild(i); // get the object
            Image img = t.GetComponent<Image>(); // get the image property of the object
            if (img != null) // check if the image isn't null
            {
                loadingBar = img; // set the property's loading bar to the image
                break; // exit from the loop (we've found the loading bar)
            }
        }
    }
    public void LoadLevel(string name) // start loading the level
    {
        StartCoroutine(LoadAsync(name)); // do this in parallel
    }
    void OnGUI() // on each UI update
    {
        SetProgressBar(progress); // start lerp (is ignored on subsequent UI loops as it is flagged as lerping)
        if (progressLerping) // if it is lerping
        {
            if (currentProgressTime <= LerpTime) // check that the time is less than the time specified
            {
                currentProgressTime += Time.deltaTime; // increment by time since last update
                progressLerp = Mathf.Lerp(oldProgressLerp, desiredProgressLerp, currentProgressTime / LerpTime); // set the progress
            }
            else
            {
                progressLerp = desiredProgressLerp; // lerp to desired progress
                progressLerping = false; // stop lerping
                currentProgressTime = 0; // reset lerp time
            }
            loadingBar.transform.localScale = new Vector3(progressLerp, 1, 1); // set scale to the lerp amount (using a layer anchored to the left) 
        }
    }
    void SetProgressBar(float progress) // set the progress bar if it has changed
    {
        if (!progressLerping) // only do this on lerp
        {
            if (progress != progressLerp) // if it isn't equal
            {
                progressLerping = true; // mark as lerping
                oldProgressLerp = progressLerp; // set the initial progress lerp to the current one
                desiredProgressLerp = progress; // set the desired to the function parameter
            }
        }
    }
    IEnumerator LoadAsync(string name) // load the scene in parallel
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(name); // use unity's scene manager
        while (!op.isDone) // while the operation isn't complete
        {
            progress = Mathf.Clamp(op.progress / 0.9f, 0, 1); // set progress (note that it is divied by .9 as unity has a small issue where it sometimes isn't fully loaded
            yield return null; // async return
        }
    }
}
