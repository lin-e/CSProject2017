using UnityEngine;
using System;

public class RandomMotion : MonoBehaviour
{
    public float noiseStep = 0.0001f;
    public float maxX;
    public float maxY;
    public float maxZ;
    float initialX;
    float initialY;
    float initialZ;
    System.Random prng;
    float timeOffset = 0;
    float offset = 0;
    void Start()
    {
        initialX = gameObject.transform.position.x;
        initialY = gameObject.transform.position.y;
        initialZ = gameObject.transform.position.z;
        prng = new System.Random((int)initialX * (int)initialY + (int)initialZ + (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        offset = (float)prng.NextDouble();
    }
    void Update ()
    {
        timeOffset += noiseStep;
        float delta = Mathf.Lerp(-1, 1, Mathf.PerlinNoise(offset + timeOffset, offset));
        gameObject.transform.position = new Vector3(initialX + delta * maxX, initialY + delta * maxY, initialZ + delta * maxZ);
	}
}
