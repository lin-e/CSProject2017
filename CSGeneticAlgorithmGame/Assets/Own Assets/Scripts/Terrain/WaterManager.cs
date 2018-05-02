using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class WaterManager : MonoBehaviour
{
    public GameObject Player; // we are using the player's position in order to manage where the water is rendered
    public GameObject Prefab; // the actual water "panels"
    public float NoiseStep = 0.0005f; // the amount the offset should increase by each update
    public float NoiseAmplitude = 0.5f; // the amplitude of the mesh
    public float Scale = 2; // scale of mesh
    public int Side = 7; // side length
    public bool SkipCorners = true; // small optimisation tweak - skips rendering the corners are they aren't visible most of the time

    GameObject[] waterPanels = { }; // array of all the panels
    float[][] positions; // array of positions
    GameObject master; // master object (in which all calculations are done through)
    bool childrenCreationComplete = false; // flag to toggle when all the children meshes are generated
    public void SetValues(float step, float amp) // set the values (just in case we want to tweak it during gameplay)
    {
        NoiseStep = step; // set to params
        NoiseAmplitude = amp; // same as above
        master.GetComponent<CustomWater>().NoiseStep = NoiseStep; // change the master's attributes
        master.GetComponent<CustomWater>().NoiseAmplitude = NoiseAmplitude; // same as above
    }
    void Start()
    {
        System.Random prng = new System.Random(); // creates a new random
        float offset = prng.Next(0, 2000); // random offset
        if (Side % 2 == 0)
        {
            Side++; // makes sure that there's an odd number of sides
        }
        waterPanels = new GameObject[Side * Side]; // creates new flat array
        List<float[]> calculatedPositions = new List<float[]>(); // list to hold all positions
        int[] skipIndex = { 0, Side - 1, (Side - 1) * Side, (Side * Side) - 1 }; // index to skip
        master = Instantiate(Prefab); // create the prefab
        master.transform.parent = gameObject.transform; // make the prefab a child of the current object
        master.transform.localScale = Vector3.zero; // make the scale 0 (we shouldn't be able to see it)
        for (int i = 0; i < Side * Side; i++) // iterate through each point
        {
            if (skipIndex.Contains(i) && SkipCorners) // if it's set to skip corners and the curent index is a corner
            {
                waterPanels[i] = null; // set to null (just so calculations don't mess up later)
                calculatedPositions.Add(new float[2]); // just in case i miss something later on, we can at least have an index
            }
            else // otherwise if we're creating the panel
            {
                GameObject g = Instantiate(Prefab); // create the object
                g.transform.parent = gameObject.transform; // set the object as a child of this object
                g.GetComponent<CustomWater>().SetOffset(offset); // set the offset to the generated offset
                waterPanels[i] = g; // add it to the array of panels
                calculatedPositions.Add(calculatePosition(i)); // calculate the relative position
            }
        }
        positions = calculatedPositions.ToArray(); // converts back to array for operation later
        SetValues(NoiseStep, NoiseAmplitude); // sets the values to the ones specified in the unity scene configuration
        childrenCreationComplete = true; // mark complete
    }
    void Update()
    {
        if (PauseScreen.Paused) // if the game is paused
        {
            return; // do not run the rest of the code
        }
        if (!childrenCreationComplete) // if it hasn't completed yet
        {
            return; // ignore the rest of this code
        }
        CustomWater masterWater = master.GetComponent<CustomWater>(); // get the component of the master
        masterWater.Calculate(); // calculate the mesh
        Vector2 playerPos = new Vector2(Player.transform.position.x, Player.transform.position.z); // take the player's current position in the XZ plane (we're ignoring Y as the water shouldn't rise if the player is to jump)
        // the following code uses maths that's explained in the documentation in full detail; essentially it remaps the location to a smaller grid which allows for manipulation on a secondary grid which is able to manipulate motion on a tiled basis, instead of shifting the entire plane
        playerPos.x /= (Scale * 10); 
        playerPos.y /= (Scale * 10);
        playerPos.x = (float)Math.Round(playerPos.x);
        playerPos.y = (float)Math.Round(playerPos.y);
        for (int i = 0; i < waterPanels.Length; i++)
        {
            if (waterPanels[i] == null)
            {
                continue;
            }
            float[] calculatedPosition = positions[i];
            float x = (calculatedPosition[0] + playerPos.x) * Scale * 10;
            float y = waterPanels[i].transform.position.y;
            float z = (calculatedPosition[1] + playerPos.y) * Scale * 10;
            waterPanels[i].transform.position = new Vector3(x, y, z);
            CustomWater water = waterPanels[i].GetComponent<CustomWater>();
            water.mesh = masterWater.mesh;
            water.filter = masterWater.filter;
            water.vertices = masterWater.vertices;
            water.Apply();
        }
    }
    float[] calculatePosition(int i)
    {
        int offset = (Side - 1) / 2;
        return new float[] { (i % Side) - offset, offset - (float)Math.Floor(i / (float)Side) };
    }
}