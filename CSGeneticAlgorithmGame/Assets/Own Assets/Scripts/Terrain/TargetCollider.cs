using UnityEngine;

public class TargetCollider : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
        GameObject obj = col.gameObject; // get the game object
        if (obj.GetComponent<PlayerController>() != null) // check if the collided object is the player
        {
            FindObjectOfType<EventManager>().Win(); // fire the win event
        }
    }
}
