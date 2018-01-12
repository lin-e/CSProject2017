using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform t = transform.GetChild(i);
            Image img = t.GetComponent<Image>();
            if (img != null)
            {
                loadingBar = img;
                break;
            }
        }
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
            loadingBar.transform.localScale = new Vector3(progress, 1, 1);
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
