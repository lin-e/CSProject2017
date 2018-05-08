using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public static System.Random Generator = new System.Random(); // declare a new random for the agents to use
    public GameObject Target; // variable to hold the target (the player)
    public GameObject Projectiles; // holds the projectile parent (more efficient detection)
    public GameObject Agent; // variable to hold the agent to spawn
    public int InitialPopulation = 150; // holds the size of the initial population, the rest of these are pretty self explanatory
    public float SafeDiameter = 150f;
    public float ReproduceChance = 0.0002f;
    public float ObstacleRadius = 50f;
    public float MotionMultiplier = 12f;
    public float CureChance = 0.00001f;
    public float InfectionChance = 0.00001f;
    public int SpawnHeight = 20;
    public int SpawnSpread = 100;
    public int CenterX = -380;
    public int CenterZ = -380;

    // we are now working in 3 dimensions, so we have to restrict movement in an axis
    public float YLimit = 30f;
    public float YLimitForce = 10f;
    public float DownwardsBias = 1f;

    // these variables are ported from the javascript version of the code
    public static float AttractionMin = -1f;
    public static float AttractionMax = 1f;
    public static float ViewMin = 0f;
    public static float ViewMax = 150f;
    public static float MutationChance = 0.0001f;

    List<GameObject> Agents = new List<GameObject>(); // holds all the agents (we will do all the updates from here instead of from the agents)

    void Start()
    {
        for (int i = 0; i < InitialPopulation; i++) // do this for each agent
        {
            GameObject created = Instantiate(Agent); // create the object
            created.transform.parent = gameObject.transform; // set the new agent as a child of this object
            created.transform.position = new Vector3(CenterX + Generator.Next(-SpawnSpread, SpawnSpread), SpawnHeight, CenterZ + Generator.Next(-SpawnSpread, SpawnSpread)); // spread the locations of the entities
            Agents.Add(created); // add to list
        }
    }
    void Update()
    {
        if (PauseScreen.Paused) // if the game is paused
        {
            return; // ignore the rest of this code
        }
        foreach (GameObject a in Agents) // iterates through each agent
        {
            Agents agent = a.GetComponent<Agents>(); // gets the component
            agent.Calculate(); // runs calculations to update the entity
            agent.UpdateTransform(); // applies transformations
        }
    }
}
