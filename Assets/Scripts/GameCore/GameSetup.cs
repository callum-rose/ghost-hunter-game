using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameSetup : MonoBehaviour {

    void Start()
    {
        var inits = FindObjectsOfType<MonoBehaviour>().Select(m => new { g = m, i = m.GetComponent<IInitialisable>() }).Where(o => o.i != null);
        foreach (var a in inits)
        {
            print(a.g.GetType().Name + " ");
        }

        Map.Instance.Init();
    }
}