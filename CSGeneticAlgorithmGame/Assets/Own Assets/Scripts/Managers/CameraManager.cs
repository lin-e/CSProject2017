using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera FirstPerson; // let = 0
    public Camera ThirdPerson; // let = 1
    public KeyCode CameraToggleKey = KeyCode.O; // the key to switch between cameras
    public float LerpTime = 0.05f; // lerp time for FOV kick

    [HideInInspector] // do not show this in the editor
    public int ActiveIndex; // we need this public, but the user shouldn't modify this
    
    float fov; // current fov
    float oldFov; // old fov (before set)
    float desiredFov; // new fov (what it's set to)
    bool lerping; // whether it is lerping
    float currentTime; // current time since lerp started

    public float FOV // we can use accessors to clean up this method
    {
        get
        {
            return fov; // return the current fov (might be mid lerp)
        }
        set
        {
            if (!lerping) // if it's already lerping, ignore any sets
            {
                lerping = true; // start lerping
                oldFov = fov; // set the old fov to the current fov
                desiredFov = value; // set the desired to whatever the code set it as
            }
        }
    }
    public Camera Active // get the active camera
    {
        get
        {
            switch (ActiveIndex) // return the current camera, was debating whether to have more cameras, so it's set to a switch
            {
                case 0:
                    return FirstPerson;
                case 1:
                    return ThirdPerson;
                default:
                    return null;
            }
        }
    }
    void Start() // on start
    {
        FirstPerson.enabled = true; // default as first person
        ThirdPerson.enabled = false;
        ActiveIndex = 0;
    }
    void Update() // do this each physics update
    {
        if (Input.GetKeyDown(CameraToggleKey)) // if the key is pressed
        {
            if (ActiveIndex == 0) // change index
            {
                ActiveIndex = 1;
                FirstPerson.enabled = false;
                ThirdPerson.enabled = true;
            }
            else
            {
                ActiveIndex = 0;
                FirstPerson.enabled = true;
                ThirdPerson.enabled = false;
            }
            Debug.Log("DEBUG: Camera switched to '" + Active.name + "'"); // log to console
        }
        if (lerping) // if it's mid lerp
        {
            if (currentTime <= LerpTime) // if the elapsed time is below the lerp time
            {
                currentTime += Time.deltaTime; // add time elapsed
                fov = Mathf.Lerp(oldFov, desiredFov, currentTime / LerpTime); // lerp (linear interpolation) fov between the two points based on time
            }
            else
            {
                fov = desiredFov; // if the lerp time has passed, set it to the max value
                lerping = false; // stop lerp code
                currentTime = 0; // reset time
            }
            FirstPerson.fieldOfView = fov; // set fov for both
            ThirdPerson.fieldOfView = fov;
        }
    }
}