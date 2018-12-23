using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostDeadzone : GhostRegion {

    public static event RegionEvent OnDeadzoneUpdated;

    void OnEnable()
    {
        if (OnDeadzoneUpdated != null)
            OnDeadzoneUpdated();
    }

    void OnValidate()
    {
        if (OnDeadzoneUpdated != null)
            OnDeadzoneUpdated();
    }

    void OnDisable()
    {
        if (OnDeadzoneUpdated != null)
            OnDeadzoneUpdated();
    }

    void OnDestroy()
    {
        if (OnDeadzoneUpdated != null)
            OnDeadzoneUpdated();
    }
}
