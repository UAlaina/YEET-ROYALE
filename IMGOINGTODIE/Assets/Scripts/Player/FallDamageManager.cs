using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FallDamageHandler : MonoBehaviour
{
    public float fallThreshold = -15f;        // Minimum fall speed to take damage
    public float damageMultiplier = 2f;       // How much damage per unit beyond threshold
    public float groundCheckDistance = 1.1f;  // Adjust based on your controller height

    private float lastYVelocity;
    private bool wasGroundedLastFrame = true;
    private CharacterController controller;
    private HPManager hpManager;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        hpManager = GetComponent<HPManager>(); // Assuming HPManager is on the same GameObject
    }

    void Update()
    {
        // Save velocity.y before grounding
        float currentYVelocity = controller.velocity.y;

        // Detect landing after falling
        if (!wasGroundedLastFrame && controller.isGrounded)
        {
            if (lastYVelocity < fallThreshold)
            {
                float excessSpeed = Mathf.Abs(lastYVelocity) - Mathf.Abs(fallThreshold);
                int damageToApply = Mathf.RoundToInt(excessSpeed * damageMultiplier);
                hpManager.takeDamage(damageToApply);

                Debug.Log($"Fall damage taken: {damageToApply}");
            }
        }

        // Update tracking variables
        wasGroundedLastFrame = controller.isGrounded;
        lastYVelocity = currentYVelocity;
    }
}
