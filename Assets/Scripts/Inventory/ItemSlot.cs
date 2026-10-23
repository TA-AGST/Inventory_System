// One place in the inventory: an item and how many of it. Equipment slots only accept one kind of item.
public class ItemSlot
{
    public ItemData item;
    public int count;
    public readonly EquipSlot accepts;      // None = any item (normal slot)

    public bool IsEmpty => item == null;

    public ItemSlot(EquipSlot accepts)
    {
        this.accepts = accepts;
    }

    public bool CanHold(ItemData candidate)
    {
        return accepts == EquipSlot.None || candidate.equipSlot == accepts;
    }

    public void Set(ItemData newItem, int newCount)
    {
        item = newItem;
        count = newCount;
    }

    public void Clear()
    {
        item = null;
        count = 0;
    }
}
