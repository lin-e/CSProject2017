using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class Explode : MonoBehaviour
{
    public float Force = 3f; // explosion force
    public float Multiplier = 10f; // standard multiplier (more for tweaking)
    IEnumerator Start()
    {
        Destroy(gameObject, 5f); // remove after 5 seconds to prevent lag
        yield return null;
        List<Collider> colliders = Physics.OverlapSphere(transform.position, 10 * Multiplier).ToList(); // get a list of colliders within the range
        colliders.RemoveAll(c => c.attachedRigidbody == null); // remove all without rigid bodies
        List<Rigidbody> bodies = new List<Rigidbody>(); // create a list of bodies
        foreach (Collider c in colliders)
        {
            Rigidbody body = c.attachedRigidbody; // get the attached body
            if (bodies.Contains(body)) // if it has already had a force applied
            {
                continue; // ignore
            }
            if (c.GetComponent<Projectile>() != null) // if the body has a projectile component
            {
                continue; // ignore it, because you can chain projectiles if you don't which leads to extremely unpredictable gameplay
            }
            bodies.Add(body); // add to list
            body.AddExplosionForce(Force * Multiplier, transform.position, 10 * Multiplier, Multiplier, ForceMode.Impulse); // modified version of unity prefab
        }
    }
}