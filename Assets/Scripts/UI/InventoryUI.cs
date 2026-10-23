using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// The inventory screen (open with I or Tab) and the hotbar. It builds all the slots, and handles drag and drop,
// right click and the tooltip. The real data stays in Inventory, this class only shows it.
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private PlayerController player;
    [SerializeField] private SlotUI slotPrefab;

    [Header("Screen")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform gridParent;
    [SerializeField] private Transform hotbarParent;
    [SerializeField] private SlotUI[] equipmentSlots;      // head, body, legs, weapon
    [SerializeField] private Text statsText;
    [SerializeField] private ItemTooltip tooltip;
    [SerializeField] private RectTransform dragLayer;      // the ghost is created here so it is drawn on top

    private SlotUI[] views;
    private SlotUI ghost;
    private SlotUI dragged;
    private SlotUI hovered;

    public bool IsOpen => panel.activeSelf;

    private void Start()
    {
        views = new SlotUI[inventory.SlotCount];

        for (int i = 0; i < inventory.HotbarSize; i++)
            views[i] = CreateSlot(hotbarParent, i, (i + 1).ToString(), "");

        for (int i = inventory.HotbarSize; i < inventory.GridSize; i++)
            views[i] = CreateSlot(gridParent, i, "", "");

        string[] hints = { "HEAD", "BODY", "LEGS", "WEAPON" };
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            int index = inventory.GridSize + i;
            equipmentSlots[i].Init(this, index, "", hints[i]);
            views[index] = equipmentSlots[i];
        }

        ghost = Instantiate(slotPrefab, dragLayer);
        ghost.Init(this, -1, "", "");
        ghost.GetComponent<CanvasGroup>().blocksRaycasts = false;
        ghost.gameObject.SetActive(false);

        inventory.Changed += Refresh;
        Refresh();
        SetOpen(false);
    }

    private void OnDestroy()
    {
        inventory.Changed -= Refresh;
    }

    private SlotUI CreateSlot(Transform parent, int index, string key, string hint)
    {
        SlotUI slot = Instantiate(slotPrefab, parent);
        slot.name = "Slot_" + index;
        slot.Init(this, index, key, hint);
        return slot;
    }

    private void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if (kb.iKey.wasPressedThisFrame || kb.tabKey.wasPressedThisFrame) SetOpen(!IsOpen);
        else if (IsOpen && kb.escapeKey.wasPressedThisFrame) SetOpen(false);
    }

    private void SetOpen(bool open)
    {
        panel.SetActive(open);
        player.InputBlocked = open;

        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = open;

        if (!open) CancelDrag();
        tooltip.Hide();
    }

    private void Refresh()
    {
        for (int i = 0; i < views.Length; i++) views[i].Show(inventory.GetSlot(i));

        statsText.text = "Armor  " + inventory.Defense + "%\nDamage  " + inventory.WeaponDamage.ToString("0");
        UpdateTooltip();
    }

    // ---------------- called by SlotUI ----------------

    public void BeginDrag(SlotUI slot)
    {
        if (slot.Slot == null || slot.Slot.IsEmpty) return;

        dragged = slot;
        ghost.Show(slot.Slot.item, slot.Slot.count);
        ghost.gameObject.SetActive(true);
        tooltip.Hide();
    }

    public void Drag(Vector2 position)
    {
        if (dragged != null) ghost.transform.position = position;
    }

    public void EndDrag(bool outsideScreen)
    {
        if (dragged == null) return;

        // dropped on a slot is handled in DropOn, so only "over nothing" is left
        if (outsideScreen && IsOpen) inventory.Drop(dragged.Index);
        CancelDrag();
    }

    public void DropOn(SlotUI target)
    {
        if (dragged == null) return;
        inventory.Move(dragged.Index, target.Index);
    }

    // Right click: split a stack of resources, or use / wear / take off the item.
    public void RightClick(SlotUI slot)
    {
        ItemSlot data = slot.Slot;
        if (data == null || data.IsEmpty) return;

        if (data.item.type == ItemType.Resource && data.count > 1) inventory.Split(slot.Index);
        else inventory.Use(slot.Index);
    }

    public void Hover(SlotUI slot, bool entering)
    {
        if (entering) hovered = slot;
        else if (hovered == slot) hovered = null;

        UpdateTooltip();
    }

    // ---------------- helpers ----------------

    private void UpdateTooltip()
    {
        bool show = IsOpen && dragged == null && hovered != null && hovered.Slot != null && !hovered.Slot.IsEmpty;
        if (show) tooltip.Show(hovered.Slot.item, hovered.Slot.count);
        else tooltip.Hide();
    }

    private void CancelDrag()
    {
        dragged = null;
        if (ghost != null) ghost.gameObject.SetActive(false);
    }
}
