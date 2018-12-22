using UnityEngine;
using CustomExtensions;

public class GhostHotspot : GhostRegion {

    public static event RegionEvent OnHotspotUpdated;
    
    void OnEnable()
    {
        if (OnHotspotUpdated != null)
            OnHotspotUpdated();
    }

    void OnValidate()
    {
        if (OnHotspotUpdated != null)
            OnHotspotUpdated();
    }

    void OnDisable()
    {
        if (OnHotspotUpdated != null)
            OnHotspotUpdated();
    }

    void OnDestroy()
    {
        if (OnHotspotUpdated != null)
            OnHotspotUpdated();
    }
}