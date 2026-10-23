using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Looks at what is in front of the camera. If it is an item, shows a prompt and picks it up on E.
public class ItemInteractor : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private PlayerController player;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Text promptText;
    [SerializeField] private float reach = 3.5f;

    private WorldItem target;

    private void Update()
    {
        target = player.InputBlocked ? null : FindTarget();

        promptText.gameObject.SetActive(target != null);
        if (target == null) return;

        string amount = target.Count > 1 ? " x" + target.Count : "";
        promptText.text = "[E]  Pick up " + target.Item.displayName + amount;

        Keyboard kb = Keyboard.current;
        if (kb != null && kb.eKey.wasPressedThisFrame) PickUp(target);
    }

    private WorldItem FindTarget()
    {
        Transform cam = playerCamera.transform;
        if (!Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, reach, ~0, QueryTriggerInteraction.Ignore)) return null;
        return hit.collider.GetComponentInParent<WorldItem>();
    }

    private void PickUp(WorldItem worldItem)
    {
        int leftover = inventory.Add(worldItem.Item, worldItem.Count);

        if (leftover <= 0) Destroy(worldItem.gameObject);
        else worldItem.SetCount(leftover);
    }
}
