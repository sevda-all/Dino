using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler
{
    public static ObjectPooler Instance => _instance;
    private static ObjectPooler _instance;

    private readonly Dictionary<int, Pool> m_PoolDictionary;
    
    private const int DefaultCapacity = 20;

    public ObjectPooler()
    {
        if (_instance == null)
            _instance = this;
        else
            Debug.LogError("Attempt to try to create another singleton instance - " + GetType().Name);

        m_PoolDictionary = new Dictionary<int, Pool>();
    }
    
    public void SetPoolCapacity(int poolId, int capacity)
    {
        if (m_PoolDictionary.ContainsKey(poolId))
        {
            var pool = m_PoolDictionary[poolId];
            pool.capacity = capacity;
            m_PoolDictionary[poolId] = pool;
        }
        else
        {
            AddNewPool(poolId);
            var pool = m_PoolDictionary[poolId];
            pool.capacity = capacity;
            m_PoolDictionary[poolId] = pool;
        }
    }

    private void AddNewPool(int poolId, int poolCapacity = DefaultCapacity)
    {
        var objectPool = new List<GameObject>();
        var poolParent = new GameObject("Pool" + poolId);
        var pool = new Pool { objects = objectPool, capacity = poolCapacity, parent = poolParent.transform };
        m_PoolDictionary.Add(poolId, pool);
    }
    
    public GameObject SpawnFromPool(GameObject prefab, Vector3 spawnPosition, int poolCapacity = DefaultCapacity, Transform parent = null)
    {
        var poolId = prefab.GetHashCode();

        // Creating new pool if needed
        if (!m_PoolDictionary.ContainsKey(poolId))
        {
            AddNewPool(poolId, poolCapacity);
        }
        var pool = m_PoolDictionary[poolId];

        // Getting pooling object instance if it is free or creating new one otherwise
        var poolingObj = pool.objects.Find(obj => (obj != null) && obj.GetComponent<IPoolingObject>().IsFree);
        if (poolingObj == null)
        {
            if (pool.objects.Count < pool.capacity)
                poolingObj = AddInstance(prefab, spawnPosition, poolId);
            else
            {
                Debug.Log("Pool is overfill");
                return null;
            }
        }

        // Configure object from pool 
        poolingObj.transform.position = spawnPosition;
        if (parent) 
            poolingObj.transform.SetParent(parent);
        poolingObj.SetActive(true);
        poolingObj.GetComponent<IPoolingObject>().OnObjectSpawn();
        return poolingObj;
    }

    private GameObject AddInstance(GameObject prefab, Vector3 spawnPosition, int poolId)
    {
        var obj = Object.Instantiate(prefab, spawnPosition, Quaternion.identity);
        m_PoolDictionary[poolId].objects.Add(obj);
        obj.SetActive(true);
        obj.GetComponent<IPoolingObject>().OnObjectSpawn();
        return obj;
    }

    private void RemoveInstance(GameObject prefab)
    {
        var poolId = prefab.GetHashCode();
        if (m_PoolDictionary.ContainsKey(poolId))
        {
            m_PoolDictionary[poolId].objects.Remove(prefab);
        }
    }
}

public interface IPoolingObject
{
    bool IsFree { get; }
    void OnObjectSpawn();
}

internal struct Pool
{
    internal List<GameObject> objects;
    internal int capacity;
    internal Transform parent;
}
