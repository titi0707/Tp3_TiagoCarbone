using UnityEngine;

public class WeaponMotion : MonoBehaviour
{
    [SerializeField] float recoilKick = 0.08f;
    [SerializeField] float recoilRotation = 6f;
    [SerializeField] float returnSpeed = 12f;
    [SerializeField] Vector3 reloadOffset = new Vector3(0f, -0.15f, 0f);
    [SerializeField] float reloadTilt = 30f;
    [SerializeField] float equipDrop = 0.3f;

    Weapon weapon;
    Vector3 currentOffset;
    Vector3 currentRotation;

    void Awake()
    {
        weapon = GetComponent<Weapon>();
    }

    void OnEnable()
    {
        currentOffset = Vector3.down * equipDrop;
        currentRotation = Vector3.zero;
    }

    public void Recoil()
    {
        currentOffset += Vector3.back * recoilKick;
        currentRotation += Vector3.left * recoilRotation;
    }

    void Update()
    {
        bool reloading = weapon != null && weapon.IsReloading;

        Vector3 targetOffset = reloading ? reloadOffset : Vector3.zero;
        Vector3 targetRotation = reloading ? new Vector3(reloadTilt, 0f, 0f) : Vector3.zero;

        currentOffset = Vector3.Lerp(currentOffset, targetOffset, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Lerp(currentRotation, targetRotation, returnSpeed * Time.deltaTime);

        transform.localPosition = currentOffset;
        transform.localRotation = Quaternion.Euler(currentRotation);
    }
}