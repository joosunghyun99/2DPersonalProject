using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int highScore = 0;
    public int score = 0;
    public int playerHp = 0;
    public int selectedCharacter = 0;
    public Vector2 originalPlayerPos = new Vector2(-7.0f, 0.0f);

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
        UIManager.Instance.InitScene();
    }

    public void AddScore(int amount) 
    {
        score += amount;
        UIManager.Instance.UpdateScore(score);
    }

    public void GameOver() 
    {
        if (score > highScore) 
        {
            highScore = score;
        }
        UIManager.Instance.PopUpResult(highScore, score);
        ResetObject();
    }

    public void GameStart() 
    {
        UIManager.Instance.StartGame();
    }

    public void PlayerHpUpdate(int hp) 
    {
        UIManager.Instance.UpdateHpSlider(hp);
    }

    public void SelectCharacter(int index) 
    {
        selectedCharacter = index;
    }

    public void ResetObject()
    {
        GameObject[] gameObjects = GameObject.FindObjectsOfType<GameObject>();
        string[] targetTags = { "Item", "Ground", "Obstacle" };


        foreach (GameObject gameObject in gameObjects) 
        {
            if (targetTags.Contains(gameObject.tag)) 
            {
                gameObject.SendMessage("ReturnPool", SendMessageOptions.DontRequireReceiver);
            }
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = originalPlayerPos;
    }
}
