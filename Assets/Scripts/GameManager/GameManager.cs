using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Health playerHealth;
    [SerializeField] PlayerController playerController;
    [SerializeField] Weapon weapon;
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

        EnemyAI[] enemies = FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        foreach (EnemyAI enemy in enemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth == null) continue;

            enemyHealth.onDeath.AddListener(OnEnemyKilled);
            enemiesRemaining++;
        }

        UpdateObjectiveText();
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
        weapon.enabled = false;

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