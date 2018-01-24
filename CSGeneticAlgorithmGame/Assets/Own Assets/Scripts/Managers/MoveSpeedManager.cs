using UnityEngine;

public class MoveSpeedManager : MonoBehaviour
{
    public float LerpTime = 0.05f; // time to interpolate between speeds
    float speed; // current speed
    float oldSpeed; // initial speed
    float desiredSpeed; // target speed
    bool lerping; // toggle for lerp
    float currentTime; // current lerp time
    public float Speed // use an accessor to cleanly access code
    {
        get
        {
            return speed; // return the current speed (can be mid-lerp)
        }
        set
        {
            if (!lerping) // if it isn't lerping, then start lerping
            {
                lerping = true; // mark as lerping
                oldSpeed = speed; // set the old speed to the current speed
                desiredSpeed = value; // set the desired value to the input
            }
        }
    }
    void Update()
    {
        if (lerping) // if it is lerping
        {
            if (currentTime <= LerpTime) // if the current time is still less than the lerp time
            {
                currentTime += Time.deltaTime; // increment time by time since last update
                speed = Mathf.Lerp(oldSpeed, desiredSpeed, currentTime / LerpTime); // interpolate between old speed and desired
            }
            else
            {
                speed = desiredSpeed; // set speed as desired speed
                lerping = false; // stop lerping
                currentTime = 0; // reset time
            }
        }
    }
}