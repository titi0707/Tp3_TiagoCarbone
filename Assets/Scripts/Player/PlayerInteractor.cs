using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] float interactRange = 3f;
    [SerializeField] LayerMask interactMask = ~0;
    [SerializeField] TMP_Text promptText;

    PlayerInventory inventory;

    void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {
        IInteractable target = FindInteractable();
        string prompt = target != null ? target.GetPrompt(inventory) : null;

        bool hasPrompt = !string.IsNullOrEmpty(prompt);
        promptText.enabled = hasPrompt;
        if (hasPrompt) promptText.text = prompt;

        if (hasPrompt && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            target.Interact(inventory);
    }

    IInteractable FindInteractable()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactMask, QueryTriggerInteraction.Ignore))
            return hit.collider.GetComponentInParent<IInteractable>();

        return null;
    }
}