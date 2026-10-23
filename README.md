# Inventory System

A grid inventory for a first person game, made in Unity (URP). It has a small test room with items to pick up.

> **Note:** Logic build mein AI ki help li gayi hai.

## Controls
| Key / mouse | Action |
|---|---|
| W A S D | Move |
| Shift | Sprint |
| Space | Jump |
| Mouse | Look |
| I or Tab | Open / close the inventory |
| E | Pick up the item you look at |
| 1 - 6 | Use the item in that hotbar slot |
| Drag and drop | Move, stack or swap items |
| Drag outside the window | Drop the item in the world |
| Right click | Use / wear / take off an item, or split a stack of resources |
| K | Take 20 damage (to test armor and healing) |
| R | Revive after dying |

## Features
- **Items are data:** every item is an `ItemData` ScriptableObject (weapons, consumables, armor, resources).
- **30 slot grid** with a hotbar (first 6 slots) and **4 equipment slots** (head, body, legs, weapon). Equipment slots only accept the right kind of item.
- **Stacking and splitting** with a stack limit per item.
- **Consumables** heal the player, **armor** blocks a percent of the damage, a **weapon** shows its damage.
- **Tooltip** with the item information, drop and pick up in the world.
- The inventory logic (`Inventory`) does not know anything about the UI, `InventoryUI` only shows it.

## Run it
1. Unity **6000.3.25f1** (Universal 3D / URP).
2. Open `Assets/Scenes/SampleScene.unity` and press Play.