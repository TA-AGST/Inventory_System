using UnityEngine;

public enum ItemType { Weapon, Consumable, Armor, Resource }

// Where an item can be worn. Only weapons and armor use this.
public enum EquipSlot { None, Head, Body, Legs, Weapon }

// One asset per item. All the numbers of an item live here, so the inventory code stays the same for every item.
[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string displayName = "Item";
    [TextArea] public string description;
    public ItemType type = ItemType.Resource;
    public Color color = Color.white;      // colour of the tile in the UI and of the item in the world
    public Sprite icon;                    // optional, the tile shows the first letter when there is no icon
    public int maxStack = 1;

    [Header("Consumable")]
    public float healAmount;

    [Header("Equipment")]
    public EquipSlot equipSlot = EquipSlot.None;
    public int defense;                    // percent of damage that armor blocks
    public float damage;                   // weapons

    public bool IsStackable => maxStack > 1;
}
