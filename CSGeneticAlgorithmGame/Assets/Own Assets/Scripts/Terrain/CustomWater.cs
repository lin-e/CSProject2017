using UnityEngine;

public class CustomWater : MonoBehaviour
{
    // Derivative of danielzeller (GitHub)'s Lowpoly-Water-Unity, as permitted by the MIT license, by lin-e (GitHub) https://eugenel.in/
    public float NoiseStep = 0.0001f;
    public float NoiseAmplitude = 1;

    float timeOffset = 0;
    float[] offset = { 0, 0 };
    int[] meshSize;
    public Mesh mesh;
    public MeshFilter filter;
    public Vector3[] vertices;
    public void SetOffset(float x)
    {
        offset = new float[] { x, x };
    }
    void Start()
    {
        filter = GetComponent<MeshFilter>();
        mesh = filter.sharedMesh;
        Vector3[] oldVertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        vertices = new Vector3[triangles.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = oldVertices[triangles[i]];
            triangles[i] = i;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
    public void Apply()
    {
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.MarkDynamic();
        filter.mesh = mesh;
    }
    public void Calculate()
    {
        timeOffset += NoiseStep;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vector = vertices[i];
            vector.y = Mathf.PerlinNoise(vector.x * (timeOffset + offset[0]), vector.z * (timeOffset + offset[1])) * NoiseAmplitude;
            vertices[i] = vector;
        }
    }
}