using UnityEngine;

// this script is used exclusively for the third person camera
public class SmoothFollow : MonoBehaviour
{
    public Transform PositionTarget; // this is normally a position relative to the player which the camera is aiming to be at
    public Transform ViewTarget; // this is normally the player (or whatever the camera should be focused on
    public float LerpTime; // time time it takes to interpolate between the two points
    void FixedUpdate() // does this on physics update
    {
        transform.position = Vector3.Lerp(transform.position, PositionTarget.position, LerpTime); // lerp to the desired position
        transform.LookAt(ViewTarget); // always look at the target
    }
    public void SetTargetRotation(Vector3 euler) // public method to override local rotation
    {
        PositionTarget.localRotation = Quaternion.Euler(euler); // set the target's rotation
    }
}
