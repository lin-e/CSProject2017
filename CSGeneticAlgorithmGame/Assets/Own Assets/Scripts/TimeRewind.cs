using UnityEngine;
using System.Collections.Generic;

public class TimeRewind : MonoBehaviour
{
    public float MaxTime = 10;
    public bool PreserveVelocity = false;
    bool rewinding = false;
    List<PositionInfo> data;
    Rigidbody body;
    void Start()
    {
        data = new List<PositionInfo>();
        body = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartRewind();
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            StopRewind();
        }
    }
    void FixedUpdate()
    {
        if (rewinding)
        {
            if (data.Count > 0)
            {
                PositionInfo point = data[data.Count - 1];
                transform.position = point.Position;
                transform.rotation = point.Rotation;
                if (PreserveVelocity)
                {
                    body.velocity = point.Velocity;
                    body.angularVelocity = point.AngularVelocity;
                }
                data.RemoveAt(data.Count - 1);
            }
            else
            {
                StopRewind();
            }
        }
        else
        {
            if (data.Count > Mathf.RoundToInt(MaxTime / Time.fixedDeltaTime))
            {
                data.RemoveAt(0);
            }
            data.Add(new PositionInfo(transform.position, transform.rotation, body.velocity, body.angularVelocity));
        }
    }
    public void StartRewind()
    {
        if (!PreserveVelocity)
        {
            body.isKinematic = true;
        }
        rewinding = true;
    }
    public void StopRewind()
    {
        if (!PreserveVelocity)
        {
            body.isKinematic = false;
        }
        rewinding = false;
    }
    public void ToggleRewind()
    {
        if (rewinding)
        {
            StopRewind();
        }
        else
        {
            StartRewind();
        }
    }
    public class PositionInfo
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public Vector3 AngularVelocity;
        public PositionInfo(Vector3 pos, Quaternion rot, Vector3 vel, Vector3 angleVel)
        {
            Position = pos;
            Rotation = rot;
            Velocity = vel;
            AngularVelocity = angleVel;
        }
    }
}
