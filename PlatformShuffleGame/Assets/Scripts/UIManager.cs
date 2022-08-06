using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject GameOverUI;

    private void Awake()
    {
        GameOverUI?.SetActive(false);
    }
    private void Start()
    {
        GameManager.Instance.GameOverEvent += GameOverListener;
    }
    #region OutSourceMethods
    private void GameOverListener(object sender, EventArgs e)
    {
        GameOverUI?.SetActive(true);
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
