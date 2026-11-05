using UnityEngine;

public class TripwireZone : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] AudioClip explosionSound;

    bool triggered;

    void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player")) return;

        Health health = other.GetComponentInParent<Health>();
        if (health == null) return;

        triggered = true;

        if (explosionPrefab != null)
            Destroy(Instantiate(explosionPrefab, other.transform.position, Quaternion.identity), 3f);

        if (explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound, other.transform.position);

        health.TakeDamage(health.CurrentHealth);
    }
}