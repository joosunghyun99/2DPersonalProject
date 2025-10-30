using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private Coin coinPrefab;
    [SerializeField] private Potion potionPrefab;
    [SerializeField] private Star starPrefab;
    [SerializeField] private Magnet magnetPrefab;

    [SerializeField] private Platform platformPrefab;
    [SerializeField] private Obstacle obstaclePrefab;

    [SerializeField] private Transform anker;
    private Vector3 nextSpawnPosition = Vector3.zero;

    private SpriteRenderer ankerSr;
    private float screenRightEdge;
    private float ankerLeftEdge;

    public string mapFolder = "Maps";
    public float tileSize = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        PoolManager.Instance.CrestePool(coinPrefab, 30);
        PoolManager.Instance.CrestePool(potionPrefab, 10);
        PoolManager.Instance.CrestePool(starPrefab, 10);
        PoolManager.Instance.CrestePool(magnetPrefab, 10);

        PoolManager.Instance.CrestePool(platformPrefab, 50);
        PoolManager.Instance.CrestePool(obstaclePrefab, 30);

        ankerSr = anker.GetComponent<SpriteRenderer>();
        screenRightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0.0f, 0.0f)).x;

        //anker.position = Vector3.zero;
        LoadRandomMap();
    }

    public void Update()
    {
        ankerLeftEdge = anker.transform.position.x - (ankerSr.bounds.size.x / 2.0f);

        if (ankerLeftEdge <= screenRightEdge) 
        {
            LoadRandomMap();
        }
    }

    void LoadRandomMap()
    {
        TextAsset[] mapFiles = Resources.LoadAll<TextAsset>(mapFolder);
        if (mapFiles.Length == 0)
        {
            Debug.LogError("No CSV files/" + mapFolder);
            return;
        }

        int randomIndex = Random.Range(0, mapFiles.Length);
        TextAsset selectedMap = mapFiles[randomIndex];

        GenerateMap(selectedMap.text);
    }

    void GenerateMap(string csvText)
    {
        string[] lines = csvText.Split('\n');
        nextSpawnPosition = new Vector3(anker.position.x + tileSize, anker.position.y + (lines.Length-1) * tileSize, anker.position.z);
        
        for (int y = 0; y < lines.Length; y++)
        {
            string[] cells = lines[y].Trim().Split(',');
            for (int x = 0; x < cells.Length; x++)
            {
                string cell = cells[x].Trim();
                if (cell == "0") //빈 공간
                {
                    continue;
                }
                Vector3 position = nextSpawnPosition + new Vector3(x * tileSize, -y * tileSize, 0);

                anker.position = position;
                //Debug.Log(anker.position);

                switch (cell)
                {
                    case "1": //플랫폼
                        var platform = PoolManager.Instance.GetFromPool(platformPrefab);
                        if (platform != null)
                        {
                            platform.transform.position = position;
                            platform.gameObject.SetActive(true);
                        }
                        break;
                    case "2": //코인
                        var coin = PoolManager.Instance.GetFromPool(coinPrefab);
                        if (coin != null)
                        {
                            coin.transform.position = position;
                            coin.gameObject.SetActive(true);
                        }
                        break;
                    case "3": //포션
                        var potion = PoolManager.Instance.GetFromPool(potionPrefab);
                        if (potion != null)
                        {
                            potion.transform.position = position;
                            potion.gameObject.SetActive(true);
                        }
                        break;
                    case "4": //스타
                        var star = PoolManager.Instance.GetFromPool(starPrefab);
                        if (star != null)
                        {
                            star.transform.position = position;
                            star.gameObject.SetActive(true);
                        }
                        break;
                    case "5": //자석
                        var magnet = PoolManager.Instance.GetFromPool(magnetPrefab);
                        if (magnet != null)
                        {
                            magnet.transform.position = position;
                            magnet.gameObject.SetActive(true);
                        }
                        break;
                    case "6": //장애물
                        var obstacle = PoolManager.Instance.GetFromPool(obstaclePrefab);
                        if (obstacle != null)
                        {
                            obstacle.transform.position = position;
                            obstacle.gameObject.SetActive(true);
                        }
                        break;
                }
            }
        }
    }
}
