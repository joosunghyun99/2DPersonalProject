using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI curScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI newScoreText;

    [SerializeField] private Slider hpSlider;

    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject lobbySceneUI;
    [SerializeField] private GameObject gameSceneUI;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        resultPanel.SetActive(false);

        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            lobbySceneUI.SetActive(false);
            gameSceneUI.SetActive(true);
        }
        else
        {
            lobbySceneUI.SetActive(true);
            gameSceneUI.SetActive(false);
        }
    }

    public void UpdateScore(int newScore) 
    {
        curScoreText.text = $"Score : {newScore}";
    }

    public void UpdateHpSlider(int hp) 
    {
        hpSlider.value = hp;
    }

    public void Quit()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void Restart() 
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("GameScene");
    }

    public void StartGame() 
    {
        SceneManager.LoadScene("GameScene");
    }

    public void PopUpResult(int highScore, int newScore) 
    {
        Time.timeScale = 0.0f;
        resultPanel.SetActive(true);
        highScoreText.text = $"HighScore : {highScore}";
        newScoreText.text = $"Score : {newScore}";
    }
}
