using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("GameScene")]
    [SerializeField] private TextMeshProUGUI curScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI newScoreText;

    [SerializeField] private Slider hpSlider;

    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject lobbySceneUI;
    [SerializeField] private GameObject gameSceneUI;

    [Header("LobbyScene")]
    [SerializeField] private TextMeshProUGUI characterInfoText;

    [SerializeField] private Slider effectSoundSlider;
    [SerializeField] private Slider bgmSlider;

    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject characterPanel;
    [SerializeField] private GameObject shopPanel;

    [SerializeField] private Image characterImage;

    [SerializeField] private Sprite[] characterSprite;
    private int characterIndex = 0;

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
    }

    public void PopUp(GameObject panel) 
    {
        if (panel.activeSelf == true)
        {
            panel.SetActive(false);
        }
        else 
        {
            panel.SetActive(true);
        }
    }

    public void IndexChange(int index) 
    {
        characterIndex += index;
        if (characterIndex < 0)
        {
            characterIndex = characterSprite.Length - 1;
        }
        else if (characterIndex >= characterSprite.Length)
        {
            characterIndex = 0;
        }
        characterImage.sprite = characterSprite[characterIndex];
    }

    public void CharacterSelect() 
    {
        GameManager.Instance.SelectCharacter(characterIndex);
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

    public void InitScene() 
    {
        resultPanel.SetActive(false);

        gameSceneUI.SetActive(false);
        lobbySceneUI.SetActive(false);

        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            gameSceneUI.SetActive(true);
        }
        else
        {
            lobbySceneUI.SetActive(true);
        }

        Time.timeScale = 1.0f;
    }
}
