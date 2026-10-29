using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] Weapon weapon;
    [SerializeField] TMP_Text ammoText;

    void Update()
    {
        ammoText.text = weapon.IsReloading
            ? "Recargando..."
            : $"{weapon.CurrentAmmo} / {weapon.ReserveAmmo}";
    }
}