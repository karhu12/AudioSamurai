using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T>
{
    public int Size { get; private set; }

    private Poolable<T> prefab;
    private Transform poolParent;
    private Queue<Poolable<T>> objectPool = new Queue<Poolable<T>>();
     
    public ObjectPool(Poolable<T> poolable, Transform parent, int initialSize = 5)
    {
        poolParent = parent;
        Size = initialSize;
        prefab = poolable;
        for (int sz = 0; sz < initialSize; sz++)
        {
            InstantiateNewPoolable();
        }
    }

    public Poolable<T> Get()
    {
        if (objectPool.Count == 0)
            InstantiateNewPoolable();
        Poolable<T> obj = objectPool.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }

    /* */
    public void ReturnToPool(Poolable<T> poolable)
    {
        poolable.gameObject.SetActive(false);
        objectPool.Enqueue(poolable);
    }

    /* Private methods */

    /*
     * Instantiates a new game object from given prefab, adds it to the pool and returns it.
     */
    private Poolable<T> InstantiateNewPoolable()
    {
        Poolable<T> obj = MonoBehaviour.Instantiate<Poolable<T>>(prefab);
        obj.transform.parent = poolParent;
        obj.transform.position = poolParent.position;
        obj.gameObject.SetActive(false);
        obj.Pool = this;
        objectPool.Enqueue(obj);
        return obj;
    }
}

public class Poolable<T> : MonoBehaviour
{
    public ObjectPool<T> Pool { get; set; }

    public void ReturnToPool()
    {
        if (Pool != null)
            Pool.ReturnToPool(this);
        Destroy(this);
    }
}
