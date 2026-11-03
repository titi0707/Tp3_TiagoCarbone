using UnityEngine;

public class HealthSounds : MonoBehaviour
{
    [SerializeField] AudioClip hurtSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] float volume = 1f;

    public void PlayHurt()
    {
        Play(hurtSound);
    }

    public void PlayDeath()
    {
        Play(deathSound);
    }

    void Play(AudioClip clip)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, transform.position, volume);
    }
}