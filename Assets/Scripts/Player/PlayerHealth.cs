using System;
using UnityEngine;

// Health of the player. Armor from the inventory blocks a part of every hit.
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Inventory inventory;
    [SerializeField, Range(0f, 90f)] private float maxBlock = 80f;      // armor can never block more than this percent

    public float Max => maxHealth;
    public float Current { get; private set; }
    public bool IsFull => Current >= maxHealth;
    public bool IsDead => Current <= 0f;

    public event Action Changed;

    private void Awake()
    {
        Current = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        float blocked = Mathf.Min(inventory.Defense, maxBlock) / 100f;
        Current = Mathf.Max(0f, Current - amount * (1f - blocked));
        Changed?.Invoke();
    }

    public void Heal(float amount)
    {
        Current = Mathf.Min(maxHealth, Current + amount);
        Changed?.Invoke();
    }

    public void Revive()
    {
        Current = maxHealth;
        Changed?.Invoke();
    }
}
