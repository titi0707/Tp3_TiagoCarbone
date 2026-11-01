using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [SerializeField] Color flashColor = Color.red;
    [SerializeField] float duration = 0.1f;

    Renderer[] renderers;
    Color[] originalColors;
    float flashEndTime;
    bool flashing;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].material.color;
    }

    public void Flash()
    {
        foreach (Renderer r in renderers)
            r.material.color = flashColor;

        flashEndTime = Time.time + duration;
        flashing = true;
    }

    void Update()
    {
        if (!flashing || Time.time < flashEndTime) return;

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = originalColors[i];

        flashing = false;
    }
}