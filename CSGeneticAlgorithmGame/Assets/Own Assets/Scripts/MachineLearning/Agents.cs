using UnityEngine;

public class Agents : MonoBehaviour
{
    public AgentManager Manager; // stores the manager
    public Genes Genes; // stores the genes of this entity
    public int Health = 2000; // default starting health
    public Vector3 Velocity; // velocity component
    public Vector3 Acceleration; // same with acceleration
    public bool Alive = true; // flag for whether the entity is active or not
    public bool Infected = false; // same flag but for disease
    public float PlayerDamageOnHit = 2f; // damage the agent should do to the player
    public int HealthGainOnHit = 5; // the amount of health the agent should gain if it hits the player
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
    void OnCollisionEnter(Collision col) // fires on collision
    {
        GameObject collided = col.gameObject; // gets the object collided with
        if (collided.GetComponent<PlayerHealth>() != null) // if the object has the player health script
        {
            PlayerHealth player = collided.GetComponent<PlayerHealth>(); // get a reference to the script
            player.DecreaseHealth(PlayerDamageOnHit); // decrease the health by some amount
            Health += HealthGainOnHit; // increase the health of the entity on hit
        }
        if (collided.GetComponent<Projectile>() != null) // if the object collides with a projectile
        {
            Alive = false; // flag as dead
            Destroy(gameObject); // simply destroy the object
        }
    }
    public bool Calculate() // runs on each update to calculate
    {
        if (!Alive) // if the entity is somehow dead before the update
        {
            return false; // mark as dead
        }
        Vector3 target = Manager.Target.transform.position; // gets the position vector of the player
        if (Infected) // if the entity is infected
        {
            Health -= 2; // reduce the health by 2
        }
        if (Vector3.Distance(target, transform.position) > Manager.SafeDiameter) // if the agent isn't close enough to the target
        {
            Health--; // decrease the health of the agent
            foreach (Transform projectile in Manager.Projectiles.transform) // iterate through each child projectile
            {
                if (projectile.gameObject.GetComponent<Projectile>().Destroyed) // if the projectile is destroyed
                {
                    continue; // skip it
                }
                if (Vector3.Distance(projectile.position, transform.position) < Manager.ObstacleRadius) // if the entity is within a certain radius of the projectile
                {
                    Health -= 5; // decrease the health by 5
                }
            }
        }
        else
        {
            Health += 1; // increase the health to reward good genes
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
            if (projectile.gameObject.GetComponent<Projectile>().Destroyed) // if the projectile is destroyed
            {
                continue; // skip it
            }
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
        Velocity = Velocity + new Vector3(0, -1 * Manager.DownwardsBias, 0); // add a down force
        if (transform.position.y > Manager.YLimit) // if it is above the limit
        {
            Velocity = Velocity + new Vector3(0, -1 * Manager.YLimitForce, 0); // add a vertical force
        }
        Velocity = Velocity.Limit((Mathf.Abs(Genes.DNA[0]) + Mathf.Abs(Genes.DNA[2])) / 8); // limit the velocity to prevent the agent from shooting off
        Acceleration.SetMagnitude(0); // reset the acceleration
        Velocity = Velocity * 0.999f; // small dampening force
        transform.position = transform.position + Velocity; // move the agent in space
        return true; // tell the manager that the agent is alive
    }
    public void Reproduce()
    {
        float diseaseRoll = AgentManager.Generator.NextFloat(0, 1); // the disease roll
        if ((!Infected && diseaseRoll < Manager.InfectionChance) || (Infected && diseaseRoll < Manager.CureChance)) // does check depending on current infection state
        {
            Infected = !Infected; // toggles infection
        }
        if (AgentManager.Generator.NextFloat(0, 1) < Manager.ReproduceChance) // if the generator rolls something lower than the chance; then it should run the code
        {
            int count = Manager.transform.childCount; // get the number of children
            Agents partner = null; // hold the current partner, use null as placeholder
            for (int i = 0; i < 5; i++) // have max 5 attempts to prevent locking
            {
                GameObject obj = Manager.transform.GetChild(AgentManager.Generator.Next(0, count)).gameObject; // pick a random child component
                if (obj.GetComponent<Agents>() != null) // if its an entity
                {
                    Agents temp = obj.GetComponent<Agents>(); // set the temp partner to be the entity
                    if (temp.Alive) // if the possible partner is alive
                    {
                        partner = temp; // set the partner
                        break; // exit the iteration
                    }
                }
            }
            if (partner != null) // if a partner was found
            {
                if (partner.Infected || Infected) // if either party is infected
                {
                    partner.Infected = true; // set both to be infected to spread the disease
                    Infected = true;
                }
                else
                {
                    Vector3 spawnLocation = new Vector3(
                        gameObject.transform.position.x + AgentManager.Generator.NextFloat(-15, 15),
                        gameObject.transform.position.y + AgentManager.Generator.NextFloat(-15, 15),
                        gameObject.transform.position.z + AgentManager.Generator.NextFloat(-15, 15)
                        ); // create a new location vector close to this parent
                    Genes newGenes = Genes.Mix(partner.Genes); // use the gene mixing code to create a new child
                    GameObject created = Instantiate(Manager.Agent); // create the object
                    created.transform.parent = gameObject.transform.parent; // set the new agent as a child of this object's parent, so that the new object is a sibling
                    created.transform.position = spawnLocation; // set the location to be the one created earlier
                    created.GetComponent<Agents>().Overwrite(newGenes); // replace the random genes with the mixed genes
                }
            }
        }
    }
    public void UpdateTransform() // change the transform (rotation etc)
    {
        Vector3 target = transform.position + Velocity; // calculate the target
        transform.LookAt(target); // rotate the transform to look at the target
        Vector3 euler = transform.rotation.eulerAngles; // get the euler angles (easier to modify than quaternions)
        euler.x = euler.x + 90; // offset the x by 90 to rotate
        transform.rotation = Quaternion.Euler(euler); // set the rotation by converting the euler angles back into quaternions
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
