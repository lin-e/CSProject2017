using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

using System.Diagnostics;

// tree generation function
public class TreeGenerator : MonoBehaviour
{
    public float BranchChance = 0.3f; // the probability that it will spawn a branch
    public int InitialLength = 6; // the length of the master trunk
    public float UpwardsBias = 0.25f; // the bias for it to go upward (hence branches will point up)
    public bool DebugMode = false; // debugging will allow me to regenerate the trees with a key

    PolyhedronBuilder builder; // the custom polyhedron builder allows me to use cinema4d style method to build the tree (useful because i have some experience with C4D)
    MeshFilter filter; // the actual mesh filter
    MeshCollider collider; // the collider
    System.Random prng; // the random generator
    Mesh m; // mesh to edit
    public void Start() // runs at the object creation
    {
        Generate(new System.Random()); // create with new random
    }
    public void StartGenerate() // start generation (manually)
    {
        Generate(null); // generate without creating a new random
    }
    void Update() // runs on update, only for debugging
    {
        if (DebugMode)
        {
            if (Input.GetKeyDown(KeyCode.T)) // if t is pressed
            {
                UnityEngine.Debug.ClearDeveloperConsole(); // clear the console
                StartGenerate(); // generate
                Apply(); // apply
            }
        }
    }
    void FixedUpdate() // runs on physics update
    {
        if (DebugMode) // only if in debug mode
        {
            // rotate based on direction pressed
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(new Vector3(0, 2, 0));
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(new Vector3(0, -2, 0));
            }
        }
    }
    public void AddBranch(BranchExtrusion branch) // add branch
    {
        List<BranchExtrusion> be = new List<BranchExtrusion>();
        float currentWidth = branch.Scale;
        BuilderPolygon toExtrude = branch.Polygon;
        TreeNode t = branch.Node;
        while (true)
        {
            int childrenCount = t.ChildrenCount();
            Vector3 extrude = new Vector3(0, 0, 0);
            Vector3 rot = new Vector3(0, 1, 0).RotateTo(toExtrude.Normal);
            if (childrenCount <= 1)
            {
                extrude = new Vector3(prng.NextFloat(-0.45f, 0.45f), prng.NextFloat(1.2f, 2.2f), prng.NextFloat(-0.45f, 0.45f));
                extrude.y = extrude.y * currentWidth;
                extrude.x = extrude.x * Mathf.Pow(currentWidth, 0.3f);
                extrude.z = extrude.z * Mathf.Pow(currentWidth, 0.3f);
            }
            else
            {
                extrude = new Vector3(0, currentWidth, 0);
            }
            extrude = extrude.Rotate(rot.x, rot.y, rot.z, new Vector3(0, 0, 0));
            Vector3[] extrudeBase = toExtrude.ExtrudeRaw(extrude);
            if (childrenCount <= 1)
            {
                float factor = prng.NextFloat(0.7f, 0.9f);
                currentWidth *= factor;
                for (int i = 0; i < extrudeBase.Length; i++)
                {
                    extrudeBase[i].y += UpwardsBias;
                }
                extrudeBase = extrudeBase.QuadScale(factor);
                if (childrenCount == 0)
                {
                    Vector3 norm = extrudeBase.Normal();
                    if (norm.y >= 0)
                    {
                        Vector3 rotateToHorizontal = norm.RotateTo(new Vector3(0, 1, 0));
                        rotateToHorizontal = rotateToHorizontal * prng.NextFloat(0.05f, 0.15f);
                        extrudeBase = extrudeBase.QuadRotate(rotateToHorizontal.x, rotateToHorizontal.y, rotateToHorizontal.z);
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Skipped!");
                    }
                }
                toExtrude.Extrude(extrudeBase);
            }
            else
            {
                toExtrude.Extrude(extrudeBase);
                int polygon = builder.Polygons.Count;
                be.Add(new BranchExtrusion(builder.Polygon(polygon - prng.Next(2, 6)), t.Branch, currentWidth));
            }
            toExtrude = builder.LastPolygon();
            if (childrenCount == 0)
            {
                break;
            }
            else
            {
                t = t.Up;
            }
        }
        foreach (BranchExtrusion extrude in be)
        {
            AddBranch(extrude);
        }
    }
    public void Generate(System.Random random)
    {
        builder = new PolyhedronBuilder();
        filter = GetComponent<MeshFilter>();
        collider = GetComponent<MeshCollider>();
        m = new Mesh();
        if (random != null)
        {
            prng = random;
        }
        TreeData tree = new TreeData(BranchChance, InitialLength, prng);
        TreeNode t = tree.BaseNode;
        float currentWidth = 1;
        builder = new PolyhedronBuilder();
        builder.AddQuad(new Vector3(-0.5f, 0, 0.5f), new Vector3(-0.5f, 0, -0.5f), new Vector3(0.5f, 0, -0.5f), new Vector3(0.5f, 0, 0.5f));
        AddBranch(new BranchExtrusion(builder.Polygon(0), t, currentWidth));
        m = builder.Generate();
    }
    public void Apply() // apply all the meshes
    {
        filter.mesh = m; // set filters
        filter.sharedMesh = m;
        collider.sharedMesh = m; // set collider
    }
}