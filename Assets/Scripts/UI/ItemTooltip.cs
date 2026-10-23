using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// The small box with the item information that follows the mouse.
public class ItemTooltip : MonoBehaviour
{
    [SerializeField] private RectTransform box;
    [SerializeField] private Text text;
    [SerializeField] private Canvas canvas;

    private static readonly Vector2 offset = new Vector2(18f, -18f);

    private void Awake()
    {
        Hide();
    }

    public void Show(ItemData item, int count)
    {
        string typeColor = ColorUtility.ToHtmlStringRGB(SlotUI.TypeColor(item.type));
        string content = "<b>" + item.displayName + "</b>\n<color=#" + typeColor + ">" + item.type + "</color>";

        if (!string.IsNullOrEmpty(item.description)) content += "\n\n" + item.description;

        switch (item.type)
        {
            case ItemType.Consumable: content += "\n\nHeals " + item.healAmount.ToString("0") + " HP"; break;
            case ItemType.Armor: content += "\n\nBlocks " + item.defense + "% damage"; break;
            case ItemType.Weapon: content += "\n\nDamage " + item.damage.ToString("0"); break;
        }

        if (item.IsStackable) content += "\n<color=#9aa0a6>Stack " + count + " / " + item.maxStack + "</color>";

        text.text = content;
        gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(box);
        Follow();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        Follow();
    }

    // Under the mouse, but flipped to the other side when it would leave the screen.
    private void Follow()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 size = box.rect.size * canvas.scaleFactor;
        Vector2 position = mouse.position.ReadValue() + new Vector2(offset.x, offset.y);

        if (position.x + size.x > Screen.width) position.x = mouse.position.ReadValue().x - offset.x - size.x;
        if (position.y - size.y < 0f) position.y = mouse.position.ReadValue().y - offset.y + size.y;

        box.position = position;
    }
}
