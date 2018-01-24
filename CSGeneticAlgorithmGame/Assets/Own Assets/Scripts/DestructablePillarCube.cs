using UnityEngine;
using System.Collections.Generic;

// this script was created for debugging collisions and forces
public class DestructablePillarCube : MonoBehaviour
{
    public GameObject prefab; // the object to clone
    public int X = 4; // use the following fields to specify how many of each object should be cloned in any direction
    public int Y = 8;
    public int Z = 4;
    public float Size = 0.5f; // size of the object (this is designed specifically for cubes)
    List<GameObject> objs; // list of all the objects in the body
    void Start()
    {
        objs = new List<GameObject>(); // declares the list
        float[] offset = { (X / 2f) * Size, Size / -2f , (Z / 2f) * Size }; // calculate the offset
        for (int x = 0; x < X; x++) // iterate through each x
        {
            for (int y = 0; y < Y; y++) // same as above for y
            {
                for (int z = 0; z < Z; z++) // same as above for z
                {
                    GameObject g = Instantiate(prefab, transform) as GameObject; // create a new gameobject
                    g.GetComponent<BoxCollider>().enabled = false; // enables the collider
                    g.GetComponent<Rigidbody>().isKinematic = true; // enables kinematic
                    objs.Add(g); // adds he object to the list
                    g.transform.localPosition = new Vector3((x * Size) - offset[0], (y * Size) - offset[1], (z  * Size) - offset[2]); // changes the position relative to the parent
                }
            }
        }
        foreach (GameObject g in objs) // do the following to each item in the array (could also iterate through children objects)
        {
            g.GetComponent<BoxCollider>().enabled = true; // enable box collider
            g.GetComponent<Rigidbody>().isKinematic = false; // disables kinematic so that it can be influenced by unity physics
        }
    }
}