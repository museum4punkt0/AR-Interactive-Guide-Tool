using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Linq;

// This class is used to serve as a reference the ARContentScreen can find
// there must be only one in the scene and it must always be active

// its direct children should be nothing except containers for the rest of the content
// so all the AR stuff can be toggled by SetContentActive
// usually just one child that contains all the rest

public class ARRoot : MonoBehaviour
{
    ARScene[] arScenes;
    ARSession arSession;

    private void Awake()
    {
        arScenes = GetComponentsInChildren<ARScene>(true);
        arSession = GetComponentInChildren<ARSession>(true);

        foreach (var scene in arScenes)
        {
            scene.gameObject.SetActive(true);
            scene.gameObject.SetActive(false);
        }
    }

    public void ShowContent(ARFile file)
    {
        arSession.enabled = true;

        foreach(var scene in arScenes)
        {
            scene.Reset();

            if (scene.uniqueName == file.uniqueName)
                scene.gameObject.SetActive(true);
            else
                scene.gameObject.SetActive(false);
        }
    }

    public void Reset()
    {
        foreach (var scene in arScenes)
            scene.Reset();
    }

    public void Deactivate()
    {
        foreach (var scene in arScenes)
            scene.gameObject.SetActive(false);

        arSession.enabled = false;
    }
}