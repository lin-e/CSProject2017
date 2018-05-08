using UnityEngine;

public class CustomWater : MonoBehaviour
{
    // Derivative of danielzeller (GitHub)'s Lowpoly-Water-Unity, as permitted by the MIT license, by lin-e (GitHub) https://eugenel.in/
    public float NoiseStep = 0.0001f; // the amount the noise changes by each update
    public float NoiseAmplitude = 1; // amplitude of the nopise

    float timeOffset = 0; // initial offset
    float[] offset = { 0, 0 }; // x y offset
    int[] meshSize; // size of mesh
    public Mesh mesh; // actual mesh component
    public MeshFilter filter; // mesh filter
    public Vector3[] vertices; // vertices of mesh
    public void SetOffset(float x) // set the offset in x and y
    {
        offset = new float[] { x, x }; // set both offsets to x
    }
    void Start()
    {
        filter = GetComponent<MeshFilter>(); // initialises components
        mesh = filter.sharedMesh; // set mesh to shared mesh
        Vector3[] oldVertices = mesh.vertices; // get array of old verts
        int[] triangles = mesh.triangles; // get all triangle verts
        vertices = new Vector3[triangles.Length]; // create a new array of verts
        for (int i = 0; i < vertices.Length; i++) // iterate through each item
        {
            vertices[i] = oldVertices[triangles[i]]; // take the verticies from the old array based on the triangle offset
            triangles[i] = i; // set triangle to the counter
        }
        mesh.vertices = vertices; // set mesh verts
        mesh.triangles = triangles; // set mesh triangles
        mesh.RecalculateBounds(); // recalculate bounding box of mesh
        mesh.RecalculateNormals(); // recalculate lighting normals of mesh
    }
    public void Apply() // apply changes
    {
        mesh.vertices = vertices; // apply the vertices
        mesh.RecalculateBounds(); // recalculate mesh bounds
        mesh.RecalculateNormals(); // recalculate the normals for the mesh
        mesh.MarkDynamic(); // mark the mesh as dynamic 
        filter.mesh = mesh; // apply the mesh
    }
    public void Calculate() // calculate next update
    {
        timeOffset += NoiseStep; // increment offset by step
        for (int i = 0; i < vertices.Length; i++) // calculate for each vert
        {
            Vector3 vector = vertices[i]; // get the specific vertex
            vector.y = Mathf.PerlinNoise(vector.x * (timeOffset + offset[0]), vector.z * (timeOffset + offset[1])) * NoiseAmplitude; // use perlin noise to create a gradual wave motion
            vertices[i] = vector; // set vector
        }
    }
    void OnTriggerEnter(Collider other) // triggers when something touches the water
    {
        GameObject obj = other.gameObject; // the object
        if (obj.GetComponent<PlayerHealth>() != null) // if its the player
        {
            obj.GetComponent<PlayerHealth>().DecreaseHealth(60f); // decreasae health by 60
            obj.GetComponent<PlayerHealth>().DecreaseEnergy(25f); // decrease energy by 25
        }
    }
}