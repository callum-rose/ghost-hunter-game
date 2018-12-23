using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CustomExtensions;

public class HotspotManager : Singleton<HotspotManager> {

    GhostHotspot[] hotspotArr;

    void Awake()
    {
        Init();
    }

    public void Init()
    {
        UpdateHotspots();
    }

    void OnEnable()
    {
        GhostHotspot.OnHotspotUpdated += UpdateHotspots;
    }

    void OnDisable()
    {
        GhostHotspot.OnHotspotUpdated -= UpdateHotspots;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        GhostHotspot.OnHotspotUpdated -= UpdateHotspots;
    }

    public float NormalisedDistanceFromHotspot(Vector2 point)
    {
        if (hotspotArr == null || hotspotArr.Length == 0)
            return 1f;

        GhostHotspot[] hotspotsWithin = GetHotspotsWithin(point);

        if (hotspotsWithin == null || hotspotsWithin.Length == 0)
            return 1f;

        float closestNormDistance = float.MaxValue;
        foreach (var hotspot in hotspotsWithin)
        {
            float distance = Vector2.Distance(hotspot.transform.position.XZ2XY(), point);
            float normDistance = distance / hotspot.radius;
            if (normDistance < closestNormDistance)
                closestNormDistance = normDistance;
        }

        return closestNormDistance;
    }

    GhostHotspot[] GetHotspotsWithin(Vector2 point)
    {
        var hotspotsWithin = hotspotArr.Where(h => Vector2.Distance(h.transform.position.XZ2XY(), point) < h.radius);
        return hotspotsWithin.ToArray();
    }

    void UpdateHotspots()
    {
        // get all hotspots
        hotspotArr = FindObjectsOfType<GhostHotspot>();
    }
}