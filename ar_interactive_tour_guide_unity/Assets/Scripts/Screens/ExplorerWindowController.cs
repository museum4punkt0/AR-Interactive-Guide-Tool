using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using deVoid.UIFramework;

// This is not currently in use!

[Serializable]
public class ExplorerWindowProperties : WindowProperties
{
    public MyFile activeFile;

    public ExplorerWindowProperties(MyFile activeFile)
    {
        this.activeFile = activeFile;
    }
}

public class ExplorerWindowController : AWindowController<ExplorerWindowProperties>
{
    [SerializeField] private TourExplorer tourExplorer;
    [SerializeField] private bool closeOnFileClicked;

    

    protected override void OnPropertiesSet()
    {
        tourExplorer.Initialize(App.activeTour, false);
        tourExplorer.SelectFile(Properties.activeFile);
    }

    void OnFileClicked(MyFile file)
    {
        App.guideSync.ShowContent(file);

        if (closeOnFileClicked)
            CloseRequest(this);
    }
}
