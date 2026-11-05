using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour, IInteractable
{
    [SerializeField] Transform handle;
    [SerializeField] Vector3 activatedRotation = new Vector3(60f, 0f, 0f);
    [SerializeField] AudioClip activateSound;

    public UnityEvent onActivated;

    bool activated;
    Quaternion targetRotation;

    void Awake()
    {
        if (handle != null)
            targetRotation = handle.localRotation * Quaternion.Euler(activatedRotation);
    }

    void Update()
    {
        if (activated && handle != null)
            handle.localRotation = Quaternion.RotateTowards(handle.localRotation, targetRotation, 360f * Time.deltaTime);
    }

    public string GetPrompt(PlayerInventory inventory)
    {
        return activated ? null : "[E] Activar palanca";
    }

    public void Interact(PlayerInventory inventory)
    {
        if (activated) return;

        activated = true;
        if (activateSound != null) AudioSource.PlayClipAtPoint(activateSound, transform.position);
        onActivated?.Invoke();
    }
}