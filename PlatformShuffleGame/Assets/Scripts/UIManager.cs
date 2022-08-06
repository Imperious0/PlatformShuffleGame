using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject GameOverUI;
    [SerializeField] private GameObject GameWinUI;
    [SerializeField] private TextMeshProUGUI gameScore;
    private void Awake()
    {
        GameOverUI?.SetActive(false);
        GameWinUI?.SetActive(false);
    }
    private void Start()
    {
        GameManager.Instance.GameOverEvent += GameOverListener;
        GameManager.Instance.GameWonEvent += GameWonListener;
    }
    #region OutSourceMethods
    private void GameOverListener(object sender, EventArgs e)
    {
        GameOverUI?.SetActive(true);
    }
    private void GameWonListener(object sender, GameWonEventArgs e)
    {
        GameWinUI?.SetActive(true);
        gameScore.text = "SCORE: "+e.LevelScore;
    }
    #endregion

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
