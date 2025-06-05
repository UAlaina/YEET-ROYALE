using UnityEngine;
using UnityEngine.Rendering;

public class HPManager : MonoBehaviour
{
    public int maxHealth;
    public int health;

    private PlayerRespawn respawnCode;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
    {
        health = maxHealth;
        respawnCode = GetComponent<PlayerRespawn>();
    }

   public void takeDamage(int damage)
    {
        health -= damage;
        if (health <= 1)
        {
            health = 0;
            Die();

        }
    }
    public void Die()
    {
        respawnCode.respawn();
        health = maxHealth;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
