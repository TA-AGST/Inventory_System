using UnityEngine;
using UnityEngine.UI;

// Health bar, armor value and the short messages ("Picked up ...", "Inventory full").
public class PlayerHud : MonoBehaviour
{
    [SerializeField] private PlayerHealth health;
    [SerializeField] private Inventory inventory;
    [SerializeField] private RectTransform healthFill;
    [SerializeField] private Text healthText;
    [SerializeField] private Text armorText;
    [SerializeField] private Text messageText;
    [SerializeField] private GameObject deadText;
    [SerializeField] private float messageTime = 2.2f;

    private float messageTimer;

    private void OnEnable()
    {
        health.Changed += Refresh;
        inventory.Changed += Refresh;
        inventory.Message += ShowMessage;
    }

    private void OnDisable()
    {
        health.Changed -= Refresh;
        inventory.Changed -= Refresh;
        inventory.Message -= ShowMessage;
    }

    private void Start()
    {
        messageText.text = "";
        Refresh();
    }

    private void Refresh()
    {
        float ratio = health.Current / health.Max;
        healthFill.localScale = new Vector3(ratio, 1f, 1f);
        healthText.text = health.Current.ToString("0") + " / " + health.Max.ToString("0");
        armorText.text = "Armor " + inventory.Defense + "%";
        deadText.SetActive(health.IsDead);
    }

    private void ShowMessage(string message)
    {
        messageText.text = message;
        messageText.color = Color.white;
        messageTimer = messageTime;
    }

    private void Update()
    {
        if (messageTimer <= 0f) return;

        messageTimer -= Time.deltaTime;
        Color c = messageText.color;
        c.a = Mathf.Clamp01(messageTimer / 0.5f);
        messageText.color = c;
    }
}
