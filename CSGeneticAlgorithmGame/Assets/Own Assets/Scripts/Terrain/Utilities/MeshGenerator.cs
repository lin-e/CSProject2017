using UnityEngine;
using System.Collections.Generic;

public static class MeshGenerator
{
    public static MeshData GenerateMesh(float[,] map)
    {
        int width = map.GetLength(0); // get the width of the heightmap
        int height = map.GetLength(1); // get the height
        float[] offset = { (width - 1) / -2f, (height - 1) / 2f }; // calculate the offsets
        MeshData data = new MeshData(); // create a new empty mesh
        int v = 0; // vert counter
        for (int y = 0; y < height; y++) // iterate through each row
        {
            for (int x = 0; x < width; x++) // iterate through each column
            {
                data.AddVertex(offset[0] + x, map[x, y], offset[1] - y); // add vertex based on offset
                if ((x < (width - 1)) && (y < (height - 1))) // small check
                {
                    data.AddTriangle(v, v + width + 1, v + width); // add first triangle
                    data.AddTriangle(v + width + 1, v, v + 1); // add second one
                }
                v++; // increment verts
            }
        }
        return data; // return generated data
    }
}
public class MeshData
{
    List<Vector3> vertices;
    List<int> triangles;
    public MeshData() // constructor
    {
        vertices = new List<Vector3>(); // create list
        triangles = new List<int>();
    }
    public void AddTriangle(params int[] p) // add triangle
    {
        foreach (int point in p) // iterate points
        {
            triangles.Add(point); // add to triangle
        }
    }
    public void AddVertex(float x, float y, float z) // simple function to add vertices
    {
        vertices.Add(new Vector3(x, y, z)); // add new vert
    }
    public Mesh Generate() // export mesh
    {
        Mesh ret = new Mesh(); // create new return mesh
        ret.vertices = vertices.ToArray(); // convert vertices to array to array
        ret.triangles = triangles.ToArray(); // same for triangles
        ret.RecalculateNormals(); // calculate render normals
        ret.RecalculateBounds(); // calculate collider bounds
        return ret; // return generated mesh
    }
}
