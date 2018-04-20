using UnityEngine;

public class LinkDisplacement : MonoBehaviour
{
    public GameObject Child; // we can use this script to link the displacement of the child object relative to the parent
    public bool X; // toggles for each of the axes
    public bool Y;
    public bool Z;
    float[] selfInitial = { 0, 0, 0 }; // holds the parent's initial position
    float[] childInitial = { 0, 0, 0 }; // holds the child's initial position
    void Start()
    {
        Vector3 selfPos = gameObject.transform.position; // take the vector of the parent transform
        selfInitial[0] = selfPos.x; // assign to the float array
        selfInitial[1] = selfPos.y;
        selfInitial[2] = selfPos.z;
        Vector3 childPos = Child.transform.position; // same as above but for the child
        childInitial[0] = childPos.x;
        childInitial[1] = childPos.y;
        childInitial[2] = childPos.z;
    }
    void Update()
    {
        Vector3 selfPos = gameObject.transform.position; // get the current position
        float[] displacement = { selfInitial[0] - selfPos.x, selfInitial[1] - selfPos.y, selfInitial[2] - selfPos.z }; // gets the position delta from the initial state
        Vector3 newPos = new Vector3(); // create a new vector which will be assigned to the child
        if (X) // only change the vector if X is supposed to be displaced
        {
            newPos.x = childInitial[0] - displacement[0]; // sets the position (note that it doesn't just use the parent's location as this script allows for offsets
        }
        else
        {
            newPos.x = Child.transform.position.x; // otherwise use the child's regular position
        }
        if (Y) // same as above
        {
            newPos.y = childInitial[1] - displacement[1];
        }
        else
        {
            newPos.y = Child.transform.position.y;
        }
        if (Z) // same as above
        {
            newPos.z = childInitial[2] - displacement[2];
        }
        else
        {
            newPos.z = Child.transform.position.z;
        }
        Child.transform.position = newPos; // assign the new position to the child
    }
}
