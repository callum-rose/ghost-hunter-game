using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : Singleton<GhostManager>, IInitialisable {

    Ghost[] activeGhostArr;

    public delegate void GhostEvent(Ghost[] ghostArr);
    public static event GhostEvent OnGhostsUpdated;

    void Start()
    {
        Init();
    }

    public void Init()
    {
    }

    void OnEnable()
    {
        GhostSpawnManager.OnGhostSpawn += OnGhostSpawned;
    }

    void OnDisable()
    {
        GhostSpawnManager.OnGhostSpawn -= OnGhostSpawned;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        GhostSpawnManager.OnGhostSpawn -= OnGhostSpawned;
    }

    void OnGhostSpawned()
    {
        activeGhostArr = FindObjectsOfType<Ghost>();

        if (OnGhostsUpdated != null)
            OnGhostsUpdated(activeGhostArr);
    }
}