using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class WaterManager : MonoBehaviour
{
    public GameObject Player;
    public GameObject Prefab;
    public float NoiseStep = 0.0005f;
    public float NoiseAmplitude = 0.5f;
    public float Scale = 2;
    public int Side = 7;
    public bool SkipCorners = true;

    GameObject[] waterPanels = { };
    float[][] positions;
    GameObject master;
    bool childrenCreationComplete = false;
    public void SetValues(float step, float amp)
    {
        NoiseStep = step;
        NoiseAmplitude = amp;
        master.GetComponent<CustomWater>().NoiseStep = NoiseStep;
        master.GetComponent<CustomWater>().NoiseAmplitude = NoiseAmplitude;
    }
    void Start()
    {
        System.Random prng = new System.Random();
        float offset = prng.Next(0, 2000);
        if (Side % 2 == 0)
        {
            Side++;
        }
        waterPanels = new GameObject[Side * Side];
        List<float[]> calculatedPositions = new List<float[]>();
        int[] skipIndex = { 0, Side - 1, (Side - 1) * Side, (Side * Side) - 1 };
        master = Instantiate(Prefab);
        master.transform.parent = gameObject.transform;
        master.transform.localScale = Vector3.zero;
        for (int i = 0; i < Side * Side; i++)
        {
            if (skipIndex.Contains(i) && SkipCorners)
            {
                waterPanels[i] = null;
                calculatedPositions.Add(new float[2]); // just in case i screw something up
            }
            else
            {
                GameObject g = Instantiate(Prefab);
                g.transform.parent = gameObject.transform;
                g.GetComponent<CustomWater>().SetOffset(offset);
                waterPanels[i] = g;
                calculatedPositions.Add(calculatePosition(i));
            }
        }
        positions = calculatedPositions.ToArray();
        SetValues(NoiseStep, NoiseAmplitude);
        childrenCreationComplete = true;
    }
    void Update()
    {
        if (!childrenCreationComplete)
        {
            return;
        }
        CustomWater masterWater = master.GetComponent<CustomWater>();
        masterWater.Calculate();
        Vector2 playerPos = new Vector2(Player.transform.position.x, Player.transform.position.z);
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