using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour {

    [SerializeField] GameObject ghostPrefab;

    GameObject[] activeGhostArr;

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
        GhostSpawnManager.OnGhostSpawn += OnGhostSpawned;
    }

    void OnDestroy()
    {
        GhostSpawnManager.OnGhostSpawn += OnGhostSpawned;
    }

    void OnGhostSpawned()
    {
        //activeGhostArr = GameObject.FindGameObjectsWithTag("Ghost");
    }
}