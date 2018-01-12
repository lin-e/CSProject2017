using UnityEngine;

public class LinkDisplacement : MonoBehaviour
{
    public GameObject Child;
    public bool X;
    public bool Y;
    public bool Z;
    float[] selfInitial = { 0, 0, 0 };
    float[] childInitial = { 0, 0, 0 };
    void Start()
    {
        Vector3 selfPos = gameObject.transform.position;
        selfInitial[0] = selfPos.x;
        selfInitial[1] = selfPos.y;
        selfInitial[2] = selfPos.z;
        Vector3 childPos = Child.transform.position;
        childInitial[0] = childPos.x;
        childInitial[1] = childPos.y;
        childInitial[2] = childPos.z;
    }
    void Update()
    {
        Vector3 selfPos = gameObject.transform.position;
        float[] displacement = { selfInitial[0] - selfPos.x, selfInitial[1] - selfPos.y, selfInitial[2] - selfPos.z };
        Vector3 newPos = new Vector3();
        if (X)
        {
            newPos.x = childInitial[0] - displacement[0];
        }
        else
        {
            newPos.x = Child.transform.position.x;
        }
        if (Y)
        {
            newPos.y = childInitial[1] - displacement[1];
        }
        else
        {
            newPos.y = Child.transform.position.y;
        }
        if (Z)
        {
            newPos.z = childInitial[2] - displacement[2];
        }
        else
        {
            newPos.z = Child.transform.position.z;
        }
        Child.transform.position = newPos;
    }
}
