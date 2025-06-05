using UnityEngine;

public enum CollectibleType { Coin, Bomb } // You can add more types later

public class Collectible : MonoBehaviour
{
    public CollectibleType type;

    [Header("Hover Settings")]
    public float floatStrength = 0.5f;
    public float floatSpeed = 2f;
    public float rotationSpeed = 45f;

    private Vector3 startPos;
    private float floatOffset;

    private void Start()
    {
        startPos = transform.position;
        floatOffset = Random.Range(0f, 2 * Mathf.PI); // Adds phase offset to desync hover
    }

    private void Update()
    {
        // Floating motion
        float newY = Mathf.Sin(Time.time * floatSpeed + floatOffset) * floatStrength;
        transform.position = startPos + new Vector3(0, newY, 0);

        // Rotation
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var handler = other.GetComponent<CollectibleHandler>();
        if (handler != null)
        {
            handler.OnCollect(this); // Send reference to the collectible
        }

        gameObject.SetActive(false); // Disable after pickup
    }
}

