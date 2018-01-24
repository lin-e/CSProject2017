using UnityEngine;

public class PlayerMagic : MonoBehaviour
{
    public GameObject[] Projectiles; // list of all projectiles that are accessible to the user
    public GameObject ProjectileParent; // store all projectiles in this, allows for machine learning to check easily
    public int Active = 0; // index of currently active projectile

    PlayerHealth statManager; // manager to allow this module to interface with the player's health statistics
    void Start()
    {
        statManager = GetComponent<PlayerHealth>(); // sets the manager
    }
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // if the mouse is scrolling up
        {
            Active = (Active + 1) % Projectiles.Length; // increment by one, modulo to make sure index is within range
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f) // same as above but for scroll down
        {
            Active = Active - 1; // decrease by one
            if (Active < 0) // modulo works differently with negatives, hence a manual case is used here
            {
                Active = Projectiles.Length - 1;
            }
        }
        if (Input.GetMouseButtonDown(0)) // if the left mouse button is clicked
        {
            Projectile projectile = Projectiles[Active].GetComponent<Projectile>(); // get the projectile to be fired
            if (statManager.CurrentEnergy >= projectile.Energy) // check that the player has enough energy to use the projectile
            {
                GameObject g = Instantiate(Projectiles[Active], ProjectileParent.transform) as GameObject; // create the projectile in the scene
                g.transform.position = transform.position + GetComponent<CameraManager>().Active.transform.forward * 2; // set the transform to the player's current position, moved forward by the forward transform of the camera (to spawn it further in front of the player
                Rigidbody body = g.GetComponent<Rigidbody>(); // get the body of the spawned projectile
                body.velocity = GetComponent<CameraManager>().Active.transform.forward * projectile.Velocity; // set the velocity to be in the direction of the camera and scale the vector by the set velocity
                statManager.DecreaseEnergy(projectile.Energy); // decrease the player's energy
            }
        }
    }
}