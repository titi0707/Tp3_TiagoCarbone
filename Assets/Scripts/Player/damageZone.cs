using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] float damage = 10f;
    [SerializeField] float interval = 1f;   

    float nextDamageTime;

    void OnTriggerStay(Collider other)
    {
        if (Time.time < nextDamageTime) return;

        Health health = other.GetComponentInParent<Health>();
        if (health == null) return;

        health.TakeDamage(damage);
        nextDamageTime = Time.time + interval;
    }
}