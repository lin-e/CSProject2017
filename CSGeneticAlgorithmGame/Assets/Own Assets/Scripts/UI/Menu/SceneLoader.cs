using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Image LoadingBar;
    public float LerpTime = 0.05f;

    float progress = 0;
    bool progressLerping = false;
    float progressLerp;
    float oldProgressLerp;
    float desiredProgressLerp;
    float currentProgressTime;
    // BRACKEYS
    public void Start()
    {
        LIFXLan.Initialise();
        LIFXLan.ChangeColour("ffffff", 512);
    }
    public void LoadLevel(string name)
    {
        StartCoroutine(LoadAsync(name));
    }
    void OnGUI()
    {
        SetProgressBar(progress);
        if (progressLerping)
        {
            if (currentProgressTime <= LerpTime)
            {
                currentProgressTime += Time.deltaTime;
                progressLerp = Mathf.Lerp(oldProgressLerp, desiredProgressLerp, currentProgressTime / LerpTime);
            }
            else
            {
                progressLerp = desiredProgressLerp;
                progressLerping = false;
                currentProgressTime = 0;
            }
            LoadingBar.transform.localScale = new Vector3(progress, 1, 1);
        }
    }
    void SetProgressBar(float progress)
    {
        if (!progressLerping)
        {
            if (progress != progressLerp)
            {
                progressLerping = true;
                oldProgressLerp = progressLerp;
                desiredProgressLerp = progress;
            }
        }
    }
    IEnumerator LoadAsync(string name)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(name);
        while (!op.isDone)
        {
            progress = Mathf.Clamp(op.progress / 0.9f, 0, 1);
            yield return null;
        }
    }
}