using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.Diagnostics;

// tree generation function
public class TreeGenerator : MonoBehaviour
{
    public float BranchChance = 0.3f; // the probability that it will spawn a branch
    public int InitialLength = 6; // the length of the master trunk
    public float UpwardsBias = 0.25f; // the bias for it to go upward (hence branches will point up)
    public bool DebugMode = false; // debugging will allow me to regenerate the trees with a key

    PolyhedronBuilder builder; // the custom polyhedron builder allows me to use cinema4d style methods to build the tree (useful because i have some experience with C4D)
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
            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.Rotate(new Vector3(2, 0, 0));
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.Rotate(new Vector3(-2, 0, 0));
            }
        }
    }
    public void AddBranch(BranchExtrusion branch) // recursively add branches
    {
        List<BranchExtrusion> be = new List<BranchExtrusion>(); // creates a list of extrusions
        float currentWidth = branch.Scale; // take the scale of the current branch
        BuilderPolygon toExtrude = branch.Polygon; // takes the polygon of the branch (from the custom polyhedron builder)
        TreeNode t = branch.Node; // gets the branch's corresponding node
        while (true) // iterates until exited
        {
            int childrenCount = t.ChildrenCount(); // gets the number of children in the tree
            Vector3 extrude = new Vector3(0, 0, 0); // creates a zero vector for extrusion
            Vector3 rot = new Vector3(0, 1, 0).RotateTo(toExtrude.Normal); // matrix maths for rotation
            if (childrenCount <= 1) // if there are no children (or only goes up)
            {
                extrude = new Vector3(prng.NextFloat(-0.45f, 0.45f), prng.NextFloat(1.2f, 2.2f), prng.NextFloat(-0.45f, 0.45f)); // creates a random extrusion vector with the given ranges
                extrude.y = extrude.y * currentWidth; // vertical extrusion is linearly scaled by the current node's width
                extrude.x = extrude.x * Mathf.Pow(currentWidth, 0.3f); // horizontal (xz) extrusion scaled by approximate cube root of node's width
                extrude.z = extrude.z * Mathf.Pow(currentWidth, 0.3f);
            }
            else
            {
                extrude = new Vector3(0, currentWidth, 0); // otherwise only extrudes vertically
            }
            extrude = extrude.Rotate(rot.x, rot.y, rot.z, new Vector3(0, 0, 0)); // rotates by the given vector from the origin
            Vector3[] extrudeBase = toExtrude.ExtrudeRaw(extrude); // raw extrusion data from the polyhedron builder
            if (childrenCount <= 1) // once again, if only goes up or is end node
            {
                float factor = prng.NextFloat(0.7f, 0.9f); // factor to taper branch
                currentWidth *= factor; // multiply by factor
                for (int i = 0; i < extrudeBase.Length; i++) // iterates through each of the 4 vertices of the extrusion, not hardcoded incase modification to create n-sided polygons is desired in the future
                {
                    extrudeBase[i].y += UpwardsBias; // increments by the upward bias (we tend to want it to go upwards, as we are trying to mimic organic generation, where branches would tend upwards)
                }
                extrudeBase = extrudeBase.QuadScale(factor); // scale by the precalculated factor
                if (childrenCount == 0) // if there are no more children (hence it is the end of the branch)
                {
                    Vector3 norm = extrudeBase.Normal(); // take the normal vector of the extrusion base
                    if (norm.y >= 0) // if the normal's y component is non-negative
                    {
                        Vector3 rotateToHorizontal = norm.RotateTo(new Vector3(0, 1, 0)); // calculate the rotation vector
                        rotateToHorizontal = rotateToHorizontal * prng.NextFloat(0.05f, 0.15f); // randomly dampens the rotation
                        extrudeBase = extrudeBase.QuadRotate(rotateToHorizontal.x, rotateToHorizontal.y, rotateToHorizontal.z); // rotates the extrusion by the calculated amount
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Skipped!"); // log that the point has been skipped
                    }
                }
                toExtrude.Extrude(extrudeBase); // builds the extrusion (and corresponding sides) through the polyhedron builder
            }
            else // however if the node has more children
            {
                toExtrude.Extrude(extrudeBase); // take the base as usual
                int polygon = builder.Polygons.Count; // takes the polygon count
                be.Add(new BranchExtrusion(builder.Polygon(polygon - prng.Next(2, 6)), t.Branch, currentWidth)); // chooses random side to create branch from
            }
            toExtrude = builder.LastPolygon(); // mark the last polygon to be extruded
            if (childrenCount == 0) // exit the loop if there are no more children
            {
                break;
            }
            else
            {
                t = t.Up; // otherwise the next node is upwards
            }
        }
        foreach (BranchExtrusion extrude in be) // recursively adds extrusions for each branch
        {
            AddBranch(extrude);
        }
    }
    public void Generate(System.Random random) // generate with a given random seed
    {
        builder = new PolyhedronBuilder(); // declare the polyhedron builder
        filter = GetComponent<MeshFilter>(); // gets the mesh filter of the object
        collider = GetComponent<MeshCollider>(); // gets the collider of the object
        m = new Mesh(); // creates a new mesh
        if (random != null) // if the random is specified, use that
        {
            prng = random;
        }
        TreeData tree = new TreeData(BranchChance, InitialLength, prng); // create a new tree
        TreeNode t = tree.BaseNode; // take the first node as the base of tree
        float currentWidth = 1; // start off the width as 1
        builder = new PolyhedronBuilder(); // create new builder
        builder.AddQuad(new Vector3(-0.5f, 0, 0.5f), new Vector3(-0.5f, 0, -0.5f), new Vector3(0.5f, 0, -0.5f), new Vector3(0.5f, 0, 0.5f)); // adds initial quad (we use a width of 1, as it will be easier to scale with unity's built in transform tools if modifications are needed)
        AddBranch(new BranchExtrusion(builder.Polygon(0), t, currentWidth)); // starts the branch generation
        m = builder.Generate(); // generate the mesh from the polyhedron builder
    }
    public void Apply() // apply all the meshes
    {
        filter.mesh = m; // set filters
        filter.sharedMesh = m;
        collider.sharedMesh = m; // set collider
    }
}