using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject lobbySceneUI;
    [SerializeField] private GameObject gameSceneUI;

    [Header("GameScene")]
    [SerializeField] private TextMeshProUGUI curScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI newScoreText;

    [SerializeField] private Slider hpSlider;

    [SerializeField] private GameObject resultPanel;

    [Header("LobbyScene")]
    [SerializeField] private TextMeshProUGUI characterInfoText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI characterSelectText;

    [SerializeField] private Slider effectSoundSlider;
    [SerializeField] private Slider bgmSlider;

    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject characterPanel;
    [SerializeField] private GameObject shopPanel;

    [SerializeField] private Image characterImage;

    [Header("Shop")]
    [SerializeField] private ProductCard productCardPrefab;
    [SerializeField] private ProductData[] productDataList;
    [SerializeField] private GameObject scrollviewContent;

    [SerializeField] private GameObject noticePopUp;
    [SerializeField] private TextMeshProUGUI noticeText;

    private List<CharacterData> characterOwned = new List<CharacterData>();
    private int characterIndex = 0;
    private int selectedCharacter = 0;

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

    private void Start()
    {
        PoolManager.Instance.CreatePool(productCardPrefab, 8);
        ProductRefresh();

        characterOwned = GameManager.Instance.characterOwned;
        characterImage.sprite = characterOwned[0].charSprite;
        characterInfoText.text = $"{characterOwned[0].charName}\n{characterOwned[0].charDescription}";
        characterSelectText.text = "Selected";
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitScene();
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

    public void NoticePopUp(string warning)
    {
       noticePopUp.SetActive(true);
        noticeText.text = warning;
    }

    public void IndexChange(int index) 
    {
        characterOwned = GameManager.Instance.characterOwned;
        characterIndex += index;
        if (characterIndex < 0)
        {
            characterIndex = characterOwned.Count - 1;
        }
        else if (characterIndex >= characterOwned.Count)
        {
            characterIndex = 0;
        }

        if (characterIndex == selectedCharacter) 
        {
            characterSelectText.text = "Selected";
        }
        else
        {
            characterSelectText.text = "Select";
        }

        characterImage.sprite = characterOwned[characterIndex].charSprite;
        characterInfoText.text = $"{characterOwned[characterIndex].charName}\n{characterOwned[characterIndex].charDescription}";
    }

    public void UpdateGold() 
    {
        int gold = GameManager.Instance.gold;
        goldText.text = $"{gold}G";
    }

    public void CharacterSelect() 
    {
        selectedCharacter = characterIndex;
        characterSelectText.text = "Selected";
        GameManager.Instance.SelectCharacter(characterIndex);
    }

    public void UpdateScore(int newScore) 
    {
        curScoreText.text = $"Score : {newScore}";
    }

    public void UpdateHpSlider(int curHp) 
    {
        float max = hpSlider.maxValue;
        hpSlider.value = Mathf.Clamp(curHp,0,max);
    }

    public void SetHpSlider(int curHp, int maxHp)
    {
        hpSlider.maxValue = maxHp;
        hpSlider.value = Mathf.Clamp(curHp, 0, maxHp);
    }

    public void UpdateVolume(int num) 
    {
        if (num == 0)
        {
            SoundManager.Instance.SetVolume(0, effectSoundSlider.value);
        }
        else if (num == 1)
        {
            SoundManager.Instance.SetVolume(1, bgmSlider.value);
        }
    }

    public void ProductRefresh()
    {
        productDataList = Resources.LoadAll<ProductData>("Data/ProductData");

        for (int i = 0; i < productDataList.Length; i++)
        {
            var productCard = PoolManager.Instance.GetFromPool(productCardPrefab);
            productCard.CardRefresh(productDataList[i]);
            productCard.transform.SetParent(scrollviewContent.transform);
        }
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

    public void ExitGame() 
    {
        Application.Quit();
    }

    public void InitScene() 
    {
        resultPanel.SetActive(false);

        gameSceneUI.SetActive(false);
        lobbySceneUI.SetActive(false);

        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            gameSceneUI.SetActive(true);
            if (GameManager.Instance != null)
                GameManager.Instance.GameStart();
            UpdateScore(0);
        }
        else
        {
            lobbySceneUI.SetActive(true);
            UpdateGold();
        }

        Time.timeScale = 1.0f;
    }
}
