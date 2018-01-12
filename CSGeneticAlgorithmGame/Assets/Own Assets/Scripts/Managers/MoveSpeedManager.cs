using UnityEngine;

public class MoveSpeedManager : MonoBehaviour
{
    public float LerpTime = 0.05f;
    float speed;
    float oldSpeed;
    float desiredSpeed;
    bool lerping;
    float currentTime;
    public float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            if (!lerping)
            {
                lerping = true;
                oldSpeed = speed;
                desiredSpeed = value;
            }
        }
    }
    void Update()
    {
        if (lerping)
        {
            if (currentTime <= LerpTime)
            {
                currentTime += Time.deltaTime;
                speed = Mathf.Lerp(oldSpeed, desiredSpeed, currentTime / LerpTime);
            }
            else
            {
                speed = desiredSpeed;
                lerping = false;
                currentTime = 0;
            }
        }
    }
}