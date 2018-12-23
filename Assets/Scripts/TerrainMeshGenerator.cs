using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CustomExtensions;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class TerrainMeshGenerator : MonoBehaviour, IInitialisable
{
    MeshFilter meshFilter;

    public void Init()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    public void Generate(MapPoint[] mapData)
    {
        Mesh terrainMesh = new Mesh();

        MapPoint?[,] mapPointXYData = FormatMapData(mapData);

        List<MapPointLink> linksList = GetLinks(mapPointXYData);
        // order by row
        linksList = linksList.OrderBy(l => l.Centroid.y).ThenBy(l => l.Centroid.x).ToList();

        // find triangles amoungst the links
        List<LinkTri> linkTris = CreateLinkTris(linksList);

        // asign triangle data
        int[] tris = new int[linkTris.Count * 3];
        for (int t = 0; t < linkTris.Count; t++)
        {
            MapPoint[] triPoints = linkTris[t].GetClockwiseMapPointTriangle();
            for (int i = 0; i < 3; i++)
            {
                // triangle point number is the index of the mapPoint
                tris[t * 3 + i] = mapData.IndexOf(triPoints[i]);
            }
        }

        terrainMesh.vertices = mapData.Select(m => m.PositionXYZ).ToArray();
        terrainMesh.triangles = tris;

        terrainMesh.RecalculateNormals();

        meshFilter.mesh = terrainMesh;
    }

    MapPoint?[,] FormatMapData(MapPoint[] mapData)
    {
        Vector2 minWorldMapCorner, maxWorldMapCorner;
        minWorldMapCorner.x = mapData.Min(m => m.Position.x);
        minWorldMapCorner.y = mapData.Min(m => m.Position.y);
        maxWorldMapCorner.x = mapData.Max(m => m.Position.x);
        maxWorldMapCorner.y = mapData.Max(m => m.Position.y);

        // get map width in world space
        float mapWorldWidth = maxWorldMapCorner.x - minWorldMapCorner.x;
        float mapWorldHeight = maxWorldMapCorner.y - minWorldMapCorner.y;

        // get map width in data space
        // split into rows then find the widest row
        int mapDataWidth = mapData.GroupBy(m => m.Position.y).Max(g => g.Count());
        // split into columns and find tallest column
        int mapDataHeight = mapData.GroupBy(m => m.Position.x).Max(g => g.Count());

        // indices refer to x coordinate of the mappoint
        MapPoint?[,] formattedMapData = new MapPoint?[mapDataWidth + 1, mapDataHeight + 1];

        foreach (var mapPoint in mapData)
        {
            // convert world pos into data pos
            int dataXPos = Mathf.FloorToInt(mapDataWidth * ((mapPoint.Position.x - minWorldMapCorner.x) / (mapWorldWidth + 1)));
            int dataYPos = Mathf.FloorToInt(mapDataHeight * ((mapPoint.Position.y - minWorldMapCorner.y) / (mapWorldHeight + 1)));

            //print("x:" + dataXPos);
            //print("y:" + dataYPos);

            // insert into matrix at position
            formattedMapData[dataXPos, dataYPos] = mapPoint;
        }

        return formattedMapData;
    }

    List<LinkTri> CreateLinkTris(List<MapPointLink> linksList)
    {
        List<LinkTri> linkTris = new List<LinkTri>();
        for (int i = 1; i < linksList.Count; i++)
        {
            MapPointLink link0 = linksList[i - 1];
            MapPointLink link1 = linksList[i];
            if (link0.Centroid.y.RoughlyEquals(link1.Centroid.y, 2))
            {
                // on the same row
                if (link0.SharesPointWith(link1))
                {
                    // share a vertex
                    // these links must form a map triangle
                    linkTris.Add(new LinkTri(link0, link1));
                }
            }
        }

        return linkTris;
    }

    //void OnDrawGizmos()
    //{
    //    if (Application.isPlaying)
    //    {
    //        foreach (var l in linksListTest)
    //        {
    //            Gizmos.DrawLine(l.Point0.PositionXYZ, l.Point1.PositionXYZ);
    //        }
    //    }
    //}

    List<MapPointLink> GetLinks(MapPoint?[,] mapData)
    {
        List<MapPointLink> tempList = new List<MapPointLink>();

        // loop through rows
        for (int y = 0; y < mapData.GetLength(1) - 1; y++)
        {
            // loop through points in row
            for (int x = 0; x < mapData.GetLength(0); x++)
            {
                MapPoint? pointCurrent = mapData[x, y];

                if (pointCurrent == null)
                    continue;

                MapPoint? pointLeftOfCurrent = null;
                if (mapData.HasIndex(x - 1, y))
                    pointLeftOfCurrent = mapData[x - 1, y];

                MapPoint? pointRightOfCurrent = null;
                if (mapData.HasIndex(x + 1, y))
                    pointRightOfCurrent = mapData[x + 1, y];

                // don't have to check as we know a space above exists, don't know if not null though
                MapPoint? pointAboveCurrent = mapData[x, y + 1];

                MapPoint? pointAboveLeftOfCurrent = null;
                if (mapData.HasIndex(x - 1, y + 1))
                    pointAboveLeftOfCurrent = mapData[x - 1, y + 1];

                MapPoint? pointAboveRightOfCurrent = null;
                if (mapData.HasIndex(x + 1, y + 1))
                    pointAboveRightOfCurrent = mapData[x + 1, y + 1];

                // check if x values are the same
                if (pointAboveCurrent != null)
                {
                    if (pointCurrent.Value.Position.x.RoughlyEquals(pointAboveCurrent.Value.Position.x, 2))
                    {
                        // There are the only 2 cases where a left leaning link will happen
                        if (pointAboveLeftOfCurrent != null)
                        {
                            if (pointLeftOfCurrent == null)
                            {
                                // Left leaning link: Case 1
                                // o-o 
                                //  \| 
                                // x c 
                                MapPointLink linkLeft = new MapPointLink(pointCurrent.Value, pointAboveLeftOfCurrent.Value);
                                tempList.Add(linkLeft);
                            }
                        }

                        // create link
                        MapPointLink link = new MapPointLink(pointCurrent.Value, pointAboveCurrent.Value);
                        tempList.Add(link);

                        if (pointAboveRightOfCurrent != null)
                        {
                            MapPointLink linkRight = new MapPointLink(pointCurrent.Value, pointAboveRightOfCurrent.Value);
                            tempList.Add(linkRight);
                        }
                    }
                }
                else if (pointAboveRightOfCurrent != null)
                {
                    if (pointRightOfCurrent != null)
                    {
                        if (pointRightOfCurrent.Value.Position.x.RoughlyEquals(pointAboveRightOfCurrent.Value.Position.x, 2))
                        {
                            // in this case no point exists above the current point but does to the above right
                            //
                            // x o
                            //  /|
                            // c-o

                            MapPointLink linkRight = new MapPointLink(pointCurrent.Value, pointAboveRightOfCurrent.Value);
                            tempList.Add(linkRight);
                        }
                    }
                }
                else if (pointAboveLeftOfCurrent != null)
                {
                    if (pointLeftOfCurrent != null)
                    {
                        if (pointLeftOfCurrent.Value.Position.x.RoughlyEquals(pointAboveLeftOfCurrent.Value.Position.x, 2))
                        {
                            // Left leaning link: Case 2
                            // o x
                            // |\
                            // o-c 

                            MapPointLink linkLeft = new MapPointLink(pointCurrent.Value, pointAboveLeftOfCurrent.Value);
                            tempList.Add(linkLeft);
                        }
                    }
                }
            }
        }

        return tempList;
    }

    public struct LinkTri
    {
        public MapPointLink Link0 { get; private set; }
        public MapPointLink Link1 { get; private set; }

        public LinkTri (MapPointLink link0, MapPointLink link1)
        {
            Link0 = link0;
            Link1 = link1;
        }

        public MapPoint[] GetClockwiseMapPointTriangle()
        {
            MapPoint[] points = Get3DistinctMapPoints();

            // centre of the triangle
            Vector2 center = (points[0].Position + points[1].Position + points[2].Position) / 3;
            // get angle about the center for each angle
            Dictionary<MapPoint, float> angleAboutCentreDict = new Dictionary<MapPoint, float>();
            for (int i = 0; i < 3; i++)
            {
                Vector2 vec = points[i].Position - center;
                float angle = Mathf.Atan2(vec.y, vec.x);
                angleAboutCentreDict.Add(points[i], angle);
            }

            // return the points in order of clockwise about the center
            return angleAboutCentreDict.OrderByDescending(p => p.Value).Select(p => p.Key).ToArray();
        }

        MapPoint[] Get3DistinctMapPoints()
        {
            MapPoint[] allPoints =
            {
                Link0.Point0,
                Link0.Point1,
                Link1.Point0,
                Link1.Point1
            };

            // remove one of the equal points
            MapPoint[] points = allPoints.Distinct().ToArray();

            if (points.Length != 3)
            {
                Logger.WriteError("More than 3 vertices!");
                throw new System.Exception("More than 3 vertices!");
            }

            return points;
        }
    }

    public struct MapPointLink
    {
        public MapPoint Point0 { get; private set; }
        public MapPoint Point1 { get; private set; }

        public Vector2 Centroid { get { return (Point0.Position + Point1.Position) / 2; } }

        public MapPointLink(MapPoint point0, MapPoint point1)
        {
            Point0 = point0;
            Point1 = point1;
        }

        public bool SharesPointWith(MapPointLink otherLink)
        {
            return Point0.Position == otherLink.Point0.Position 
                || Point1.Position == otherLink.Point1.Position
                || Point0.Position == otherLink.Point1.Position
                || Point1.Position == otherLink.Point0.Position;
        }

        public override string ToString()
        {
            return Centroid.ToString();
        }
    }
}