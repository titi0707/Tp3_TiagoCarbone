using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] bool openWithE = true;
    [SerializeField] string requiredKey = "";
    [SerializeField] string keyDisplayName = "la llave";
    [SerializeField] Vector3 openOffset = new Vector3(0f, 3f, 0f);
    [SerializeField] float openSpeed = 2f;
    [SerializeField] AudioClip openSound;
    [SerializeField] AudioClip lockedSound;

    public UnityEvent onOpened;

    Vector3 closedPosition;
    Vector3 openPosition;
    bool isOpen;

    bool NeedsKey => !string.IsNullOrEmpty(requiredKey);

    void Awake()
    {
        closedPosition = transform.localPosition;
        openPosition = closedPosition + openOffset;
    }

    void Update()
    {
        Vector3 target = isOpen ? openPosition : closedPosition;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, openSpeed * Time.deltaTime);
    }

    public string GetPrompt(PlayerInventory inventory)
    {
        if (isOpen) return null;
        if (!openWithE) return "Se abre con un mecanismo";
        if (NeedsKey && !HasKey(inventory)) return $"Cerrada · Necesitás {keyDisplayName}";
        return "[E] Abrir";
    }

    public void Interact(PlayerInventory inventory)
    {
        if (isOpen || !openWithE) return;

        if (NeedsKey && !HasKey(inventory))
        {
            PlaySound(lockedSound);
            return;
        }

        Open();
    }

    public void Open()
    {
        if (isOpen) return;

        isOpen = true;
        PlaySound(openSound);
        onOpened?.Invoke();
    }

    bool HasKey(PlayerInventory inventory)
    {
        return inventory != null && inventory.HasKey(requiredKey);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null) AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}