using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [SerializeField] WeaponData data;
    [SerializeField] Transform cameraTransform;
    [SerializeField] Transform muzzlePoint;
    [SerializeField] LayerMask hitMask = ~0;

    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] BulletTracer tracerPrefab;
    [SerializeField] GameObject impactPrefab;
    [SerializeField] GameObject enemyImpactPrefab;

    int currentAmmo;
    int reserveAmmo;
    float nextFireTime;
    bool isReloading;
    Collider ownerCollider;

    public string WeaponName => data.weaponName;
    public int CurrentAmmo => currentAmmo;
    public int ReserveAmmo => reserveAmmo;
    public int MagazineSize => data.magazineSize;
    public bool IsReloading => isReloading;

    void Awake()
    {
        currentAmmo = data.magazineSize;
        reserveAmmo = data.startingReserveAmmo;
        ownerCollider = GetComponentInParent<CharacterController>();
    }

    void OnDisable()
    {
        isReloading = false;
    }

    void Update()
    {
        if (Mouse.current == null || Keyboard.current == null) return;
        if (isReloading) return;

        if (Keyboard.current.rKey.wasPressedThisFrame && currentAmmo < data.magazineSize && reserveAmmo > 0)
        {
            StartCoroutine(Reload());
            return;
        }

        bool trigger = data.automatic
            ? Mouse.current.leftButton.isPressed
            : Mouse.current.leftButton.wasPressedThisFrame;

        if (trigger && Time.time >= nextFireTime)
        {
            if (currentAmmo > 0)
            {
                nextFireTime = Time.time + 1f / data.fireRate;
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

        if (data.useProjectile) ShootProjectile();
        else ShootRaycast();
    }

    void ShootRaycast()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        Vector3 endPoint = ray.GetPoint(data.range);

        if (Physics.Raycast(ray, out RaycastHit hit, data.range, hitMask, QueryTriggerInteraction.Ignore))
        {
            endPoint = hit.point;

            Health health = hit.collider.GetComponentInParent<Health>();
            if (health != null) health.TakeDamage(data.damage);

            if (hit.rigidbody != null)
                hit.rigidbody.AddForceAtPosition(ray.direction * 5f, hit.point, ForceMode.Impulse);

            GameObject effect = health != null ? enemyImpactPrefab : impactPrefab;
            if (effect != null)
            {
                GameObject impact = Instantiate(effect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f);
            }
        }

        SpawnTracer(endPoint);
    }

    void SpawnTracer(Vector3 endPoint)
    {
        if (tracerPrefab == null || muzzlePoint == null) return;

        BulletTracer tracer = Instantiate(tracerPrefab, muzzlePoint.position, Quaternion.identity);
        tracer.Init(endPoint);
    }

    void ShootProjectile()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, data.range, hitMask, QueryTriggerInteraction.Ignore)
            ? hit.point
            : ray.GetPoint(data.range);

        Vector3 direction = (targetPoint - muzzlePoint.position).normalized;

        Projectile projectile = Instantiate(data.projectilePrefab, muzzlePoint.position, Quaternion.LookRotation(direction));
        projectile.Launch(direction * data.projectileSpeed, data.damage, data.explosionRadius, data.explosionForce, ownerCollider);
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Recargando...");
        yield return new WaitForSeconds(data.reloadTime);

        int needed = data.magazineSize - currentAmmo;
        int toLoad = Mathf.Min(needed, reserveAmmo);

        currentAmmo += toLoad;
        reserveAmmo -= toLoad;
        isReloading = false;
    }

    public void AddAmmo(int amount)
    {
        reserveAmmo = Mathf.Min(reserveAmmo + amount, data.maxReserveAmmo);
    }
}