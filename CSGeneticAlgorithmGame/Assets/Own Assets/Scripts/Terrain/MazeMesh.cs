using UnityEngine;
using System.Threading;
using System.Linq;

public class MazeMesh : MonoBehaviour
{
    public int MeshScale = 10;
    public int Octaves = 3;
    public int MazeSize = 21;
    public int StoneCount = 250;
    public GameObject Stone;
    public int TreeCount = 100;
    public GameObject Tree;

    float[,] map = { };
    MazeGenerator generator;
    System.Random prng;
    
    void Start()
    {
        prng = new System.Random();
        Debug.Log("DEBUG: Generating maze heightmap");
        GenerateMap();
        Debug.Log("DEBUG: Generating maze mesh");
        CreateMesh();
        Debug.Log("DEBUG: Populating environment with stones");
        PopulateStones();
        Debug.Log("DEBUG: Populating environment with dead trees");
        PopulateDeadTrees();
        Debug.Log("DEBUG: Terrain generation complete");
    }
    public void PopulateDeadTrees()
    {
        float[] offset = { (map.GetLength(0) - 1) / -2f, (map.GetLength(1) - 1) / 2f };
        for (int i = 0; i < TreeCount; i++)
        {
            GameObject g = Instantiate(Tree);
            g.transform.parent = gameObject.transform;
            g.transform.localScale = new Vector3(Tree.transform.localScale.x / transform.localScale.x, Tree.transform.localScale.y / transform.localScale.y, Tree.transform.localScale.z / transform.localScale.z);
            g.GetComponent<TreeGenerator>().Generate(prng);
            g.GetComponent<TreeGenerator>().Apply();
            int[] point = getMapPoint();
            g.transform.localPosition = new Vector3(offset[0] + point[0], map[point[0], point[1]] - 0.15f, offset[1] - point[1]);
        }
    }
    public void PopulateStones()
    {
        float[] offset = { (map.GetLength(0) - 1) / -2f, (map.GetLength(1) - 1) / 2f };
        for (int i = 0; i < StoneCount; i++)
        {
            GameObject g = Instantiate(Stone);
            g.transform.parent = gameObject.transform;
            g.transform.localScale = new Vector3(0.15f / transform.localScale.x, 0.15f / transform.localScale.y, 0.15f / transform.localScale.z);
            g.GetComponent<RockGenerator>().GenerateMap(prng);
            g.GetComponent<RockGenerator>().CreateMesh();
            int[] point = getMapPoint();
            g.transform.localPosition = new Vector3(offset[0] + point[0], map[point[0], point[1]] - 0.75f, offset[1] - point[1]);
        }
    }
    int[] getMapPoint()
    {
        int[] point = generator.MapPoints[prng.Next(0, generator.MapPoints.Count)];
        generator.MapPoints.RemoveAll(e => e.SequenceEqual(point));
        return point;
    }
    public void GenerateMap()
    {
        generator = new MazeGenerator(MazeSize, MazeSize, 0, 4, MeshScale);
        generator.Generate();
        map = generator.Map;
    }
    public void CreateMesh()
    {
        float[,] heightMap = map;
        for (int o = 1; o < Octaves + 1; o++)
        {
            System.Random prng = new System.Random();
            int[] offset = { prng.Next(0, 10000), prng.Next(0, 10000) };
            float amplitude = 3f / o;
            float frequency = (1 / 100f) * Mathf.Pow(6, o);
            for (int y = 0; y < heightMap.GetLength(1); y++)
            {
                for (int x = 0; x < heightMap.GetLength(0); x++)
                {
                    if ((heightMap[x, y]) == 0)
                    {
                        continue;
                    }
                    heightMap[x, y] += Mathf.Lerp((amplitude / -2f), amplitude, Mathf.PerlinNoise((x + offset[0]) * frequency, (y + offset[1]) * frequency));
                }
            }
        }
        MeshData data = MeshGenerator.GenerateMesh(heightMap);
        Mesh mesh = data.Generate();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.MarkDynamic();
        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
