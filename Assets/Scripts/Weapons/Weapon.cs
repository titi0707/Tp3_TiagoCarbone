using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
   
    [SerializeField] Transform cameraTransform;
    [SerializeField] float damage = 25f;
    [SerializeField] float range = 100f;
    [SerializeField] float fireRate = 8f;
    [SerializeField] bool automatic = true;
    [SerializeField] LayerMask hitMask = ~0;

    
    [SerializeField] int magazineSize = 30;     
    [SerializeField] int reserveAmmo = 90;      
    [SerializeField] int maxReserveAmmo = 180; 
    [SerializeField] float reloadTime = 1.5f;

    
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject impactPrefab;

    int currentAmmo;
    float nextFireTime;
    bool isReloading;

    public int CurrentAmmo => currentAmmo;
    public int ReserveAmmo => reserveAmmo;
    public int MagazineSize => magazineSize;
    public bool IsReloading => isReloading;

    void Awake()
    {
        currentAmmo = magazineSize;
    }

    void Update()
    {
        if (Mouse.current == null || Keyboard.current == null) return;
        if (isReloading) return;

       
        if (Keyboard.current.rKey.wasPressedThisFrame && currentAmmo < magazineSize && reserveAmmo > 0)
        {
            StartCoroutine(Reload());
            return;
        }

        bool trigger = automatic
            ? Mouse.current.leftButton.isPressed
            : Mouse.current.leftButton.wasPressedThisFrame;

        if (trigger && Time.time >= nextFireTime)
        {
            if (currentAmmo > 0)
            {
                nextFireTime = Time.time + 1f / fireRate;
                Shoot();
            }
            else if (reserveAmmo > 0)
            {
                StartCoroutine(Reload()); 
            }
            else if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Debug.Log("Sin munición");
            }
        }
    }

    void Shoot()
    {
        currentAmmo--;
        if (muzzleFlash != null) muzzleFlash.Play();

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);

            Health health = hit.collider.GetComponentInParent<Health>();
            if (health != null) health.TakeDamage(damage);

            if (hit.rigidbody != null)
                hit.rigidbody.AddForceAtPosition(ray.direction * 5f, hit.point, ForceMode.Impulse);

            if (impactPrefab != null)
            {
                GameObject impact = Instantiate(impactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f);
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Recargando...");
        yield return new WaitForSeconds(reloadTime);

       
        int needed = magazineSize - currentAmmo;
        int toLoad = Mathf.Min(needed, reserveAmmo);

        currentAmmo += toLoad;
        reserveAmmo -= toLoad;
        isReloading = false;
    }

    
    public void AddAmmo(int amount)
    {
        reserveAmmo = Mathf.Min(reserveAmmo + amount, maxReserveAmmo);
    }
}