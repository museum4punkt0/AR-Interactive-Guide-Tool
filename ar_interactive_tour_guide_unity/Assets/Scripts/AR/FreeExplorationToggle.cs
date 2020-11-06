using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using deVoid.Utils;

[Serializable]
public class FreeExplorationSignal : ASignal<bool> { }

public class FreeExplorationToggle : MonoBehaviour
{
    public void Toggle(bool free)
    {
        Signals.Get<FreeExplorationSignal>().Dispatch(free);
    }

    public void ToggleNot(bool notFree)
    {
        Toggle(!notFree);
    }
}
