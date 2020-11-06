using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using deVoid.Utils;

public class BackButton : MonoBehaviour
{
    public void CloseCurrentWindow()
    {
        App.uiFrame.CloseCurrentWindow();
    }

    public void EndTour()
    {
        App.guideSync.EndTour();
    }
}
