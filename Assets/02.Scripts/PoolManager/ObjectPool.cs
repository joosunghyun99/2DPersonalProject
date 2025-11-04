using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private T prefab;
    private Queue<T> pool = new Queue<T>();

    public Transform Root { get; private set; }

    public ObjectPool(T prefab, int generateCount, Transform parent = null)
    {
        this.prefab = prefab;
        Root = new GameObject($"{prefab.name}_pool").transform;
        Object.DontDestroyOnLoad(Root.gameObject);

        if (parent != null)
        {
            Root.SetParent(parent, false);
        }

        for (int i = 0; i < generateCount; i++)
        {
            var instance = Object.Instantiate(prefab, Root);
            instance.name = prefab.name;
            instance.gameObject.SetActive(false);
            pool.Enqueue(instance);
        }
    }

    public T Dequeue()
    {
        if (pool.Count == 0) return null;

        var instance = pool.Dequeue();

        if (instance == null || instance.gameObject == null)
        {
            return null;
        }
        else 
        {
            instance.gameObject.SetActive(true);
            return instance;
        }
    }

    public void Enqueue(T instance)
    {
        if (instance == null) return;

        instance.gameObject.SetActive(false);
        pool.Enqueue(instance);
    }
}
