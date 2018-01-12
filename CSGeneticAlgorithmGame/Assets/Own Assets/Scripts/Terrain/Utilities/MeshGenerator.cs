using UnityEngine;
using System.Collections.Generic;

public static class MeshGenerator
{
    public static MeshData GenerateMesh(float[,] map)
    {
        // SEBASTIAN LAGUE
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        float[] offset = { (width - 1) / -2f, (height - 1) / 2f };
        MeshData data = new MeshData();
        int v = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                data.AddVertex(offset[0] + x, map[x, y], offset[1] - y);
                if ((x < (width - 1)) && (y < (height - 1)))
                {
                    data.AddTriangle(v, v + width + 1, v + width);
                    data.AddTriangle(v + width + 1, v, v + 1);
                }
                v++;
            }
        }
        return data;
    }
}
public class MeshData
{
    List<Vector3> vertices;
    List<int> triangles;
    public MeshData()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
    }
    public void AddTriangle(params int[] p)
    {
        foreach (int point in p)
        {
            triangles.Add(point);
        }
    }
    public void AddVertex(float x, float y, float z)
    {
        vertices.Add(new Vector3(x, y, z));
    }
    public Mesh Generate()
    {
        Mesh ret = new Mesh();
        ret.vertices = vertices.ToArray();
        ret.triangles = triangles.ToArray();
        ret.RecalculateNormals();
        ret.RecalculateBounds();
        return ret;
    }
}
