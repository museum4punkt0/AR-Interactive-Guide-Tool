using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An entry and exitpoint to starting the AR Exploration
// The actual AR scene is in the hierarchy from the beginning
// since it should be synced by NetworkBehaviours and I do not want
// to spawn stuff

public class ARContentContainer : MonoBehaviour
{
    private ARRoot arRoot;

    private void Awake()
    {
        arRoot = FindObjectOfType<ARRoot>();
    }

    public void ShowContent(ARFile file)
    {
        // arRoot can, upon finding a marker, open its own window
        // in order to close all unnecessary things that should not
        // be on the screen during AR exploration.

        // it will close itself. It will also be closed when any other content is shown.
        arRoot.ShowContent(file);
    }
}