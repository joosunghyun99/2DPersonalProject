using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<CharacterData> characterOwned = new List<CharacterData>();
    private CharacterData[] characterDatas;

    public int highScore = 0;
    public int score = 0;
    public int selectedCharacter = 0;
    public Vector2 originalPlayerPos = new Vector2(-7.0f, 0.0f);

    public int gold = 200;

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

        characterDatas = Resources.LoadAll<CharacterData>("Data/CharacterData");
        characterOwned.Add(characterDatas[0]);
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
        SoundManager.Instance.BGMOnOff(0);
        GetReward();
        ResetObject();
    }

    public void GameStart() 
    {
        score = 0;
        if(SoundManager.Instance != null)
        SoundManager.Instance.BGMOnOff(1);
    }

    public void GetReward() 
    {
        gold += score/10;
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

    public void PurchaseCharacter(ProductCard product) 
    {
        if (gold >= product.price)
        {
            for (int i = 0; i < characterDatas.Length; i++)
            {
                if (product.productId == characterDatas[i].charID)
                {
                    characterOwned.Add(characterDatas[i]);
                    gold -= product.price;
                    product.SoldOut();
                    UIManager.Instance.UpdateGold();
                    break;
                }
            }
        }
        else
        {
            UIManager.Instance.NoticePopUp("Not Enough Gold!");
        }
    }
}
