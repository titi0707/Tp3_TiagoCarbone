using UnityEngine;

public class BossBullet : MonoBehaviour
{
    [SerializeField] float radius = 0.1f;
    [SerializeField] float lifeTime = 5f;
    [SerializeField] LayerMask hitMask = ~0;
    [SerializeField] AudioClip ricochetSound;

    Vector3 direction;
    float speed;
    float damage;
    int bouncesLeft;
    Transform owner;

    public void Launch(Vector3 direction, float speed, float damage, int maxBounces, Transform owner)
    {
        this.direction = direction;
        this.speed = speed;
        this.damage = damage;
        bouncesLeft = maxBounces;
        this.owner = owner;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        float distance = speed * Time.deltaTime;

        if (Physics.SphereCast(transform.position, radius, direction, out RaycastHit hit, distance, hitMask, QueryTriggerInteraction.Ignore)
            && !(owner != null && hit.transform.IsChildOf(owner)))
        {
            Health health = hit.collider.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            if (bouncesLeft > 0)
            {
                bouncesLeft--;
                transform.position = hit.point + hit.normal * radius;
                direction = Vector3.Reflect(direction, hit.normal);
                transform.rotation = Quaternion.LookRotation(direction);

                if (ricochetSound != null) AudioSource.PlayClipAtPoint(ricochetSound, hit.point);
                return;
            }

            Destroy(gameObject);
            return;
        }

        transform.position += direction * distance;
    }
}