using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using deVoid.UIFramework;

public class ModeSelectionWindowController : AWindowController
{
    public void StartAsGuide()
    {

        App.uiFrame.OpenWindow(ScreenList.guideSetupWindow);
    }

    public void StartAsVisitor()
    {
        App.uiFrame.OpenWindow(ScreenList.visitorSetupWindow);
    }

    public void OpenNetworkSettings()
    {
        App.uiFrame.OpenWindow(ScreenList.networkSettingsWindow);
    }

    protected override void OnPropertiesSet() {
        Mirror.NetworkManager.singleton.StopHost();
    }
}