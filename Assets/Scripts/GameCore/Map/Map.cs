#define TEST_NOT_EDITOR

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CustomExtensions;
using Utils;

[RequireComponent(typeof(TerrainMeshGenerator))]
public class Map : Singleton<Map>, IInitialisable, ISaveAndLoadable
{
    // the spacing between each point on the map
    [SerializeField] float mapFidelity = 1;
    // point defining the boundary of the map
    [SerializeField] List<Vector2> boundaryPoints;
    [SerializeField] Gradient colorGradient;

    [SerializeField] bool drawGizmos;

    // list of all the points inside the map
    List<MapPoint> m_mapPointsList;

    public MapPoint[] MapPointsArr
    {
        get
        {
            MapPoint[] tempArr = new MapPoint[m_mapPointsList.Count];
            m_mapPointsList.CopyTo(tempArr, 0);
            return tempArr;
        }
    }

    // transforms of the colliders used for inside / outside map testing
    List<Transform> boundaryTransforms;
    // physics layer of the boundary colliders
    const int physicsLayer = 9;
    // generates the terrain mesh
    TerrainMeshGenerator terrainMeshGenerator;

    void Awake()
    {
        Init();
    }

    public void Init()
    {
#if !(UNITY_EDITOR && !TEST_NOT_EDITOR)
        LoadData();
#endif

#if UNITY_EDITOR && !TEST_NOT_EDITOR
        UpdateBoundaryCollider();
        CreateContainedGrid();
        SaveData();
#endif

        GenerateTerrain();
    }

    public bool Contains(Vector2 point)
    {
        // raycast in a random direction
        RaycastHit[] hits = Physics.RaycastAll(point.XY2XZ(), Random.onUnitSphere.SetY(0), 1000, 1 << physicsLayer);

        if (hits.Any(r => r.collider.name.Contains("Corner")))
        {
//            print("Hit corner. Trying again");
            // ray has hit a corner so try again to avoid hitting unioned walls
            return Contains(point);
        }

        // if raycast hits an edge collider an odd number of times point is inside map
        return hits.Length % 2 == 1;
    }

#region ISaveAndLoadable

    public void SaveData()
    {
        MapData mapData = new MapData
        {
            mapFidelity = mapFidelity,
            boundaryPoints = boundaryPoints,
            mapPointsList = m_mapPointsList,
            colorGradient = colorGradient
        };

        FileUtil.SaveToResources("map_data", mapData);
    }

    public void LoadData()
    {
        MapData mapData = FileUtil.LoadFromResources<MapData>("map_data");

        if (mapData == null)
        {
            OnLoadDataFailed();
            return;
        }

        mapFidelity = mapData.mapFidelity;
        boundaryPoints = mapData.boundaryPoints;
        m_mapPointsList = mapData.mapPointsList;
        colorGradient = mapData.colorGradient;
    }

    public void OnLoadDataFailed()
    {
        UpdateBoundaryCollider();

        CreateContainedGrid();
        SaveData();

        LogUtil.WriteWarning("Data failed to load.");
    }

    #endregion

    void GenerateTerrain()
    {
        terrainMeshGenerator = gameObject.GetComponent<TerrainMeshGenerator>();
        terrainMeshGenerator.Init();

#if UNITY_EDITOR && !TEST_NOT_EDITOR
        terrainMeshGenerator.Generate(MapPointsArr);
        terrainMeshGenerator.SaveData();
#else
        terrainMeshGenerator.LoadData();
#endif
    }

    void UpdateBoundaryCollider()
    {
        if (boundaryTransforms != null)
        {
            // clear previous edges
            foreach (Transform boundaryPart in boundaryTransforms)
                Destroy(boundaryPart.gameObject);

            boundaryTransforms.Clear();
        }
        else
        {
            boundaryTransforms = new List<Transform>();
        }
        // create boundary
        for (int v = 0; v < boundaryPoints.Count; v++)
        {
            CreateEdge(v);
            CreateCorner(v);
        }
    }

    void CreateEdge(int vertexIndex)
    {
        int nextPointIndex = (vertexIndex + 1) % boundaryPoints.Count;

        // create child edge gameobject
        GameObject edgeObj = new GameObject("Edge_" + vertexIndex);
        edgeObj.transform.SetParent(transform);
        edgeObj.layer = physicsLayer;

        Vector3 point0 = boundaryPoints[vertexIndex].XY2XZ();
        Vector3 point1 = boundaryPoints[nextPointIndex].XY2XZ();

        // find edge parameters
        Vector3 edgeCenter = (point0 + point1) / 2;
        Vector3 edgeVector = point1 - point0;
        float edgeLength = Vector3.Magnitude(edgeVector);

        // set position
        edgeObj.transform.position = edgeCenter;
        edgeObj.transform.localScale = new Vector3(edgeLength, 1, 0.01f);
        edgeObj.transform.right = edgeVector;

        // add collider for raycast testing
        edgeObj.AddComponent<BoxCollider>();

        boundaryTransforms.Add(edgeObj.transform);
    }

    void CreateCorner(int vertexIndex)
    {
        // create child corner gameobject
        GameObject cornerObj = new GameObject("Corner_" + vertexIndex);
        cornerObj.transform.SetParent(transform);
        cornerObj.layer = physicsLayer;

        Vector3 vertexPos = boundaryPoints[vertexIndex].XY2XZ();

        // set position
        cornerObj.transform.position = vertexPos;

        // add collider for raycast testing
        CapsuleCollider cornerCapsuleCollider = cornerObj.AddComponent<CapsuleCollider>();
        cornerCapsuleCollider.radius = 0.05f;
        cornerCapsuleCollider.height = 1.2f;
    }

    void CreateContainedGrid()
    {
        m_mapPointsList = new List<MapPoint>();

        // get bounds
        Vector2 minCorner, maxCorner;
        minCorner.x = boundaryPoints.Min(v => v.x);
        minCorner.y = boundaryPoints.Min(v => v.y);
        maxCorner.x = boundaryPoints.Max(v => v.x);
        maxCorner.y = boundaryPoints.Max(v => v.y);

        for (float y = minCorner.y; y <= maxCorner.y; y += mapFidelity)
        {
            for (float x = minCorner.x; x <= maxCorner.x; x += mapFidelity)
            {
                Vector2 point = new Vector2(x, y);
                if (Contains(point))
                {
                    float height = Mathf.PerlinNoise(x / 5, y / 5);
                    Color pointColour = colorGradient.Evaluate(height);
                    MapPoint m = new MapPoint(point, height, pointColour);
                    m_mapPointsList.Add(m);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.red;
            foreach (var point in boundaryPoints)
            {
                Gizmos.DrawSphere(point.XY2XZ(), 0.5f);
            }

            if (Application.isPlaying && m_mapPointsList != null)
            {
                foreach (var mapPoint in m_mapPointsList)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(mapPoint.PositionXYZ, mapFidelity / 5);
                }
            }
        }
    }
}