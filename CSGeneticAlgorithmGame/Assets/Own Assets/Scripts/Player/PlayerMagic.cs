using UnityEngine;

public class PlayerMagic : MonoBehaviour
{
    public GameObject[] Projectiles;
    public GameObject ProjectileParent; // store all projectiles in this, allows for machine learning to check easily
    public int Active = 0;

    PlayerHealth statManager;
    void Start()
    {
        statManager = GetComponent<PlayerHealth>();
    }
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            Active++;
            if (Active > Projectiles.Length - 1)
            {
                Active = 0;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            Active--;
            if (Active < 0)
            {
                Active = Projectiles.Length - 1;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Projectile projectile = Projectiles[Active].GetComponent<Projectile>();
            if (statManager.CurrentEnergy >= projectile.Energy)
            {
                GameObject g = Instantiate(Projectiles[Active], ProjectileParent.transform) as GameObject;
                g.transform.position = transform.position + GetComponent<CameraManager>().Active.transform.forward * 2;
                Rigidbody body = g.GetComponent<Rigidbody>();
                body.velocity = GetComponent<CameraManager>().Active.transform.forward * projectile.Velocity;
                statManager.DecreaseEnergy(projectile.Energy);
            }
        }
    }
}