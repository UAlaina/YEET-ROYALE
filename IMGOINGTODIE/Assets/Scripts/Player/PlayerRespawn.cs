using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Transform respawnPoint; //assign this in the inspector 
    public float fallThreshold; //anything that hits the assigned value will respawn
    public float respawnDelay; //for later uses


    public CharacterController controller;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < fallThreshold) 
        {
            respawn();
        }

    }

    public void respawn()
    {
        controller.enabled = false;
        transform.position = respawnPoint.position;
        controller.enabled = true;
    }
}
