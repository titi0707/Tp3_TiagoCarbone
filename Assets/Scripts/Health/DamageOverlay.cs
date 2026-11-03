using UnityEngine;
using UnityEngine.UI;

public class DamageOverlay : MonoBehaviour
{
    [SerializeField] Image overlay;
    [SerializeField] float maxAlpha = 0.4f;
    [SerializeField] float fadeSpeed = 2f;

    float alpha;

    void Awake()
    {
        SetAlpha(0f);
    }

    public void Flash()
    {
        alpha = maxAlpha;
        SetAlpha(alpha);
    }

    void Update()
    {
        if (alpha <= 0f) return;

        alpha = Mathf.MoveTowards(alpha, 0f, fadeSpeed * Time.deltaTime);
        SetAlpha(alpha);
    }

    void SetAlpha(float value)
    {
        Color color = overlay.color;
        color.a = value;
        overlay.color = color;
    }
}