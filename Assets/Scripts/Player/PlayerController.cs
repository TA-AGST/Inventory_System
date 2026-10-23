using UnityEngine;
using UnityEngine.InputSystem;

// Walking and looking around in first person. Both stop while the inventory is open.
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float walkSpeed = 4.5f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float lookSensitivity = 0.12f;

    private CharacterController controller;
    private float verticalSpeed;
    private float yaw;
    private float pitch;

    // Set by the inventory screen: when true the player does not move or look.
    public bool InputBlocked { get; set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        yaw = transform.eulerAngles.y;
    }

    private void Update()
    {
        Keyboard kb = Keyboard.current;
        Mouse mouse = Mouse.current;
        if (kb == null || mouse == null) return;

        Vector3 move = Vector3.zero;

        if (!InputBlocked)
        {
            Vector2 delta = mouse.delta.ReadValue() * lookSensitivity;
            yaw += delta.x;
            pitch = Mathf.Clamp(pitch - delta.y, -80f, 80f);

            float x = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
            float z = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);
            float speed = kb.leftShiftKey.isPressed ? sprintSpeed : walkSpeed;
            move = (transform.right * x + transform.forward * z).normalized * speed;

            if (controller.isGrounded && kb.spaceKey.wasPressedThisFrame)
                verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        if (controller.isGrounded && verticalSpeed < 0f) verticalSpeed = -2f;
        verticalSpeed += gravity * Time.deltaTime;
        move.y = verticalSpeed;

        controller.Move(move * Time.deltaTime);
    }
}
