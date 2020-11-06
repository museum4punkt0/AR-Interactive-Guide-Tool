using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteAlways]
public class SelectableSystem : MonoBehaviour
{
    public MySelectable.SelectableState initialState = MySelectable.SelectableState.Normal;
    readonly HashSet<MySelectable> registeredSelectables = new HashSet<MySelectable>();

    public bool Register(MySelectable selectable)
    {
        var reg = registeredSelectables.Add(selectable);

        selectable.DoSetState(initialState);
        return reg;
    }

    public bool Unregister(MySelectable selectable)
    {
        return registeredSelectables.Remove(selectable);
    }

    public void Select(MySelectable selectable)
    {
        foreach(var s in registeredSelectables)
        {
            if (s != selectable)
                s.DoSetState(MySelectable.SelectableState.Inactive);
        }

        if(selectable != null)
            selectable.DoSetState(MySelectable.SelectableState.Selected);

        initialState = MySelectable.SelectableState.Inactive;
    }

    public void ResetAll()
    {
        foreach (var s in registeredSelectables)
        {
            s.DoSetState(MySelectable.SelectableState.Normal);
        }

        initialState = MySelectable.SelectableState.Normal;
    }

    public void HideAll()
    {
        foreach (var s in registeredSelectables)
        {
            s.DoSetState(MySelectable.SelectableState.Hidden);
        }

        initialState = MySelectable.SelectableState.Hidden;
    }
}