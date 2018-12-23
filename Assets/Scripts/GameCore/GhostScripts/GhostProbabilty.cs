using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostProbabilty : Singleton<GhostProbabilty> {

    [SerializeField] AnimationCurve probabilityByDistanceCurve;

    public float SpawnLikelihoodDueToHotspot(Vector2 point)
    {
        float distance = HotspotManager.Instance.NormalisedDistanceFromHotspot(point);
        return probabilityByDistanceCurve.Evaluate(distance);
    }
}