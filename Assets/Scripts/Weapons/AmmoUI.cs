using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] WeaponSwitcher weaponSwitcher;
    [SerializeField] TMP_Text ammoText;

    void Update()
    {
        Weapon weapon = weaponSwitcher.CurrentWeapon;

        ammoText.text = weapon.IsReloading
            ? $"{weapon.WeaponName}   Recargando..."
            : $"{weapon.WeaponName}   {weapon.CurrentAmmo} / {weapon.ReserveAmmo}";
    }
}