using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // the following values can be changed in the editor (the defaults are what I find to work for my project)
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
    
    public Text XCoordinate; // the XCoordinate which would be displayed on the minimap
    public Text ZCoordinate; // same as above

    [HideInInspector]
    public Vector3 Velocity = Vector3.zero; // used to access the velocity from other objects

    Vector3 rotation = Vector3.zero; // current rotation vector for the user (horizontal)
    Vector3 cameraRotation = Vector3.zero; // current rotation vector for the camera (vertical)

    bool collisionJumpFlag; // a flag toggled in the jump controller
    float runTime = 0f; // timer to check whether the entity's energy should decrease
    MovementModifier movementModifier = MovementModifier.Walk; // current speed modifier

    MoveSpeedManager speedManager; // external manager to manage other parts of the player
    PlayerHealth statManager; // same as above
    CameraManager cameraManager; // same as above
    Rigidbody body; // body of the player to control physics
    void Start()
    {
        speedManager = GetComponent<MoveSpeedManager>(); // gets the components from the user
        statManager = GetComponent<PlayerHealth>(); // same as above
        cameraManager = GetComponent<CameraManager>(); // same as above
        body = GetComponent<Rigidbody>(); // same as above
    }
    void Update()
    {
        if (LockCursor) // prevent the cursor from moving around on screen
        {
            Cursor.lockState = CursorLockMode.Locked; // Unity's lock flag
        }
        if (Input.GetKeyDown(KeyCode.Space)) // check if the space key is pressed (pretty much standard
        {
            if (IsGrounded()) // access the grounding function (only allows jumping if the user is on floor)
            {
                body.velocity = new Vector3(body.velocity.x, JumpForce, body.velocity.z); // add upwards velocity to the user
            }
            else
            {
                Debug.Log("DEBUG: Player not grounded"); // more of a debugging feature
            }
        }
        if (Input.GetKey(KeyCode.LeftControl)) // crouch key
        {
            movementModifier = MovementModifier.Crouch; // sets the user's modifier to crouch
        }
        else if (Input.GetKey(KeyCode.LeftShift)) // sprint key
        {
            if (statManager.CurrentEnergy >= ((movementModifier == MovementModifier.Run) ? 5 : 20)) // if the current user is running, check if the energy is above 5, else check if it is above 20. this prevents an awkward stuttering when the energy is recharged
            {
                movementModifier = MovementModifier.Run; // sets the modifer to run
            }
            else
            {
                movementModifier = MovementModifier.Walk; // sets the modifier to standard (walking)
            }
        }
        else
        {
            movementModifier = MovementModifier.Walk; // otherwise the player is walking normally
        }
        setModifierSpeed(); // change the speed based on modifier
        cameraManager.FOV = getFOV(); // create an fov kick on the camera
        // MODIFIED VERSION OF BRACKEY'S.
        float xMotion = Input.GetAxisRaw("Horizontal"); // get input from keys (default to WASD)
        float zMotion = Input.GetAxisRaw("Vertical"); // same as above
        float yRotation = Input.GetAxisRaw("Mouse X"); // get mouse movement
        float xRotation = Input.GetAxisRaw("Mouse Y"); // same as above
        Vector3 motionHorizontal = transform.right * xMotion; // multiply the right vector by a scalar
        Vector3 motionVertical = transform.forward * zMotion; // same as above
        Velocity = (motionHorizontal + motionVertical).normalized * getModifierSpeed(); // normalise the velocity and multiply by the modifier
        rotation = new Vector3(0, yRotation, 0) * LookSensitivity; // multiply the player rotation by the sensitivity
        cameraRotation = new Vector3(xRotation, 0, 0) * LookSensitivity; // camera rotation is then managed by the camera to add smoothing (in the case of 3rd person camera)
        if (IsMoving() && movementModifier == MovementModifier.Run) // check if the player is running
        {
            runTime += Time.deltaTime; // add time since last update
            if (runTime >= 0.5f) // if it's more than 500 ms
            {
                statManager.DecreaseEnergy(1f); // decrease the energy by 1
                runTime = 0f; // reset time
            }
        }
        if (movementModifier == MovementModifier.Crouch) // if the user is crouching
        {
            GetComponent<Rigidbody>().mass = 5; // increase mass (less response to knockback and recoil)
        }
        else
        {
            GetComponent<Rigidbody>().mass = 1; // set mass to normal
        }
        XCoordinate.text = "X: " + Mathf.RoundToInt(transform.position.x).ToString(); // set minimap coordinates
        ZCoordinate.text = "Z: " + Mathf.RoundToInt(transform.position.z).ToString(); // same as above
    }
    void OnCollisionEnter(Collision col) // event fired when user collides with object
    {
        collisionJumpFlag = true; // set flag to true
    }
    void FixedUpdate() // physics updates
    {
        applyMotion(); // apply motion of the user
        applyRotation(); // apply rotation of user and camera
    }
    public bool IsMoving() // check if the user is moving
    {
        return Velocity.magnitude > Mathf.Pow(10, -6); // because floating point numbers
    }
    public bool IsGrounded(GroundMode _checkMode = GroundMode.Unknown) // check if the user is grounded
    {
        GroundMode checkMode = _checkMode; // custom check mode (depending on accuracy)
        if (checkMode == GroundMode.Unknown) // if none is set, use the one set in the editor
        {
            checkMode = GroundCheckMode;
        }
        GroundMode[] checkModes = { GroundMode.CollisionToggle, GroundMode.Raycast, GroundMode.Velocity }; // all available check modes
        switch (checkMode) // hash table to get mode
        {
            case GroundMode.Raycast: // raycast mode
                {
                    float negatedRadian = (transform.rotation.y * Mathf.PI) / -180; // converts from radians to degrees (the player's horizontal rotation around the y axis)
                    Collider col = GetComponent<Collider>();  // get the collider
                    float[] posAndNeg = { -1, 1 }; // makes code less repetitive
                    Vector3 bounds = col.bounds.extents; // get bounds of the collider
                    if (Physics.Raycast(transform.position, -Vector3.up, bounds.y + 0.1f)) // raycast down from the player's position
                    {
                        return true; // is on ground
                    }
                    foreach (float x in posAndNeg) // -1 or 1
                    {
                        foreach (float z in posAndNeg) // same as above
                        {
                            Vector3 newPoint = new Vector3(x * (bounds.x - 0.001f), 0, z * (bounds.z - 0.001f)); // create a bounding box slightly smaller than the actual box
                            Vector3 rotated = matrixRotate(negatedRadian, newPoint); // rotate the point by the player's rotation
                            rotated = rotated + transform.position; // new position translated relative to the player
                            if (Physics.Raycast(rotated, -Vector3.up, bounds.y + 0.1f)) // raycast down to check relative to the player
                            {
                                return true; // is on ground
                            }
                        }
                    }
                }
                break;
            case GroundMode.CollisionToggle: // toggle mode (very simple)
                {
                    if (collisionJumpFlag) // if it has collided
                    {
                        collisionJumpFlag = false; // reset flag
                        return true; // on ground
                    }
                }
                break;
            case GroundMode.Velocity: // velocity mode
                {
                    return Mathf.Abs(body.velocity.y) < Mathf.Pow(10, -5); // check if the magnitude of the y component is less than 10^-5
                }
            case GroundMode.Union: // union / or mode
                {
                    foreach (GroundMode singleMode in checkModes) // iterate through each mode
                    {
                        if (IsGrounded(singleMode)) // if any of them are marked as true
                        {
                            return true; // on ground
                        }
                    }
                }
                break;
            case GroundMode.Intersect: // intersect / and mode
                {
                    foreach (GroundMode singleMode in checkModes) // iterate through each mode
                    {
                        if (!IsGrounded(singleMode)) // if it's not grounded
                        {
                            return false; // not on ground
                        }
                    }
                    return true; // on ground
                }
            case GroundMode.Multi: // check if there are enough to be over a certain threshold
                {
                    int count = 0; // count the number of successes
                    foreach (GroundMode singleMode in checkModes) // iterate through each mode
                    {
                        if (IsGrounded(singleMode)) // if true
                        {
                            count++; // increment counter
                        }
                    }
                    return count >= MultiThreshold; // return whether the counter is above the threshold
                }
        }
        return false; // return false otherwise
    }
    void applyMotion()
    {
        if (Velocity != Vector3.zero) // only does the code if the velocity is non zero
        {
            body.MovePosition(body.position + Velocity * Time.fixedDeltaTime); // vector sum
        }
    }
    void applyRotation() // apply the rotation
    {
        body.MoveRotation(body.rotation * Quaternion.Euler(rotation)); // multiply by quaternions (way beyond the scope of what is covered in A level mathematics)
        Camera active = cameraManager.Active; // takes the current active camera
        if (cameraManager.ActiveIndex == 0) // check if it's the first camera (first person)
        {
            active.transform.Rotate(-cameraRotation); // inverted rotation
            Vector3 currentCamera = active.transform.localRotation.eulerAngles; // take the euler angles (as it's something I can work with)
            if (currentCamera.x < 180 && currentCamera.x > 85) // clamp it between the range
            {
                currentCamera.x = 85;
            }
            if (currentCamera.x > 180 && currentCamera.x < 275) // same as above
            {
                currentCamera.x = 275;
            }
            currentCamera.y = 0; // set rotations to 0
            currentCamera.z = 0; // same as above
            active.transform.localRotation = Quaternion.Euler(currentCamera); // change back to euler angles
        }
        else if (cameraManager.ActiveIndex == 1) // third person camera
        {
            Transform follow = active.gameObject.GetComponent<SmoothFollow>().PositionTarget; // take the camera's target
            follow.Rotate(-cameraRotation); // rotate it by the negative of the camera rotation
            float localX = follow.localEulerAngles.x; // take the X value of the local rotation
            float clamp = 15; // clamp in range
            if (localX < 90 && localX > clamp) // same as above
            {
                localX = clamp;
            }
            else if (localX > 270 && localX < 360 - clamp) // same as above
            {
                localX = 360 - clamp;
            }
            follow.localEulerAngles = new Vector3(localX, 0, 0); // set the follow rotation
            Vector3 newPos = new Vector3(follow.localPosition.x, 10 * Mathf.Sin(follow.localEulerAngles.x * Mathf.Deg2Rad), -10 * Mathf.Cos(follow.localEulerAngles.x * Mathf.Deg2Rad)); // reposition the camera in an arc around the player
            follow.transform.localPosition = newPos; // set the new position
        }
    }
    void setModifierSpeed() // set the speed in the manager to the corresponding modifier speed
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
    Vector3 matrixRotate(float r, Vector3 v) // basic matrix rotation code
    {
        Vector3 result = new Vector3(0, v.y, 0); // y component should be constant (rotate on XZ plane)
        float s = Mathf.Sin(r); // no point in recalculation trig functions
        float c = Mathf.Cos(r); // same as above
        result.x = (c * v.x) + (-s * v.z); // standard matrix multiplication
        result.z = (s * v.x) + (c * v.z); // same as above
        return result;
    }
    float getModifierSpeed()
    {
        return speedManager.Speed; // linear interpolation used in speed (probably not all that noticable)
    }
    float getFOV() // get the FOV for camera
    {
        if (!IsMoving()) // use standard fov if still
        {
            return WalkFOV;
        }
        switch (movementModifier) // otherwise return the corresponding field of view
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
public enum MovementModifier // holds the movement modifiers
{
    Walk,
    Run,
    Crouch
}
public enum GroundMode // holds all the grounding check modes
{
    Velocity,
    Raycast,
    CollisionToggle,
    Unknown,
    Union,
    Intersect,
    Multi
}
