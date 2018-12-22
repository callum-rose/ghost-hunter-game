using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CustomExtensions;
using TMPro;

public class Map : Singleton<Map> 
{
    // the spacing between each point on the map
    [SerializeField] float mapFidelity = 1;
    // point defining the boundary of the map
    [SerializeField] List<Vector2> boundaryPoints;

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

    // map container transform
    Transform mapTrans;
    // transforms of the colliders used for inside / outside map testing
    List<Transform> boundaryTransforms;
    // physics layer of the boundary colliders
    const int physicsLayer = 9;
    
    void Awake()
    {
        CreateMap();

        UpdateBoundaryCollider();
        CreateContainedGrid();
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

    void CreateMap()
    {
        mapTrans = new GameObject("Map").transform;
        mapTrans.position = Vector3.zero;
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
        edgeObj.transform.SetParent(mapTrans);
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
        cornerObj.transform.SetParent(mapTrans);
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
                    MapPoint m = new MapPoint(point);
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
                }
            }
        }
    }
}

[System.Serializable]
public struct MapPoint
{
    public Vector2 Position { get; private set; }

    public MapPoint(Vector2 position)
    {
        Position = position;
    }
}