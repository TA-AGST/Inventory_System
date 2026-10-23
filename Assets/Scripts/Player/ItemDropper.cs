using UnityEngine;

// When the inventory throws something away, this puts it in the world in front of the player.
public class ItemDropper : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private WorldItem worldItemPrefab;
    [SerializeField] private float throwSpeed = 3f;

    private void OnEnable()
    {
        inventory.Dropped += OnDropped;
    }

    private void OnDisable()
    {
        inventory.Dropped -= OnDropped;
    }

    private void OnDropped(ItemData item, int count)
    {
        Transform cam = playerCamera.transform;
        WorldItem dropped = Instantiate(worldItemPrefab, cam.position + cam.forward * 1.2f, Quaternion.identity);
        dropped.Setup(item, count);

        Rigidbody body = dropped.GetComponent<Rigidbody>();
        if (body != null) body.linearVelocity = cam.forward * throwSpeed;
    }
}
