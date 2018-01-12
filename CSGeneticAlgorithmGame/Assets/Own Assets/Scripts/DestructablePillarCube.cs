using UnityEngine;
using System.Collections.Generic;

public class DestructablePillarCube : MonoBehaviour
{
    public GameObject prefab;
    public int X = 4;
    public int Y = 8;
    public int Z = 4;
    public float Size = 0.5f;
    List<GameObject> objs;
    void Start()
    {
        objs = new List<GameObject>();
        float[] offset = { (X / 2f) * Size, Size / -2f , (Z / 2f) * Size };
        for (int x = 0; x < X; x++)
        {
            for (int y = 0; y < Y; y++)
            {
                for (int z = 0; z < Z; z++)
                {
                    GameObject g = Instantiate(prefab, transform) as GameObject;
                    g.GetComponent<BoxCollider>().enabled = false;
                    g.GetComponent<Rigidbody>().isKinematic = true;
                    objs.Add(g);
                    g.transform.localPosition = new Vector3((x * Size) - offset[0], (y * Size) - offset[1], (z  * Size) - offset[2]);
                }
            }
        }
        foreach (GameObject g in objs)
        {
            g.GetComponent<BoxCollider>().enabled = true;
            g.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}