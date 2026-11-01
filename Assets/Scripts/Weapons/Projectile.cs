using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] float lifeTime = 6f;
    [SerializeField] GameObject explosionPrefab;

    Rigidbody rb;
    float damage;
    float explosionRadius;
    float explosionForce;
    bool exploded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Launch(Vector3 velocity, float damage, float explosionRadius, float explosionForce, Collider owner)
    {
        this.damage = damage;
        this.explosionRadius = explosionRadius;
        this.explosionForce = explosionForce;

        if (owner != null)
            Physics.IgnoreCollision(GetComponent<Collider>(), owner);

        rb.AddForce(velocity, ForceMode.VelocityChange);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (exploded) return;
        exploded = true;

        Vector3 point = collision.GetContact(0).point;
        Explode(point);

        if (explosionPrefab != null)
            Destroy(Instantiate(explosionPrefab, point, Quaternion.identity), 2f);

        Destroy(gameObject);
    }

    void Explode(Vector3 center)
    {
        Collider[] hits = Physics.OverlapSphere(center, explosionRadius, ~0, QueryTriggerInteraction.Ignore);
        HashSet<Health> damaged = new HashSet<Health>();

        foreach (Collider hit in hits)
        {
            Health health = hit.GetComponentInParent<Health>();
            if (health != null && damaged.Add(health))
            {
                float distance = Vector3.Distance(center, hit.bounds.ClosestPoint(center));
                float falloff = 1f - Mathf.Clamp01(distance / explosionRadius);
                health.TakeDamage(damage * falloff);
            }

            if (hit.attachedRigidbody != null)
                hit.attachedRigidbody.AddExplosionForce(explosionForce, center, explosionRadius, 0.5f, ForceMode.Impulse);
        }
    }
}