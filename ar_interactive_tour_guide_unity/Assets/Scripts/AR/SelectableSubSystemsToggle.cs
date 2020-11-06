using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using deVoid.Utils;
using UnityEngine.Events;
using System;

[Serializable]
public class UpdateToggleEvent : UnityEvent<bool> { }

public class SelectableSubSystemsToggle : MonoBehaviour
{
    public UpdateToggleEvent updateToggle = new UpdateToggleEvent();

    private void Awake()
    {
        Signals.Get<ShowSelectableSubsystemsSignal>().AddListener(updateToggle.Invoke);
    }

    public void Toggle(bool show)
    {
        Signals.Get<ShowSelectableSubsystemsSignal>().Dispatch(show);
    }

    private void OnDestroy()
    {
        Signals.Get<ShowSelectableSubsystemsSignal>().RemoveListener(updateToggle.Invoke);
    }
}