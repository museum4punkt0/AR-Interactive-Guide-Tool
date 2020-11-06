using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using deVoid.Utils;

public class ShowExplorerWindowButton : MonoBehaviour
{
    public void ShowExplorer(bool show)
    {
        Signals.Get<ShowExplorerWindowSignal>().Dispatch(show);
    }
}