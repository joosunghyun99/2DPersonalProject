using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
   public static PoolManager Instance { get; private set; }

    private static Dictionary<string, object> pools = new Dictionary<string, object>();

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

    public void CrestePool<T>(T prefab, int generateCount, Transform parent = null) where T : MonoBehaviour 
    {
        if (prefab == null) return;

        string key = prefab.name;
        if (pools.ContainsKey(key)) return;

        pools.Add(key, new ObjectPool<T>(prefab, generateCount, parent));
    }

    public T GetFromPool<T>(T prefab) where T : MonoBehaviour 
    {
        if (prefab == null) return null;

        if (!pools.TryGetValue(prefab.name, out var box)) 
        {
            return null;
        }

        var pool = box as ObjectPool<T>;

        if (pool != null)
        {
            return pool.Dequeue();
        }
        else 
        {
            return null;
        }
    }

    public void ReturnPool<T>(T instance, T prefab) where T : MonoBehaviour 
    {
        if (instance == null || prefab == null) return;

        if (!pools.TryGetValue(prefab.name, out var box)) 
        {
            Destroy(instance.gameObject);
            return;
        }

        var pool = box as ObjectPool<T>;

        if (pool != null) 
        {
            pool.Enqueue(instance);
        }
    }
}
