using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using deVoid.Utils;

[RequireComponent(typeof(SelectableSystem))]
[ExecuteAlways]
public class SelectableSubSystem : MonoBehaviour
{
    MySelectable selectable;
    SelectableSystem system;

    public bool showOnSelect = false;
    public bool freeExploration = true;

    private bool selected { get { return selectable.selectableState == MySelectable.SelectableState.Selected; } }
    private bool subSystemsShown = false;

    private void Awake()
    {
        selectable = GetComponent<MySelectable>();
        if(selectable == null)
            selectable = GetComponentInParent<MySelectable>();

        system = GetComponent<SelectableSystem>();

        selectable.onSelect += OnSelectableStateChange;
        selectable.onNormal += OnSelectableStateChange;
        selectable.onInactive += OnSelectableStateChange;
        selectable.onHide += OnSelectableStateChange;

        system.initialState = MySelectable.SelectableState.Hidden;

        Signals.Get<ShowSelectableSubsystemsSignal>().AddListener(OnShowSignal);
    }

    private void OnShowSignal(bool show)
    {
        subSystemsShown = show;
        DoSetVisibility();
    }

    private void OnSelectableStateChange(MySelectable.SelectableState oldState)
    {
        Signals.Get<ShowSelectableSubsystemsSignal>().Dispatch(showOnSelect);
        DoSetVisibility();
    }

    private void DoSetVisibility()
    {
        if (selected && subSystemsShown)
            system.ResetAll();
        else
            system.HideAll();
    }

    private void OnDestroy()
    {
        selectable.onSelect -= OnSelectableStateChange;
        selectable.onNormal -= OnSelectableStateChange;
        selectable.onInactive -= OnSelectableStateChange;

        Signals.Get<ShowSelectableSubsystemsSignal>().RemoveListener(OnShowSignal);
    }
}

[Serializable]
public class ShowSelectableSubsystemsSignal : ASignal<bool> { }