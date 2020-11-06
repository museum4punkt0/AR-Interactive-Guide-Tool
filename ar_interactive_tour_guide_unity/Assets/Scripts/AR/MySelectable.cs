using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System;
using Mirror;
using UnityEngine.UI;
using deVoid.Utils;

// TODO better implementation of components that listen to these Actions
// eg via an interface IMySelectable or the Unity messaging system
// could be based on
// https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/MessagingSystem.html


// TODO separate out NetworkBehaviour into separate NetworkedSelectable

[ExecuteAlways]
[RequireComponent(typeof(NetworkIdentity))]
public class MySelectable : NetworkBehaviour
{
    public enum SelectableState { Hidden, Inactive, Selected, Normal }

    public event Action<SelectableState> onHide;
    public event Action<SelectableState> onSelect;
    public event Action<SelectableState> onNormal;
    public event Action<SelectableState> onInactive;

    public SelectableState selectableState { get; protected set; } = SelectableState.Normal;

    private SelectableSystem selectableSystem;

    bool freeExploration;

    private void Awake()
    {
        // missleadingly, GetComponentInParent searches through all ancestors
        // also it FINDS COMPONENTS IN ITSELF goddamnit

        if(transform.parent != null)
            selectableSystem = transform.parent.GetComponentInParent<SelectableSystem>();

        Signals.Get<FreeExplorationSignal>().AddListener(OnFreeExploration);
    }

    private void Start()
    {
        if (selectableSystem != null)
            selectableSystem.Register(this);

        else Debug.Log("MySelectable cannot find its SelectableSystem!");
    }

    public void DoSetState(SelectableState newState)
    {
        if (isServer && !freeExploration)
            RpcDoSetState(newState);

		var oldState = selectableState;

        selectableState = newState;

        switch (newState)
        {
            case SelectableState.Hidden:
                onHide?.Invoke(oldState);
                break;
            case SelectableState.Selected:
                onSelect?.Invoke(oldState);
                break;
            case SelectableState.Inactive:
                onInactive?.Invoke(oldState);
                break;
            case SelectableState.Normal:
                onNormal?.Invoke(oldState);
                break;
        }
    }

    public void Deselect()
    {
        if(selectableSystem != null)
            selectableSystem.ResetAll();
    }

    public void Select()
    {
        if (selectableSystem != null)
            selectableSystem.Select(this);
    }

    [ClientRpc]
    public void RpcDoSetState(SelectableState newState)
    {
        if (!isServer)
            DoSetState(newState);
    }

    void OnFreeExploration(bool free)
    {
        freeExploration = free;
        if(!free)
            Deselect();
    }

    private void OnDestroy()
    {
        Signals.Get<FreeExplorationSignal>().RemoveListener(OnFreeExploration);
    }
}