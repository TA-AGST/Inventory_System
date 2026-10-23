using UnityEngine;
using UnityEngine.InputSystem;

// Keys 1-6 use the item in that hotbar slot (eat it, wear it). Also two test keys: K hurts the player, R revives.
public class HotbarInput : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private PlayerHealth health;
    [SerializeField] private PlayerController player;
    [SerializeField] private float testDamage = 20f;

    private void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if (kb.kKey.wasPressedThisFrame) health.TakeDamage(testDamage);
        if (kb.rKey.wasPressedThisFrame && health.IsDead) health.Revive();

        if (player.InputBlocked) return;

        for (int i = 0; i < inventory.HotbarSize; i++)
        {
            Key key = (Key)((int)Key.Digit1 + i);
            if (kb[key].wasPressedThisFrame) inventory.Use(i);
        }
    }
}
