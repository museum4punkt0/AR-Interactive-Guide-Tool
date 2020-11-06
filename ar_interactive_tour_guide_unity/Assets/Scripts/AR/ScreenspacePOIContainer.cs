using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteAlways]
public class ScreenspacePOIContainer : MonoBehaviour
{
    private List<ScreenspacePOI> registeredPOIs = new List<ScreenspacePOI>();

    void Awake()
    {
        // sometimes ScreenspacePOIs get left over
        // when a POI is inactive and has not been active since last play
        // its OnDestroy won't be called
        ClearUnregisteredScreenspacePOIs();
    }

    public void DestroyScreenspacePOIs()
    {
        foreach (var child in GetComponentsInChildren<ScreenspacePOI>().ToList())
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
    }

    public void Register(ScreenspacePOI screenspacePOI)
    {
        registeredPOIs.Add(screenspacePOI);
    }

    public void Unregister(ScreenspacePOI screenspacePOI)
    {
        registeredPOIs.Remove(screenspacePOI);
    }

    void ClearUnregisteredScreenspacePOIs()
    {
        foreach (var child in GetComponentsInChildren<ScreenspacePOI>().ToList())
        {
            if (registeredPOIs.Contains(child))
                continue;

            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
    }
}