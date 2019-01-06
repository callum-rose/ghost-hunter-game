#define TEST_NOT_EDITOR

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CustomExtensions;
using Utils;

[RequireComponent(typeof(TerrainMeshGenerator))]
public class Map : Singleton<Map>, IInitialisable, ISaveAndLoadable
{
    public string mapName;
    // the spacing between each point on the map
    [SerializeField] float mapFidelity = 1;
    // globe coordinates defining the boundary of the map
    [SerializeField] List<Coordinate> boundaryCoordinates;
    [SerializeField] Gradient colorGradient;

    [SerializeField] bool drawGizmos;

    // world space points defining the boundary
    List<Vector2> m_boundaryPoints;
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
    List<Transform> m_boundaryTransforms;
    // physics layer of the boundary colliders
    const int m_physicsLayer = 9;

    // generates the terrain mesh
    TerrainMeshGenerator m_terrainMeshGenerator;

    void Awake()
    {
        Init();
    }

    public void Init()
    {
        StopwatchUtil.Start();
#if UNITY_EDITOR && !TEST_NOT_EDITOR
        CreateMapData();
#else
        LoadData();
#endif

        GenerateTerrain();
        StopwatchUtil.Stop();
    }

    public bool Contains(Vector2 point)
    {
        // raycast in a random direction
        RaycastHit[] hits = Physics.RaycastAll(point.XY2XZ(), Random.onUnitSphere.SetY(0), 100000, 1 << m_physicsLayer);

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
            boundaryCoords = boundaryCoordinates,
            mapPointsList = m_mapPointsList,
            colourGradient = colorGradient
        };

        if (string.IsNullOrEmpty(mapName))
        {
            LogUtil.WriteWarning("Map name is null or empty. Setting to 'default'");
            mapName = "default";
        }

        FileUtil.SaveAsJsonToResources("map_data_" + mapName, mapData);
    }

    public void LoadData()
    {
        if (string.IsNullOrEmpty(mapName))
        {
            LogUtil.WriteWarning("Map name is null or empty. Setting to 'default'");
            mapName = "default";
        }

        MapData mapData = FileUtil.LoadFromResources<MapData>("map_data_" + mapName);

        if (mapData == null || !mapData.CheckPropertiesValid())
        {
            OnLoadDataFailed();
            return;
        }

        mapFidelity = mapData.mapFidelity;
        boundaryCoordinates = mapData.boundaryCoords;
        m_mapPointsList = mapData.mapPointsList;
        colorGradient = mapData.colourGradient;

        ConvertBoundaryCoords2World();
    }

    public void OnLoadDataFailed()
    {
        LogUtil.WriteWarning("Data failed to load. Regenerating.");

        CreateMapData();
    }

#endregion

    void CreateMapData()
    {
        ConvertBoundaryCoords2World();
        UpdateBoundaryCollider();
        CreateContainedGrid();
        SaveData();
    }

    void GenerateTerrain()
    {
        m_terrainMeshGenerator = gameObject.GetComponent<TerrainMeshGenerator>();
        m_terrainMeshGenerator.Init();
        m_terrainMeshGenerator.mapName = mapName;

#if UNITY_EDITOR && !TEST_NOT_EDITOR
        m_terrainMeshGenerator.Generate(MapPointsArr);
#else
        m_terrainMeshGenerator.LoadData();
#endif
    }

    void ConvertBoundaryCoords2World()
    {
        // convert coordinate to world points
        var tempPoints = boundaryCoordinates.Select(MercatorUtil.FromGlobeToXY);

        Vector2 minCorner;
        minCorner.x = tempPoints.Min(v => v.x);
        minCorner.y = tempPoints.Min(v => v.y);

        m_boundaryPoints = tempPoints.Select(v => v - minCorner).ToList();
    }

    void UpdateBoundaryCollider()
    {
        if (m_boundaryTransforms != null)
        {
            // clear previous edges
            foreach (Transform boundaryPart in m_boundaryTransforms)
                Destroy(boundaryPart.gameObject);

            m_boundaryTransforms.Clear();
        }
        else
        {
            m_boundaryTransforms = new List<Transform>();
        }
        // create boundary
        for (int v = 0; v < m_boundaryPoints.Count; v++)
        {
            int vNext = (v + 1) % m_boundaryPoints.Count;
            Vector3 point0 = m_boundaryPoints[v].XY2XZ();
            Vector3 point1 = m_boundaryPoints[vNext].XY2XZ();

            CreateEdge(point0, point1);
            CreateCorner(point0);
        }
    }

    void CreateEdge(Vector3 point0, Vector3 point1)
    {
        // create child edge gameobject
        GameObject edgeObj = new GameObject("Edge");
        edgeObj.transform.SetParent(transform);
        edgeObj.layer = m_physicsLayer;

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

        m_boundaryTransforms.Add(edgeObj.transform);
    }

    void CreateCorner(Vector3 point)
    {
        // create child corner gameobject
        GameObject cornerObj = new GameObject("Corner");
        cornerObj.transform.SetParent(transform);
        cornerObj.layer = m_physicsLayer;
        
        // set position
        cornerObj.transform.position = point;

        // add collider for raycast testing
        CapsuleCollider cornerCapsuleCollider = cornerObj.AddComponent<CapsuleCollider>();
        cornerCapsuleCollider.radius = 0.05f;
        cornerCapsuleCollider.height = 1.2f;

        m_boundaryTransforms.Add(cornerObj.transform);
    }

    void CreateContainedGrid()
    {
        m_mapPointsList = new List<MapPoint>();

        // get bounds
        Vector2 minCorner, maxCorner;
        minCorner.x = m_boundaryPoints.Min(v => v.x);
        minCorner.y = m_boundaryPoints.Min(v => v.y);
        maxCorner.x = m_boundaryPoints.Max(v => v.x);
        maxCorner.y = m_boundaryPoints.Max(v => v.y);

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
            if (Application.isPlaying && m_mapPointsList != null)
            {
                Gizmos.color = Color.red;
                foreach (var point in m_boundaryPoints)
                {
                    Gizmos.DrawSphere(point.XY2XZ(), 0.5f);
                }

                foreach (var mapPoint in m_mapPointsList)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(mapPoint.PositionXYZ, mapFidelity / 5);
                }
            }
        }
    }
}