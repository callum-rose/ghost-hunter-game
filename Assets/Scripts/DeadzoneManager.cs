using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CustomExtensions;

public class DeadzoneManager : Singleton<DeadzoneManager> {

    GhostDeadzone[] deadzoneArr;

    void Awake()
    {
        Init();
    }

    public void Init()
    {
        UpdateDeadzones();
    }

    void OnEnable()
    {
        GhostDeadzone.OnDeadzoneUpdated += UpdateDeadzones;
    }

    void OnDisable()
    {
        GhostDeadzone.OnDeadzoneUpdated -= UpdateDeadzones;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        GhostDeadzone.OnDeadzoneUpdated -= UpdateDeadzones;
    }

    public bool IsWithinDeadZone(Vector2 point)
    {
        if (deadzoneArr == null || deadzoneArr.Length == 0)
            return false;

        // return true if point is within a deadzone
        return deadzoneArr.Any(d => Vector2.Distance(d.transform.position.XZ2XY(), point) <= d.radius);
    }

    void UpdateDeadzones()
    {
        // get all hotspots
        deadzoneArr = FindObjectsOfType<GhostDeadzone>();
    }
}
