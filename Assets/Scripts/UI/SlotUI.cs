using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// One square of the inventory screen or the hotbar. It only draws a slot and passes the mouse events to InventoryUI.
public class SlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler,
    IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image frame;
    [SerializeField] private Image tile;
    [SerializeField] private Text letter;
    [SerializeField] private Text countText;
    [SerializeField] private Text keyText;
    [SerializeField] private Text hintText;

    private static readonly Color emptyFrame = new Color(1f, 1f, 1f, 0.3f);

    private InventoryUI owner;

    public int Index { get; private set; }
    public ItemSlot Slot { get; private set; }

    public void Init(InventoryUI inventoryUI, int index, string key, string hint)
    {
        owner = inventoryUI;
        Index = index;
        keyText.text = key;
        hintText.text = hint;
    }

    public static Color TypeColor(ItemType type)
    {
        switch (type)
        {
            case ItemType.Weapon: return new Color(0.92f, 0.33f, 0.3f);
            case ItemType.Consumable: return new Color(0.35f, 0.82f, 0.42f);
            case ItemType.Armor: return new Color(0.36f, 0.62f, 0.96f);
            default: return new Color(0.78f, 0.62f, 0.4f);
        }
    }

    public void Show(ItemSlot slot)
    {
        Slot = slot;
        Show(slot.item, slot.count);
    }

    // Also used by the item that follows the mouse while dragging.
    public void Show(ItemData item, int count)
    {
        bool filled = item != null;

        tile.gameObject.SetActive(filled);
        hintText.gameObject.SetActive(!filled);
        frame.color = filled ? TypeColor(item.type) : emptyFrame;
        countText.text = filled && count > 1 ? count.ToString() : "";

        if (!filled)
        {
            letter.text = "";
            return;
        }

        tile.color = item.color;
        tile.sprite = item.icon;
        letter.text = item.icon == null ? item.displayName.Substring(0, 1).ToUpper() : "";
    }

    // ---------------- mouse ----------------

    public void OnBeginDrag(PointerEventData eventData)
    {
        owner.BeginDrag(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        owner.Drag(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // let go with the mouse over nothing = throw it away
        owner.EndDrag(eventData.pointerCurrentRaycast.gameObject == null);
    }

    public void OnDrop(PointerEventData eventData)
    {
        owner.DropOn(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) owner.RightClick(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        owner.Hover(this, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        owner.Hover(this, false);
    }
}
