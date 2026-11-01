using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "FPS/Weapon Data")]
public class WeaponData : ScriptableObject
{
   
    public string weaponName = "Arma";

    
    public float damage = 20f;
    public float range = 100f;
    public float fireRate = 10f;
    public bool automatic = true;

  
    public int magazineSize = 30;
    public int startingReserveAmmo = 120;
    public int maxReserveAmmo = 180;
    public float reloadTime = 1.5f;

    
    public bool useProjectile = false;
    public Projectile projectilePrefab;
    public float projectileSpeed = 25f;
    public float explosionRadius = 3f;
    public float explosionForce = 10f;
}