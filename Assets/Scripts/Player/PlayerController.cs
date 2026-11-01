using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerData data;
    [SerializeField] Transform cameraTransform;

    CharacterController controller;
    Vector3 velocity;
    float pitch;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        Health health = GetComponent<Health>();
        if (health != null) health.Initialize(data.maxHealth);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null) return;

        Look();
        Move();
    }

    void Look()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * data.mouseSensitivity;

        transform.Rotate(Vector3.up * mouseDelta.x);
        pitch = Mathf.Clamp(pitch - mouseDelta.y, -data.maxLookAngle, data.maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void Move()
    {
        Keyboard kb = Keyboard.current;

        bool grounded = controller.isGrounded;
        if (grounded && velocity.y < 0f) velocity.y = -2f;

        float x = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
        float z = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);
        Vector3 input = Vector3.ClampMagnitude(new Vector3(x, 0f, z), 1f);

        Vector3 move = transform.right * input.x + transform.forward * input.z;
        float speed = kb.leftShiftKey.isPressed ? data.sprintSpeed : data.walkSpeed;
        controller.Move(move * speed * Time.deltaTime);

        if (grounded && kb.spaceKey.wasPressedThisFrame)
            velocity.y = Mathf.Sqrt(data.jumpHeight * -2f * data.gravity);

        velocity.y += data.gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}