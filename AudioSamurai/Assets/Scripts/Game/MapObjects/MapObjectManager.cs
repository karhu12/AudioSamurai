﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjectManager : Singleton<MapObjectManager>
{
    public List<MapObject> mapObjectPrefabs;
    public Transform spawnParent;
    private Dictionary<string, ObjectPool<MapObject>> mapObjectPools = new Dictionary<string, ObjectPool<MapObject>>();


    private new void Awake()
    {
        base.Awake();
        foreach (var prefab in mapObjectPrefabs)
        {
            mapObjectPools.Add(prefab.GetMapObjectType(), new ObjectPool<MapObject>(prefab, spawnParent));
        }
    }

    public bool HasActiveObjects()
    {
        foreach (var key in mapObjectPools.Keys)
        {
            if (mapObjectPools[key].GetAvailableCount() != mapObjectPools[key].Size)
            {
                return true;
            }
        }

        return false;
    }

    public MapObject GetMapObject(string type)
    {
        if (mapObjectPools.ContainsKey(type))
        {
            return (MapObject)mapObjectPools[type].Get();
        }
        return null;
    }

    public List<string> GetMapObjectTypes()
    {
        List<string> objTypes = new List<string>();
        foreach (var obj in mapObjectPrefabs)
        {
            string type = obj.GetMapObjectType();
            if (!objTypes.Contains(type))
            {
                objTypes.Add(type);
            }
        }
        return objTypes;
    }
}
