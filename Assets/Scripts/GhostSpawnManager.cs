using UnityEngine;
using System.Collections;
using CustomExtensions;

public class GhostSpawnManager : MonoBehaviour {

    [SerializeField] GameObject ghostPrefab;

    // in seconds
    public static long spawnIntervalDuration = 1;

    // in seconds
    long lastSpawnTime;
    // number of spawns
    long spawnCount;

    public delegate void SpawnEvent();
    public static event SpawnEvent OnGhostSpawn;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        spawnCount = TimeManager.Seconds / spawnIntervalDuration;

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // to do
        while (true)
        {
            while (!ShouldSpawn())
            {
                // wait until it's time to spawn a new ghost
                yield return new WaitForSeconds(1);
            }

            Spawn();
        }
    }

    void Spawn()
    {
        Random.InitState((int)spawnCount);
        Vector2 position = GhostSpawnPositionManager.Instance.GetPositionAtCumulativeProb(Random.value);

        GameObject ghostObj = Instantiate(ghostPrefab, position.XY2XZ(), Quaternion.identity);
        ghostObj.name = "Ghost_" + spawnCount;

        if (OnGhostSpawn != null)
            OnGhostSpawn();

        Logger.Write("Spawned ghost at " + position);
    }

    bool ShouldSpawn()
    {
        if (TimeManager.Seconds / spawnIntervalDuration > spawnCount)
        {
            spawnCount = TimeManager.Seconds / spawnIntervalDuration;
            return true;
        }
        return false;
    }
}
