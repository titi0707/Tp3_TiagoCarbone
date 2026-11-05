using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [SerializeField] Health bossHealth;
    [SerializeField] Slider slider;
    [SerializeField] GameObject barRoot;

    void Start()
    {
        barRoot.SetActive(false);
    }

    public void Show()
    {
        barRoot.SetActive(true);
    }

    void Update()
    {
        if (bossHealth == null)
        {
            barRoot.SetActive(false);
            return;
        }

        slider.value = bossHealth.CurrentHealth / bossHealth.MaxHealth;
    }
}