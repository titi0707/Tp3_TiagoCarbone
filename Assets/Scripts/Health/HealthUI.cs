using TMPro;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] TMP_Text healthText;

    void Update()
    {
        healthText.text = $"Vida: {Mathf.CeilToInt(health.CurrentHealth)}";
    }
}