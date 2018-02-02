using UnityEngine;

public class RockGenerator : MonoBehaviour
{
    public int Octaves = 5;
    public int Size = 10;

    float[,] map;
    public void GenerateMap(System.Random prng)
    {
        // NOTE TO SELF: DOCUMENT THIS PROPERLY
        /*

        if we consider the shape of the rock, we can use a reciprocal graph to model one profile, and then have that rotated around the y axis in 3d to create a 3d shape in a 

        */
        map = new float[Size, Size]; // create a n x n grid of vertices
        for (int y = 0; y < Size; y++) // iterate through each point
        {
            for (int x = 0; x < Size; x++) // iterate through each y point
            {
                map[x, y] = 0f; // set to 0
            }
        }
        for (int i = 1; i < 4; i++) // iterate through 1-3
        {
            int bounds = (Size / 2) + i; // manually create bounds
            for (int y = Size - bounds; y < bounds; y++) // iterate within bounds (from centre out) 
            {
                for (int x = Size - bounds; x < bounds; x++) // same as above
                {
                    if (map[x, y] != 0f) // if it's already set
                    {
                        continue; // prevent accidental overwrite
                    }
                    map[x, y] = 8f / i; // set to reciprocal curve
                }
            }
        }
        for (int o = 1; o < Octaves + 1; o++) // for each octave
        {
            int[] offset = { prng.Next(0, 10000), prng.Next(0, 10000) }; // calculate offset
            float amplitude = 4f / o; // set amplitude
            float frequency = (1 / 100f) * Mathf.Pow(6, o); // set frequency
            for (int y = 0; y < Size; y++) // iterate through y
            {
                for (int x = 0; x < Size; x++) // iterate x
                {
                    if (map[x, y] == 0f) // if it's flat, ignore
                    {
                        continue;
                    }
                    map[x, y] += Mathf.Lerp((amplitude / -2f), amplitude, Mathf.PerlinNoise((x + offset[0]) * frequency, (y + offset[1]) * frequency)); // same noise function for smoothness
                }
            }
        }
        // code below goes through each point on the heightmap and then increments the height by one, unless it's the base (then keep as 0)
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                if (map[x, y] == 0f)
                {
                    continue;
                }
                map[x, y] += 1;
            }
        }
    }
    // same standard mesh generation function
    public void CreateMesh()
    {
        MeshData data = MeshGenerator.GenerateMesh(map);
        Mesh mesh = data.Generate();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.MarkDynamic();
        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}