using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class Explode : MonoBehaviour
{
    public float Force = 3f;
    public float Multiplier = 10f;
    IEnumerator Start()
    {
        Destroy(gameObject, 5f);
        yield return null;
        List<Collider> colliders = Physics.OverlapSphere(transform.position, 10 * Multiplier).ToList();
        colliders.RemoveAll(c => c.attachedRigidbody == null);
        List<Rigidbody> bodies = new List<Rigidbody>();
        foreach (Collider c in colliders)
        {
            Rigidbody body = c.attachedRigidbody;
            if (bodies.Contains(body))
            {
                continue;
            }
            if (c.GetComponent<Projectile>() != null)
            {
                continue;
            }
            bodies.Add(body);
            body.AddExplosionForce(Force * Multiplier, transform.position, 10 * Multiplier, Multiplier, ForceMode.Impulse); // modified version of unity prefab
        }
    }
}