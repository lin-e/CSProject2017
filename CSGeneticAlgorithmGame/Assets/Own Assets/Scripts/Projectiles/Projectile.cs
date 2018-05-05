using UnityEngine;
using System.Threading;
using System;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public float Velocity = 50f;
    public float Energy = 15f;
    public GameObject OnCollide;
    public bool DestroyOnCollide = true;
    public bool SmartLightEffect = false;
    public Color SmartLightColour = new Color(0, 0, 0, 0.1f);
    public int SmartLightFade = 1000;
    bool destroyed = false;

    void Start()
    {
        if (SmartLightEffect)
        {
            LIFXManager.ChangeColour(ColorUtility.ToHtmlStringRGB(SmartLightColour), 48); // immersive effect changing environment colour
            Wait(48, () => { LIFXManager.ChangeColour("FFFFFF", SmartLightFade); }); // lambda expression because it doesn't deserve a method
        }
        Destroy(gameObject, 120f); // automatically delete object after 2 minutes to prevent slowing down the game
    }
    void OnCollisionEnter(Collision col)
    {
        if (destroyed)
        {
            return; // do not trigger if it has already collided
        }
        if (OnCollide != null) // if it collides with something that exists
        {
            Instantiate(OnCollide, transform.position, transform.rotation); // spawns the oncollide object, at the same location
        }
        if (DestroyOnCollide) // destroys self if collided
        {
            foreach (ParticleSystem system in GetComponentsInChildren(typeof(ParticleSystem))) // iterates through each particle emitter
            {
                system.Stop(true); // stops it
            }
            foreach (Light light in GetComponentsInChildren(typeof(Light))) // iterates through each light source
            {
                light.intensity = 0; // deletes it
            }
            destroyed = true;  // marks it as deleted
            Destroy(gameObject, 5f); // destroys self after 5 seconds, to allow for any particles to finish
        }
    }
    public void Wait(int ms, Action action)
    {
        StartCoroutine(doWait(ms / 1000f, action)); // thread.sleep causes issues with ui freeze
    }
    IEnumerator doWait(float time, Action callback)
    {
        yield return new WaitForSecondsRealtime(time); // same fix as above
        callback(); // do this once the time is done
    }
}
