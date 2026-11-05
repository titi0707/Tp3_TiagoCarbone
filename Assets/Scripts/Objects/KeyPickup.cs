using UnityEngine;
using UnityEngine.Events;

public class KeyPickup : MonoBehaviour, IInteractable
{
    [SerializeField] string keyId = "roja";
    [SerializeField] string displayName = "la llave roja";
    [SerializeField] float rotateSpeed = 90f;
    [SerializeField] AudioClip pickupSound;

    public UnityEvent onPickedUp;

    void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
    }

    public string GetPrompt(PlayerInventory inventory)
    {
        return $"[E] Agarrar {displayName}";
    }

    public void Interact(PlayerInventory inventory)
    {
        if (inventory == null) return;

        inventory.AddKey(keyId);
        if (pickupSound != null) AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        onPickedUp?.Invoke();
        Destroy(gameObject);
    }
}