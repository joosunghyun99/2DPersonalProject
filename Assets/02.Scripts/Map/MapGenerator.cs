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

    // Start is called before the first frame update
    void Start()
    {
        PoolManager.Instance.CrestePool(coinPrefab, 30);
        PoolManager.Instance.CrestePool(potionPrefab, 10);
        PoolManager.Instance.CrestePool(starPrefab, 10);
        PoolManager.Instance.CrestePool(magnetPrefab, 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
