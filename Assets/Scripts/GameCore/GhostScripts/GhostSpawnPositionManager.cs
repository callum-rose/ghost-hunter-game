using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GhostSpawnPositionManager : Singleton<GhostSpawnPositionManager> 
{
    [SerializeField] [Range(0, 1)] float baseLikelihood = 0.1f;

    MapPoint[] mapPoints;

    // TODO don't know if this is needed
    Dictionary<MapPoint, float> spawnLikelihoodAtMapPointDict;
    // cumulative probabiliy distribution of a position on the map being used
    Dictionary<MapPoint, float> CpdAtMapPoint;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        UpdateMapReference();
        UpdateMapLocationSpawnProbabilities();
    }

    public Vector2 GetPositionAtCumulativeProb(float prob)
    {
        MapPoint mp = CpdAtMapPoint.OrderBy(k => k.Value).First(k => k.Value >= prob && k.Value != 0f).Key;
        return mp.Position;
    }

    void UpdateMapReference()
    {
        mapPoints = Map.Instance.MapPointsArr;
    }

    void UpdateMapLocationSpawnProbabilities()
    {
        // reset or create value maps
        if (spawnLikelihoodAtMapPointDict == null)
            spawnLikelihoodAtMapPointDict = new Dictionary<MapPoint, float>();
        else
            spawnLikelihoodAtMapPointDict.Clear();

        if (CpdAtMapPoint == null)
            CpdAtMapPoint = new Dictionary<MapPoint, float>();
        else
            CpdAtMapPoint.Clear();

        foreach (MapPoint mapPoint in mapPoints)
        {
            spawnLikelihoodAtMapPointDict.Add(mapPoint, 0f);
            CpdAtMapPoint.Add(mapPoint, 0f);
        }

        for (int p = 0; p < mapPoints.Length; p++)
        {
            MapPoint mapPoint = mapPoints[p];

            if (!IsInDeadzone(mapPoint))
            {
                AddBaseLikelihoods(mapPoint);
                AddHotspotLikelihoods(mapPoint);
            }
        }

        FinaliseCPD();
    }

    void AddBaseLikelihoods(MapPoint mapPoint)
    {
        // add likelihood of a spawn at position due to base likelihood
        spawnLikelihoodAtMapPointDict[mapPoint] += baseLikelihood;
    }

    void AddHotspotLikelihoods(MapPoint mapPoint)
    {
        // add likelihood of a spawn at position due to hotspot
        float spawnProb = GhostProbabilty.Instance.SpawnLikelihoodDueToHotspot(mapPoint.Position);
        spawnLikelihoodAtMapPointDict[mapPoint] += spawnProb;
    }

    bool IsInDeadzone(MapPoint mapPoint)
    {
        return DeadzoneManager.Instance.IsWithinDeadZone(mapPoint.Position);
    }

    void FinaliseCPD()
    {
        float prevCpd = 0f;
        for (int p = 0; p < mapPoints.Length; p++)
        {
            MapPoint mapPoint = mapPoints[p];

            // set cumulative likelihoods
            CpdAtMapPoint[mapPoint] = prevCpd + spawnLikelihoodAtMapPointDict[mapPoint];
            prevCpd = CpdAtMapPoint[mapPoint];
        }

        // normalise all cumulative likelihoods to make a probability distribution
        MapPoint finalMapPoint = mapPoints[mapPoints.Length - 1];
        float cumulativeLikelihood = CpdAtMapPoint[finalMapPoint];
        foreach (var mapPoint in mapPoints)
            CpdAtMapPoint[mapPoint] /= cumulativeLikelihood;
    }
}
