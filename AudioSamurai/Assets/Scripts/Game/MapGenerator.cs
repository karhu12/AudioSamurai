using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    /* Constants */
    public const float SCALED_SIZE = 800;
    public const float PLACEMENT_OFFSET = 16;
    public const int SPAWN_AHEAD_OF_TARGET = 3;

    public GameObject generationTarget;
    public GameObject generationParent;
    public List<Generatable> tilePrefabs;
    public Dictionary<string, ObjectPool> tileObjectPools = new Dictionary<string, ObjectPool>();
    public Quaternion defaultOrientation;
    public Direction generationDirection;

    private List<Generatable> spawnedTiles = new List<Generatable>();
    private string currentGenerationType;

    public enum Direction { X, Y, Z }

    public enum GenerationState
    {
        NotGenerating,
        Generating
    }

    private void Awake()
    {
        State = GenerationState.NotGenerating;
    }

    public GenerationState State { get; private set; }

    private void Start()
    {
        foreach (var tile in tilePrefabs)
        {
            tileObjectPools.Add(tile.name, new ObjectPool(tile, generationParent.transform, 3));
        }
        StartGeneration("Background");
    }

    public void StartGeneration(string tileType)
    {
        if (tileObjectPools.ContainsKey(tileType))
        {
            currentGenerationType = tileType;
            State = GenerationState.Generating;
        }
    }

    public void StopGeneration()
    {
        State = GenerationState.NotGenerating;
    }

    private void Update()
    {
        if (State == GenerationState.Generating)
        {
            if (NeedsSpawn())
            {
                Generatable obj = spawnedTiles.FindLast((tile) => GetTargetDirectionPos(tile) > GetTargetDirectionPos(generationTarget));

                float targetPos = GetTargetDirectionPos(generationTarget);
                if (obj == null)
                {
                    for (int i = 0; i < SPAWN_AHEAD_OF_TARGET; i++)
                    {
                        Generatable tile = (Generatable)tileObjectPools[currentGenerationType].Get();
                        SetTargetDirectionPos(tile.gameObject, targetPos + i * SPAWN_AHEAD_OF_TARGET);
                        spawnedTiles.Add(tile);
                    }
                }
                else
                {
                    Generatable newTile = (Generatable)tileObjectPools[currentGenerationType].Get();
                    SetTargetDirectionPos(newTile.gameObject, GetTargetDirectionPos(obj.gameObject) + PLACEMENT_OFFSET);
                    spawnedTiles.Add(newTile);
                }
            }

            List<Generatable> removeList = new List<Generatable>();
            foreach (var tile in spawnedTiles)
            {
                if (GetTargetDirectionPos(tile) - GetTargetDirectionPos(generationTarget) <= -PLACEMENT_OFFSET)
                {
                    tile.ReturnToPool();
                    removeList.Add(tile);
                }
            }
            foreach (var remove in removeList) { spawnedTiles.Remove(remove); }
        }
    }

    private bool NeedsSpawn()
    {
        bool needsNewSpawn = true;
        foreach (var spawned in spawnedTiles)
        {
            float offset = GetTargetDirectionPos(spawned) - GetTargetDirectionPos(generationTarget);

            if (offset >= SPAWN_AHEAD_OF_TARGET * PLACEMENT_OFFSET)
                needsNewSpawn = false;

            if (!needsNewSpawn)
                break;
        }
        return needsNewSpawn;
    }

    private void Orientate(GameObject tile)
    {
        Vector3 objectSize = Vector3.Scale(tile.transform.localScale, tile.gameObject.GetComponent<Mesh>().bounds.size);
        tile.transform.localScale.Set(
            (objectSize.x < SCALED_SIZE) ? (objectSize.x / SCALED_SIZE) * objectSize.x : 0,
            (objectSize.y < SCALED_SIZE) ? (objectSize.y / SCALED_SIZE) * objectSize.y : 0,
            (objectSize.z < SCALED_SIZE) ? (objectSize.z / SCALED_SIZE) * objectSize.z : 0
        );
        tile.transform.SetPositionAndRotation(new Vector3(0, 0, 0), defaultOrientation);
    }

    private float GetTargetDirectionPos(GameObject target)
    {
        switch (generationDirection)
        {
            case Direction.X:
                return target.transform.position.x;
            case Direction.Y:
                return target.transform.position.y;
            case Direction.Z:
                return target.transform.position.z;
        }
        generationDirection = Direction.Z;
        return target.transform.position.z;
    }

    private float GetTargetDirectionPos(Generatable target)
    {
        return GetTargetDirectionPos(target.gameObject);
    }

    private void SetTargetDirectionPos(GameObject target, float pos)
    {
        switch (generationDirection)
        {
            case Direction.X:
                target.transform.position = new Vector3(pos, 0, 0);
                break;
            case Direction.Y:
                target.transform.position = new Vector3(0, pos, 0);
                break;
            case Direction.Z:
                target.transform.position = new Vector3(0, 0, pos);
                break;
        }
    }
}