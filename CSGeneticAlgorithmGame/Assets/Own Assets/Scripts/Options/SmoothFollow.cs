using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public Transform PositionTarget;
    public Transform ViewTarget;
    public float LerpTime;
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, PositionTarget.position, LerpTime);
        transform.LookAt(ViewTarget);
    }
    public void SetTargetRotation(Vector3 euler)
    {
        PositionTarget.localRotation = Quaternion.Euler(euler);
    }
}
