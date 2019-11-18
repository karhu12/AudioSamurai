using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
    /* Constants */
    public const float SCALED_SIZE = 800;
    public const float PLACEMENT_OFFSET = 16;
    public const int SPAWN_AFTER_TARGET = 3;
    public const int SPAWN_BEFORE_TARGET = 1;

    public GameObject generationTarget;
    public GameObject generationParent;
    public List<Generatable> tilePrefabs;
    public Dictionary<string, ObjectPool> tileObjectPools = new Dictionary<string, ObjectPool>();

    private List<Generatable> spawnedTiles = new List<Generatable>();
    private string currentGenerationType;

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
        if (State == GenerationState.Generating) {
            List<Generatable> removeList = new List<Generatable>();
            foreach (var tile in spawnedTiles) {
                float diff = tile.transform.position.z - generationTarget.transform.position.z;
                if (diff < 0 && diff > -PLACEMENT_OFFSET * (SPAWN_BEFORE_TARGET + 1))
                    continue;
                else if (diff >= 0 && diff < PLACEMENT_OFFSET * (SPAWN_AFTER_TARGET + 1))
                    continue;
                tile.ReturnToPool();
                removeList.Add(tile);
            }
            foreach (var remove in removeList) { spawnedTiles.Remove(remove); }

            for (int tilePos = -SPAWN_BEFORE_TARGET; tilePos < SPAWN_AFTER_TARGET; tilePos++) {
                float center = generationTarget.transform.position.z + (tilePos * PLACEMENT_OFFSET);
                float backPosition = center - PLACEMENT_OFFSET;
                float forwardPosition = center + PLACEMENT_OFFSET;
                if (!spawnedTiles.Exists(item => item.transform.position.z > backPosition && item.transform.position.z < forwardPosition)) 
                {
                    Generatable g = (Generatable)tileObjectPools[currentGenerationType].Get();
                    g.transform.position = new Vector3(0, 0, (int)center);
                    spawnedTiles.Add(g);
                }
            }
        }
    }
}