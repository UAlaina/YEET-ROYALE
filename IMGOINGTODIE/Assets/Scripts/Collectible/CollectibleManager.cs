using UnityEngine;

public class CollectibleHandler : MonoBehaviour
{
    [Header("COLLECTION STUFF")]
    public Transform handTransform;
    public GameObject bombPrefab;
    public int coinCount = 0;

    private GameObject heldBomb;
    private bool hasBomb;

    private float throwCharge;
    public float maxCharge = 2f;
    public float throwForceMultiplier = 10f;

    public void OnCollect(Collectible collectible)
    {
        switch (collectible.type)
        {
            case CollectibleType.Coin:
                coinCount++;
                Debug.Log("Coins: " + coinCount);
                break;

            case CollectibleType.Bomb:
                if (!hasBomb)
                {
                    heldBomb = Instantiate(bombPrefab, handTransform);
                    heldBomb.transform.localPosition = Vector3.zero;
                    heldBomb.transform.localRotation = Quaternion.identity;
                    heldBomb.GetComponent<Collider>().enabled = false;
                    heldBomb.GetComponent<Rigidbody>().isKinematic = true;
                    hasBomb = true;
                }
                break;

                // Add future PowerUp logic here
        }
    }

    private void Update()
    {
        if (hasBomb)
        {
            if (Input.GetKeyDown(KeyCode.E))
                throwCharge = 0f;

            if (Input.GetKey(KeyCode.E))
                throwCharge += Time.deltaTime;

            if (Input.GetKeyUp(KeyCode.E))
                ThrowBomb();
        }
    }

    private void ThrowBomb()
    {
        if (heldBomb == null) return;

        heldBomb.transform.SetParent(null);

        Rigidbody rb = heldBomb.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            float clampedCharge = Mathf.Clamp(throwCharge, 0f, maxCharge);
            float throwPower = clampedCharge * throwForceMultiplier;

            rb.AddForce(handTransform.forward * throwPower, ForceMode.Impulse);
        }

        Destroy(heldBomb, 5f); // destroy after 5 seconds

        heldBomb = null;
        hasBomb = false;
        throwCharge = 0f;
    }
}
