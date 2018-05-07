﻿using UnityEngine;

public class Agents : MonoBehaviour
{
    public AgentManager Manager; // stores the manager
    public Genes Genes; // stores the genes of this entity
    public int Health = 100; // default starting health
    public Vector3 Velocity; // velocity component
    public Vector3 Acceleration; // same with acceleration
    public bool Alive = true; // flag for whether the entity is active or not
    public bool Infected = false; // same flag but for disease
    void Start()
    {
        Manager = transform.parent.gameObject.GetComponent<AgentManager>(); // get the manager by finding transform parent
        Genes = new Genes();
        Velocity = new Vector3(AgentManager.Generator.NextFloat(-0.1f, 0.1f), AgentManager.Generator.NextFloat(-0.1f, 0.1f), AgentManager.Generator.NextFloat(-0.1f, 0.1f)); // start with random velocity
        Acceleration = new Vector3(AgentManager.Generator.NextFloat(-0.1f, 0.1f), AgentManager.Generator.NextFloat(-0.1f, 0.1f), AgentManager.Generator.NextFloat(-0.1f, 0.1f)); // same with acceleration
    }
    public void Overwrite(Genes newGenes) // overwriting function for entities created via reproduction
    {
        Genes = newGenes; // set the genes to the parameters
    }
    public bool Calculate() // runs on each update to calculate
    {
        Vector3 target = Manager.Target.transform.position; // gets the position vector of the player
        if (Vector3.Distance(target, transform.position) > Manager.SafeDiameter) // if the agent isn't close enough to the target
        {
            Health--; // decrease the health of the agent
        }
        else
        {
            Health += 2; // increase the health to reward good genes
        }
        if (Health < 0) // if the health is negative
        {
            Alive = false; // mark as dead
            return false; // return false
        }
        if (Health > 100) // if health > maximum health
        {
            Health = 100; // cap at max
        }
        if (Vector3.Distance(target, transform.position) < Genes.DNA[1]) // if the player is within view range
        {
            Vector3 desired = target - transform.position; // calculate the desired vector
            desired = desired.SetMagnitude(Genes.DNA[0] * Manager.MotionMultiplier); // set the scale according to genes
            Vector3 steer = desired - Velocity; // get the steering vector
            steer = steer.Limit(Mathf.Abs(Genes.DNA[0] / 4)); // limit the vector
            Acceleration = Acceleration + steer; // add steering vector
        }
        foreach (Transform projectile in Manager.Projectiles.transform) // iterate through each child projectile
        {
            Vector3 location = projectile.position; // get the position vector of the projectile
            if (Vector3.Distance(location, transform.position) < Genes.DNA[3]) // if the projectile is within view range
            {
                // this is the same code for the player, but modified to use the coordinates of the projectile
                Vector3 desired = location - transform.position; // calculate the desired vector
                desired = desired.SetMagnitude(Genes.DNA[2] * Manager.MotionMultiplier); // set the scale according to genes
                Vector3 steer = desired - Velocity; // get the steering vector
                steer = steer.Limit(Mathf.Abs(Genes.DNA[2] / 4)); // limit the vector
                Acceleration = Acceleration + steer; // add steering vector
            }
        }
        Velocity = Velocity + Acceleration; // simple physics, the velocity is the integral of the acceleration, hence we can add the acceleration to represent a small change in velocity over a near zero time period
        Velocity = Velocity.Limit((Mathf.Abs(Genes.DNA[0]) + Mathf.Abs(Genes.DNA[2])) / 8); // limit the velocity to prevent the agent from shooting off
        Acceleration.SetMagnitude(0); // reset the acceleration
        transform.position = transform.position + Velocity; // move the agent in space
        UpdateTransform();
        return true; // tell the manager that the agent is alive
    }
    public void UpdateTransform() // change the transform (rotation etc)
    {
        transform.rotation = Quaternion.FromToRotation(transform.up, Velocity.normalized);
    }
}
public static class VectorExtensions // small extensions class to clean up code
{
    public static Vector3 SetMagnitude(this Vector3 vec, float mag) // set the magnitude to a certain value
    {
        Vector3 norm = vec.normalized; // get the normal (so we have a magnitude of 1 to scale from)
        return norm * mag; // return the normalised value multiplied by the desired magnitude
    }
    public static Vector3 Limit(this Vector3 vec, float lim) // limit the vector to a maximum
    {
        if (vec.magnitude > lim) // if the magnitude is larger than the limit
        {
            return vec.SetMagnitude(lim); // return the vector at the limit
        }
        else // otherwise
        {
            return vec; // return the vector unchanged
        }
    }
}
