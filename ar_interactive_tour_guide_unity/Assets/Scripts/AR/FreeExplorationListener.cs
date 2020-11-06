using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using deVoid.Utils;

[Serializable]
public class FreeExplorationEvent : UnityEvent<bool> { }

public class FreeExplorationListener : MonoBehaviour
{
    public FreeExplorationEvent onFreeExploration = new FreeExplorationEvent();
    // same as above with negated parameter for unity editor
    public FreeExplorationEvent onGuidedExploration = new FreeExplorationEvent();

    private void Awake()
    {
        Signals.Get<FreeExplorationSignal>().AddListener(OnFreeExploration);
    }

    private void OnDestroy()
    {
        Signals.Get<FreeExplorationSignal>().RemoveListener(OnFreeExploration);
    }

    private void OnFreeExploration(bool free)
    {
        onFreeExploration.Invoke(free);
        onGuidedExploration.Invoke(!free);
    }
}
