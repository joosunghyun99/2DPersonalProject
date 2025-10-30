using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int highScore = 0;
    public int score = 0;
    public int playerHp = 0;

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
    }

    public void GameStart() 
    {

    }

    public void PlayerHpUpdate(int hp) 
    {
        UIManager.Instance.UpdateHpSlider(hp);
    }
}
