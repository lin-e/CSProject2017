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

    // these variables are ported from the javascript version of the code
    public static float AttractionMin = -1f;
    public static float AttractionMax = 1f;
    public static float ViewMin = 0f;
    public static float ViewMax = 500f;
    public static float MutationChance = 0.0001f;

    List<GameObject> Agents = new List<GameObject>(); // holds all the agents (we will do all the updates from here instead of from the agents)

    void Start()
    {
        for (int i = 0; i < InitialPopulation; i++) // do this for each agent
        {
            GameObject created = Instantiate(Agent); // create the object
            created.transform.parent = gameObject.transform; // set the new agent as a child of this object
            created.transform.position = new Vector3(Generator.Next(-30, 30), 20, Generator.Next(-30, 30));
            Agents.Add(created); // add to list
        }
    }
    void Update()
    {
        foreach (GameObject a in Agents)
        {
            a.GetComponent<Agents>().Calculate();
        }
    }
}
