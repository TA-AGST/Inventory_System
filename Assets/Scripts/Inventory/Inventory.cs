using System;
using UnityEngine;

// All the items of the player. The first slots are the normal grid (the first few of them are the hotbar),
// the last four slots are the equipment: head, body, legs and weapon.
public class Inventory : MonoBehaviour
{
    [SerializeField] private int gridSize = 30;
    [SerializeField] private int hotbarSize = 6;
    [SerializeField] private PlayerHealth health;

    private static readonly EquipSlot[] equipmentOrder = { EquipSlot.Head, EquipSlot.Body, EquipSlot.Legs, EquipSlot.Weapon };

    private ItemSlot[] slots;

    public event Action Changed;
    public event Action<ItemData, int> Dropped;
    public event Action<string> Message;

    public int GridSize => gridSize;
    public int HotbarSize => hotbarSize;
    public int SlotCount => slots.Length;

    private void Awake()
    {
        slots = new ItemSlot[gridSize + equipmentOrder.Length];
        for (int i = 0; i < gridSize; i++) slots[i] = new ItemSlot(EquipSlot.None);
        for (int i = 0; i < equipmentOrder.Length; i++) slots[gridSize + i] = new ItemSlot(equipmentOrder[i]);
    }

    public ItemSlot GetSlot(int index)
    {
        return slots[index];
    }

    public bool IsEquipment(int index)
    {
        return index >= gridSize;
    }

    public int EquipmentIndex(EquipSlot slot)
    {
        return gridSize + Array.IndexOf(equipmentOrder, slot);
    }

    // Total armor of everything worn (percent of damage that is blocked).
    public int Defense
    {
        get
        {
            int total = 0;
            for (int i = gridSize; i < slots.Length; i++) if (!slots[i].IsEmpty) total += slots[i].item.defense;
            return total;
        }
    }

    public float WeaponDamage
    {
        get
        {
            ItemSlot weapon = slots[EquipmentIndex(EquipSlot.Weapon)];
            return weapon.IsEmpty ? 0f : weapon.item.damage;
        }
    }

    // ---------------- adding and removing ----------------

    // Puts items in the grid: first onto stacks that are not full, then into empty slots. Returns how many did not fit.
    public int Add(ItemData item, int count)
    {
        if (item.IsStackable)
        {
            for (int i = 0; i < gridSize && count > 0; i++)
            {
                ItemSlot slot = slots[i];
                if (slot.item != item || slot.count >= item.maxStack) continue;

                int moved = Mathf.Min(count, item.maxStack - slot.count);
                slot.count += moved;
                count -= moved;
            }
        }

        for (int i = 0; i < gridSize && count > 0; i++)
        {
            if (!slots[i].IsEmpty) continue;

            int moved = Mathf.Min(count, item.maxStack);
            slots[i].Set(item, moved);
            count -= moved;
        }

        Changed?.Invoke();
        if (count > 0) Message?.Invoke("Inventory full");
        return count;
    }

    // ---------------- moving ----------------

    // Drag and drop: move a stack onto another slot. The same item stacks together, otherwise the two swap places.
    public void Move(int from, int to)
    {
        if (from == to) return;

        ItemSlot a = slots[from];
        ItemSlot b = slots[to];
        if (a.IsEmpty) return;

        if (!b.CanHold(a.item))
        {
            Message?.Invoke(a.item.displayName + " cannot go there");
            return;
        }
        if (!b.IsEmpty && !a.CanHold(b.item)) return;

        if (!b.IsEmpty && b.item == a.item && a.item.IsStackable)
        {
            int moved = Mathf.Min(a.count, a.item.maxStack - b.count);
            b.count += moved;
            a.count -= moved;
            if (a.count <= 0) a.Clear();
        }
        else
        {
            ItemData item = a.item;
            int count = a.count;
            a.Set(b.item, b.count);
            b.Set(item, count);
        }

        Changed?.Invoke();
    }

    // Takes half of a stack and puts it in the first empty grid slot.
    public void Split(int index)
    {
        ItemSlot slot = slots[index];
        if (slot.IsEmpty || slot.count < 2) return;

        for (int i = 0; i < gridSize; i++)
        {
            if (!slots[i].IsEmpty) continue;

            int half = slot.count / 2;
            slot.count -= half;
            slots[i].Set(slot.item, half);
            Changed?.Invoke();
            return;
        }
        Message?.Invoke("Inventory full");
    }

    // Throws the whole stack into the world (the dropper component spawns it).
    public void Drop(int index)
    {
        ItemSlot slot = slots[index];
        if (slot.IsEmpty) return;

        Dropped?.Invoke(slot.item, slot.count);
        slot.Clear();
        Changed?.Invoke();
    }

    // ---------------- using ----------------

    public void Use(int index)
    {
        ItemSlot slot = slots[index];
        if (slot.IsEmpty) return;

        switch (slot.item.type)
        {
            case ItemType.Consumable:
                UseConsumable(slot);
                break;

            case ItemType.Weapon:
            case ItemType.Armor:
                Equip(index);
                break;

            default:
                Message?.Invoke(slot.item.displayName + " cannot be used");
                break;
        }
    }

    private void UseConsumable(ItemSlot slot)
    {
        if (health.IsFull)
        {
            Message?.Invoke("Health is already full");
            return;
        }

        health.Heal(slot.item.healAmount);
        Message?.Invoke("Used " + slot.item.displayName + "  +" + slot.item.healAmount.ToString("0") + " HP");

        slot.count--;
        if (slot.count <= 0) slot.Clear();
        Changed?.Invoke();
    }

    // From the grid: wear it. From an equipment slot: take it off into the first free grid slot.
    private void Equip(int index)
    {
        if (IsEquipment(index))
        {
            for (int i = 0; i < gridSize; i++)
            {
                if (!slots[i].IsEmpty) continue;
                Move(index, i);
                return;
            }
            Message?.Invoke("Inventory full");
            return;
        }

        Move(index, EquipmentIndex(slots[index].item.equipSlot));
    }
}
