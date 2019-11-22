using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    public int Size { get; private set; }

    private Poolable prefab;
    private Transform poolParent;
    private Queue<Poolable> objectPool = new Queue<Poolable>();
     
    public ObjectPool(Poolable poolable, Transform parent, int initialSize = 5)
    {
        Size = 0;
        poolParent = parent;
        prefab = poolable;
        for (int sz = 0; sz < initialSize; sz++)
        {
            InstantiateNewPoolable();
        }
    }

    public Poolable Get()
    {
        if (objectPool.Count == 0)
            InstantiateNewPoolable();

        Poolable obj = objectPool.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }

    public int GetAvailableCount()
    {
        return objectPool.Count;
    }

    public int GetActiveCount()
    {
        int count = 0;
        foreach (var obj in objectPool) {
            if (obj.gameObject.activeSelf)
                count++;
        }
        return count;
    }

    /* */
    public void ReturnToPool(Poolable poolable)
    {
        poolable.gameObject.SetActive(false);
        objectPool.Enqueue(poolable);
    }

    /* Private methods */

    /*
     * Instantiates a new game object from given prefab, adds it to the pool and returns it.
     */
    private Poolable InstantiateNewPoolable()
    {
        Poolable obj = MonoBehaviour.Instantiate<Poolable>(prefab);
        obj.transform.SetParent(poolParent, false);
        obj.transform.position = poolParent.position;
        obj.gameObject.SetActive(false);
        obj.Pool = this;
        objectPool.Enqueue(obj);
        Size += 1;
        return obj;
    }
}

public class Poolable : MonoBehaviour
{
    public ObjectPool Pool { get; set; }

    public virtual void ReturnToPool()
    {
        if (Pool != null)
            Pool.ReturnToPool(this);
        else
            Destroy(this);
    }
}
