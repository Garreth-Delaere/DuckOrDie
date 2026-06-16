using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject _gameOverPanel;

    public bool IsGameOver { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void TriggerGameOver()
    {
        IsGameOver = true;
        _gameOverPanel.SetActive(true);
    }

    public void Restart()
    {
        Wall.ResetSpeed();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}