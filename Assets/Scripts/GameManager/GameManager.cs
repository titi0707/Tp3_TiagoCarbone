using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Health playerHealth;
    [SerializeField] PlayerController playerController;
    [SerializeField] WeaponSwitcher weaponSwitcher;
    [SerializeField] GameObject victoryPanel;
    [SerializeField] GameObject defeatPanel;
    [SerializeField] TMP_Text objectiveText;

    int enemiesRemaining;
    bool gameOver;

    void Start()
    {
        Time.timeScale = 1f;
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);

        playerHealth.onDeath.AddListener(Defeat);

        foreach (EnemyAI enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
            RegisterEnemy(enemy.GetComponent<Health>());

        foreach (BossAI boss in FindObjectsByType<BossAI>(FindObjectsSortMode.None))
            RegisterEnemy(boss.GetComponent<Health>());

        UpdateObjectiveText();
    }

    void RegisterEnemy(Health enemyHealth)
    {
        if (enemyHealth == null) return;

        enemyHealth.onDeath.AddListener(OnEnemyKilled);
        enemiesRemaining++;
    }

    void OnEnemyKilled()
    {
        enemiesRemaining--;
        UpdateObjectiveText();

        if (enemiesRemaining <= 0) Victory();
    }

    void Victory()
    {
        EndGame(victoryPanel);
    }

    void Defeat()
    {
        EndGame(defeatPanel);
    }

    void EndGame(GameObject panel)
    {
        if (gameOver) return;
        gameOver = true;

        playerController.enabled = false;
        weaponSwitcher.enabled = false;
        weaponSwitcher.CurrentWeapon.enabled = false;

        panel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void UpdateObjectiveText()
    {
        if (objectiveText != null)
            objectiveText.text = $"Enemigos restantes: {enemiesRemaining}";
    }
}