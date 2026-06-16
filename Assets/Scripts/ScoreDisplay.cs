using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    void Update()
    {
        _scoreText.text = ScoreManager.Instance.CurrentScore.ToString();
    }
}