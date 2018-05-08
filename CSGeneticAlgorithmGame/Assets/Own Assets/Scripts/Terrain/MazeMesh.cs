using UnityEngine;
using System.Linq;

// note that this acts as the main terrain generator
public class MazeMesh : MonoBehaviour
{
    public int MeshScale = 10; // scale of mesh
    public int Octaves = 3; // octaves for variation
    public int MazeSize = 21; // size of the maze nodes
    public int StoneCount = 250; // number of stones to create
    public GameObject Stone; // object of a stone
    public int TreeCount = 100; // number of trees to create
    public GameObject Tree; // tree prefab

    float[,] map = { }; // generated heightmap
    MazeGenerator generator; // maze generator
    System.Random prng; // random seed
    
    void Start()
    {
        prng = new System.Random(); // initialise random
        Debug.Log("DEBUG: Generating maze heightmap"); // debugging
        GenerateMap(); // create height map
        Debug.Log("DEBUG: Generating maze mesh");
        CreateMesh(); // create the mesh
        Debug.Log("DEBUG: Populating environment with stones");
        PopulateStones(); // create stones
        Debug.Log("DEBUG: Populating environment with dead trees");
        PopulateDeadTrees(); // create trees
        Debug.Log("DEBUG: Terrain generation complete");
        FindObjectOfType<EventManager>().StartTimer(); // start the timer
    }
    public void PopulateDeadTrees() // creates all the trees (instantiate the prefab which then generates trees)
    {
        float[] offset = { (map.GetLength(0) - 1) / -2f, (map.GetLength(1) - 1) / 2f }; // calculate the point offset
        for (int i = 0; i < TreeCount; i++) // do this for each tree
        {
            GameObject g = Instantiate(Tree); // create new object
            g.transform.parent = gameObject.transform; // set the object as a child of the current object
            g.transform.localScale = new Vector3(Tree.transform.localScale.x / transform.localScale.x, Tree.transform.localScale.y / transform.localScale.y, Tree.transform.localScale.z / transform.localScale.z); // fix scaling
            g.GetComponent<TreeGenerator>().Generate(prng); // generate with the global prng variable
            g.GetComponent<TreeGenerator>().Apply(); // apply
            int[] point = getMapPoint(); // get a point on the map, which hasn't been used
            g.transform.localPosition = new Vector3(offset[0] + point[0], map[point[0], point[1]] - 0.15f, offset[1] - point[1]); // translate it to the point
        }
    }
    public void PopulateStones() // same as above but for stones
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
    int[] getMapPoint() // picks a random point on the map
    {
        int[] point = generator.MapPoints[prng.Next(0, generator.MapPoints.Count)]; // selects a random point from the generator's points (we don't want objects floating on water)
        generator.MapPoints.RemoveAll(e => e.SequenceEqual(point)); // remove the point from the list (LINQ because it's more elegant than iterating the points
        return point; // return the point
    }
    public void GenerateMap() // create the map
    {
        generator = new MazeGenerator(MazeSize, MazeSize, 0, 4, MeshScale); // generates with the set params (square heightmap)
        generator.Generate(); // generate function
        map = generator.Map; // save the generated map
    }
    public void CreateMesh() // create the maze's mesh
    {
        float[,] heightMap = map; // copy the heightmap
        for (int o = 1; o < Octaves + 1; o++) // add variation for each octabe
        {
            System.Random prng = new System.Random(); // create new random
            int[] offset = { prng.Next(0, 10000), prng.Next(0, 10000) }; // make random offset
            float amplitude = 3f / o; // amplitude inversely proportional to octave
            float frequency = (1 / 100f) * Mathf.Pow(6, o); // scale frequency
            for (int y = 0; y < heightMap.GetLength(1); y++) // iterate through each y point
            {
                for (int x = 0; x < heightMap.GetLength(0); x++) // iterate through each x point
                {
                    if ((heightMap[x, y]) == 0) // if it's 0 (hence flat)
                    {
                        continue; // ignore it
                    }
                    heightMap[x, y] += Mathf.Lerp((amplitude / -2f), amplitude, Mathf.PerlinNoise((x + offset[0]) * frequency, (y + offset[1]) * frequency)); // increment the point
                }
            }
        }
        MeshData data = MeshGenerator.GenerateMesh(heightMap); // create mesh from the heightmap
        Mesh mesh = data.Generate(); // generate the mesh
        mesh.RecalculateBounds(); // calculate bounds for mesh
        mesh.RecalculateNormals(); // calculate lightning normals
        mesh.MarkDynamic(); // mark the mesh as dynamic
        GetComponent<MeshFilter>().sharedMesh = mesh; // set the filter mesh
        GetComponent<MeshCollider>().sharedMesh = mesh; // set collider mesh
    }
}
