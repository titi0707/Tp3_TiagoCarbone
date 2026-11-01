using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] Weapon[] weapons;

    int currentIndex;

    public Weapon CurrentWeapon => weapons[currentIndex];

    void Start()
    {
        Equip(0);
    }

    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) Equip(0);
        else if (Keyboard.current.digit2Key.wasPressedThisFrame) Equip(1);
        else if (Keyboard.current.digit3Key.wasPressedThisFrame) Equip(2);

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll > 0f) Equip((currentIndex + 1) % weapons.Length);
        else if (scroll < 0f) Equip((currentIndex - 1 + weapons.Length) % weapons.Length);
    }

    void Equip(int index)
    {
        if (index < 0 || index >= weapons.Length) return;

        currentIndex = index;
        for (int i = 0; i < weapons.Length; i++)
            weapons[i].gameObject.SetActive(i == index);
    }
}