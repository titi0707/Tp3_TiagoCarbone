public interface IInteractable
{
    string GetPrompt(PlayerInventory inventory);
    void Interact(PlayerInventory inventory);
}