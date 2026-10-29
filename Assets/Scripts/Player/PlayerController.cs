using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float sprintSpeed = 8f;
    [SerializeField] float jumpHeight = 1.2f;
    [SerializeField] float gravity = -20f;

    
    [SerializeField] Transform cameraTransform;
    [SerializeField] float mouseSensitivity = 0.1f;
    [SerializeField] float maxLookAngle = 85f;

    CharacterController controller;
    Vector3 velocity;
    float pitch;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
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
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

       
        transform.Rotate(Vector3.up * mouseDelta.x);
        pitch = Mathf.Clamp(pitch - mouseDelta.y, -maxLookAngle, maxLookAngle);
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
        float speed = kb.leftShiftKey.isPressed ? sprintSpeed : walkSpeed;
        controller.Move(move * speed * Time.deltaTime);

        if (grounded && kb.spaceKey.wasPressedThisFrame)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}