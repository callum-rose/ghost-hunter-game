using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomExtensions;

public class RadarManager : MonoBehaviour, IInitialisable {

    //[SerializeField] PlayerLocation playerLocation;
    [SerializeField] float world2ScreenScale = 20;
    [SerializeField] GameObject ghostDotPrefab;

    Transform radarContainer;

    Ghost[] ghosts;
    // link dictionary between the actual ghost and the radar ghost
    Dictionary<Ghost, GameObject> radarGhostsLinkDict;

    void Awake()
    {
        ghosts = new Ghost[0];
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        radarContainer = transform.Find("RadarContainer");
    }

    void OnEnable()
    {
        GhostManager.OnGhostsUpdated += UpdateGhosts;
        PlayerLocation.OnPlayerMoved += UpdateGhostPositions;
    }

    void OnDisable()
    {
        GhostManager.OnGhostsUpdated -= UpdateGhosts;
        PlayerLocation.OnPlayerMoved -= UpdateGhostPositions;
    }

    void OnDestroy()
    {
        GhostManager.OnGhostsUpdated -= UpdateGhosts;
        PlayerLocation.OnPlayerMoved -= UpdateGhostPositions;
    }

    void UpdateGhosts(Ghost[] allGhosts)
    {
        ghosts = allGhosts;
        radarGhostsLinkDict = new Dictionary<Ghost, GameObject>();

        foreach (var ghost in ghosts)
        {
            GameObject radarGhost = Instantiate(ghostDotPrefab, radarContainer);
            //radarGhost.transform.localPosition = (ghost.transform.position.XZ2XY() - playerLocation.Location) * 20;
            radarGhostsLinkDict.Add(ghost, radarGhost);
        }
    }

    void UpdateGhostPositions(Vector2 playerPos)
    {
        foreach (var ghost in ghosts)
        {
            GameObject radarGhost = radarGhostsLinkDict[ghost];
            radarGhost.transform.localPosition = (ghost.transform.position.XZ2XY() - playerPos) * world2ScreenScale;
        }
    }
}
