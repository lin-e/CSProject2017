using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float WalkSpeed = 6;
    public float WalkFOV = 60;
    public float RunSpeed = 8;
    public float RunFOV = 80;
    public float CrouchSpeed = 3;
    public float CrouchFOV = 55;
    public float LookSensitivity = 5;
    public float JumpForce = 6;
    public GroundMode GroundCheckMode = GroundMode.Union;
    public int MultiThreshold = 2;
    public bool LockCursor = true;

    public Text XCoordinate;
    public Text ZCoordinate;

    [HideInInspector]
    public Vector3 Velocity = Vector3.zero;

    Vector3 rotation = Vector3.zero;
    Vector3 cameraRotation = Vector3.zero;

    bool collisionJumpFlag;
    float runTime = 0f;
    MovementModifier movementModifier = MovementModifier.Walk;

    MoveSpeedManager speedManager;
    PlayerHealth statManager;
    CameraManager cameraManager;
    Rigidbody body;
    void Start()
    {
        speedManager = GetComponent<MoveSpeedManager>();
        statManager = GetComponent<PlayerHealth>();
        cameraManager = GetComponent<CameraManager>();
        body = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (LockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsGrounded())
            {
                body.velocity = new Vector3(body.velocity.x, JumpForce, body.velocity.z);
            }
            else
            {
                Debug.Log("DEBUG: Player not grounded");
            }
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            movementModifier = MovementModifier.Crouch;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            if (statManager.CurrentEnergy >= ((movementModifier == MovementModifier.Run) ? 5 : 20))
            {
                movementModifier = MovementModifier.Run;
            }
            else
            {
                movementModifier = MovementModifier.Walk;
            }
        }
        else
        {
            movementModifier = MovementModifier.Walk;
        }
        setModifierSpeed();
        cameraManager.FOV = getFOV();
        // MODIFIED VERSION OF BRACKEY'S.
        float xMotion = Input.GetAxisRaw("Horizontal");
        float zMotion = Input.GetAxisRaw("Vertical");
        float yRotation = Input.GetAxisRaw("Mouse X");
        float xRotation = Input.GetAxisRaw("Mouse Y");
        Vector3 motionHorizontal = transform.right * xMotion;
        Vector3 motionVertical = transform.forward * zMotion;
        Velocity = (motionHorizontal + motionVertical).normalized * getModifierSpeed();
        rotation = new Vector3(0, yRotation, 0) * LookSensitivity;
        cameraRotation = new Vector3(xRotation, 0, 0) * LookSensitivity;
        if (IsMoving() && movementModifier == MovementModifier.Run)
        {
            runTime += Time.deltaTime;
            if (runTime >= 0.5f)
            {
                statManager.DecreaseEnergy(1f);
                runTime = 0f;
            }
        }
        if (movementModifier == MovementModifier.Crouch)
        {
            GetComponent<Rigidbody>().mass = 5;
        }
        else
        {
            GetComponent<Rigidbody>().mass = 1;
        }
        XCoordinate.text = "X: " + Mathf.RoundToInt(transform.position.x).ToString();
        ZCoordinate.text = "Z: " + Mathf.RoundToInt(transform.position.z).ToString();
    }
    void OnCollisionEnter(Collision col)
    {
        collisionJumpFlag = true;
    }
    void FixedUpdate()
    {
        applyMotion();
        applyRotation();
    }
    public bool IsMoving()
    {
        return Velocity.magnitude > Mathf.Pow(10, -6); // because floating point.
    }
    public bool IsGrounded(GroundMode _checkMode = GroundMode.Unknown)
    {
        GroundMode checkMode = _checkMode;
        if (checkMode == GroundMode.Unknown)
        {
            checkMode = GroundCheckMode;
        }
        GroundMode[] checkModes = { GroundMode.CollisionToggle, GroundMode.Raycast, GroundMode.Velocity };
        switch (checkMode)
        {
            case GroundMode.Raycast:
                {
                    float negatedRadian = (transform.rotation.y * Mathf.PI) / -180;
                    Collider col = GetComponent<Collider>();
                    float[] posAndNeg = { -1, 1 }; // i hate writing repetitive code.
                    Vector3 bounds = col.bounds.extents;
                    if (Physics.Raycast(transform.position, -Vector3.up, bounds.y + 0.1f))
                    {
                        return true;
                    }
                    foreach (float x in posAndNeg)
                    {
                        foreach (float z in posAndNeg)
                        {
                            Vector3 newPoint = new Vector3(x * (bounds.x - 0.001f), 0, z * (bounds.z - 0.001f));
                            Vector3 rotated = matrixRotate(negatedRadian, newPoint);
                            rotated = rotated + transform.position;
                            if (Physics.Raycast(rotated, -Vector3.up, bounds.y + 0.1f))
                            {
                                return true;
                            }
                        }
                    }
                }
                break;
            case GroundMode.CollisionToggle:
                {
                    if (collisionJumpFlag)
                    {
                        collisionJumpFlag = false;
                        return true;
                    }
                }
                break;
            case GroundMode.Velocity:
                {
                    return Mathf.Abs(body.velocity.y) < Mathf.Pow(10, -5);
                }
            case GroundMode.Union:
                {
                    foreach (GroundMode singleMode in checkModes)
                    {
                        if (IsGrounded(singleMode))
                        {
                            return true;
                        }
                    }
                }
                break;
            case GroundMode.Intersect:
                {
                    foreach (GroundMode singleMode in checkModes)
                    {
                        if (!IsGrounded(singleMode))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            case GroundMode.Multi:
                {
                    int count = 0;
                    foreach (GroundMode singleMode in checkModes)
                    {
                        if (IsGrounded(singleMode))
                        {
                            count++;
                        }
                    }
                    return count >= MultiThreshold;
                }
        }
        return false;
    }
    void applyMotion()
    {
        if (Velocity != Vector3.zero)
        {
            body.MovePosition(body.position + Velocity * Time.fixedDeltaTime);
        }
    }
    void applyRotation()
    {
        body.MoveRotation(body.rotation * Quaternion.Euler(rotation));
        Camera active = cameraManager.Active;
        if (cameraManager.ActiveIndex == 0)
        {
            active.transform.Rotate(-cameraRotation);
            Vector3 currentCamera = active.transform.localRotation.eulerAngles;
            if (currentCamera.x < 180 && currentCamera.x > 85)
            {
                currentCamera.x = 85;
            }
            if (currentCamera.x > 180 && currentCamera.x < 275)
            {
                currentCamera.x = 275;
            }
            currentCamera.y = 0;
            currentCamera.z = 0;
            active.transform.localRotation = Quaternion.Euler(currentCamera);
        }
        else if (cameraManager.ActiveIndex == 1) // just in case more cameras are added
        {
            Transform follow = active.gameObject.GetComponent<SmoothFollow>().PositionTarget;
            follow.Rotate(-cameraRotation);
            float localX = follow.localEulerAngles.x;
            float clamp = 15;
            if (localX < 90 && localX > clamp)
            {
                localX = clamp;
            }
            else if (localX > 270 && localX < 360 - clamp)
            {
                localX = 360 - clamp;
            }
            follow.localEulerAngles = new Vector3(localX, 0, 0);
            Vector3 newPos = new Vector3(follow.localPosition.x, 10 * Mathf.Sin(follow.localEulerAngles.x * Mathf.Deg2Rad), -10 * Mathf.Cos(follow.localEulerAngles.x * Mathf.Deg2Rad));
            follow.transform.localPosition = newPos;
        }
    }
    void setModifierSpeed()
    {
        switch (movementModifier)
        {
            case MovementModifier.Walk:
                speedManager.Speed = WalkSpeed;
                break;
            case MovementModifier.Run:
                speedManager.Speed = RunSpeed;
                break;
            case MovementModifier.Crouch:
                speedManager.Speed = CrouchSpeed;
                break;
        }
    }
    Vector3 matrixRotate(float r, Vector3 v)
    {
        Vector3 result = new Vector3(0, v.y, 0);
        float s = Mathf.Sin(r);
        float c = Mathf.Cos(r);
        result.x = (c * v.x) + (-s * v.z);
        result.z = (s * v.x) + (c * v.z);
        return result;
    }
    float getModifierSpeed()
    {
        return speedManager.Speed;
    }
    float getFOV()
    {
        if (!IsMoving())
        {
            return WalkFOV;
        }
        switch (movementModifier)
        {
            case MovementModifier.Walk:
                return WalkFOV;
            case MovementModifier.Run:
                return RunFOV;
            case MovementModifier.Crouch:
                return CrouchFOV;
        }
        return 0f;
    }
}
public enum MovementModifier
{
    Walk,
    Run,
    Crouch
}
public enum GroundMode
{
    Velocity,
    Raycast,
    CollisionToggle,
    Unknown,
    Union,
    Intersect,
    Multi
}
