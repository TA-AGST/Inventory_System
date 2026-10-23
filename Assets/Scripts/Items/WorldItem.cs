using UnityEngine;

// An item lying in the world. Look at it and press E to pick it up.
// It also runs in the editor, so the colour and the name show up in the scene view too.
[ExecuteAlways]
public class WorldItem : MonoBehaviour
{
    [SerializeField] private ItemData item;
    [SerializeField] private int count = 1;
    [SerializeField] private TextMesh label;
    [SerializeField] private Renderer body;
    [SerializeField] private float labelDistance = 5f;

    public ItemData Item => item;
    public int Count => count;

    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    private MaterialPropertyBlock block;

    private void OnEnable()
    {
        Refresh();
    }

    public void Setup(ItemData newItem, int newCount)
    {
        item = newItem;
        count = newCount;
        Refresh();
    }

    // Called when only a part of the stack fitted in the inventory.
    public void SetCount(int newCount)
    {
        count = newCount;
        Refresh();
    }

    private void Refresh()
    {
        if (item == null) return;

        // a property block changes the colour without making a new material for every item
        if (block == null) block = new MaterialPropertyBlock();
        block.SetColor(BaseColor, item.color);
        body.SetPropertyBlock(block);
        label.text = count > 1 ? item.displayName + " x" + count : item.displayName;

        // weapons are long, armor is chunky, the rest is a small box
        switch (item.type)
        {
            case ItemType.Weapon: body.transform.localScale = new Vector3(0.12f, 0.12f, 0.6f); break;
            case ItemType.Armor: body.transform.localScale = new Vector3(0.4f, 0.4f, 0.25f); break;
            case ItemType.Consumable: body.transform.localScale = new Vector3(0.25f, 0.18f, 0.25f); break;
            default: body.transform.localScale = new Vector3(0.28f, 0.28f, 0.28f); break;
        }
    }

    private void LateUpdate()
    {
        if (!Application.isPlaying) return;

        // the name always faces the player
        Camera cam = Camera.main;
        if (cam == null) return;

        // only items close to the player show their name, so far ones do not pile up on the screen
        float distance = Vector3.Distance(cam.transform.position, transform.position);
        label.gameObject.SetActive(distance < labelDistance);
        label.transform.rotation = cam.transform.rotation;
    }
}
