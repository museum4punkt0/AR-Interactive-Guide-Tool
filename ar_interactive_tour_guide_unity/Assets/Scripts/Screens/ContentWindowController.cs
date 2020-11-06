using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using deVoid.UIFramework;
using deVoid.Utils;
using Mirror;

// This window is a little overloaded as it contains the file explorer AND the content
// this is because we had problems using an extra popup window controlling the file explorer
// as closing the popup would call all OnPropertiesSet and fire OnTransistionInFinished
// TODO change this by adding a callback to Window Controller OnVisible that only fires when
// the screen is opened and was hidden before.

[Serializable]
public class ContentWindowControllerProperties : WindowProperties 
{
    public Tour tour;
    public MyFile startWithFile; // TODO minor should this be stored in the Tour?
    public bool startWithExplorer;
}

public class ContentWindowController : AWindowController<ContentWindowControllerProperties>
{
    // TODO possibly use uiframework for these containers as well (esp when later using animations?)
    [SerializeField] private ImageContentContainer imageContentContainer;
    [SerializeField] private VideoContentContainer videoContentContainer;
    [SerializeField] private GameObject videoControlOverlay;
    //[SerializeField] private ARContentContainer arContentContainer;
    [SerializeField] private TextContentContainer textContentContainer;
    [SerializeField] private GameContentContainer gameContentContainer;

    [SerializeField] private GameObject tourExplorerWindow;
    [SerializeField] private TourExplorer tourExplorer;
    [SerializeField] private bool closeExplorerOnImageFileClicked;
    [SerializeField] private bool closeExplorerOnVideoFileClicked;
    [SerializeField] private bool closeExplorerOnARFileClicked;
    [SerializeField] private bool closeExplorerOnTextFileClicked;
    [SerializeField] private bool closeExplorerOnGameFileClicked;



    private ARRoot arRoot; // must be in the scene since it's networked and we don't spawn

    public GameObject[] hideOnAR;
    public GameObject[] showOnAR;
    public GameObject[] hideOnARShowContent;
    [SerializeField] GuidedExplorationInfo popupWindow; 

    MyFile activeFile;

    protected override void Awake(){
        base.Awake();

        arRoot = FindObjectOfType<ARRoot>();
        DeactivateAll();
    }

    protected override void AddListeners() {
        Signals.Get<ShowContentSignal>().AddListener(ShowContent);
        Signals.Get<ShowExplorerWindowSignal>().AddListener(ShowExplorerWindow);
        Signals.Get<ARMarkerFoundSignal>().AddListener(OnStartAR);
        // TODO
        // on EndTour click, visitor goes back to VisitorSetUpWindow
        Signals.Get<EndTourSignal>().AddListener(OnEndTour);

        InTransitionFinished = c => OnInTransitionFinished();
        tourExplorer.onFileClicked += ExplorerOnFileClicked;
    }

    protected override void RemoveListeners() {
        Signals.Get<ShowContentSignal>().RemoveListener(ShowContent);
        Signals.Get<ShowExplorerWindowSignal>().RemoveListener(ShowExplorerWindow);
        Signals.Get<ARMarkerFoundSignal>().RemoveListener(OnStartAR);
        // TODO
        // on EndTour click, visitor goes back to VisitorSetUpWindow
        Signals.Get<EndTourSignal>().RemoveListener(OnEndTour);

        tourExplorer.onFileClicked -= ExplorerOnFileClicked;
    }

    protected override void OnPropertiesSet()
    {
        base.OnPropertiesSet();

        DeactivateAll();
        tourExplorer.Initialize(Properties.tour, false);

        if(Properties.startWithFile != null)
            App.guideSync.ShowContent(Properties.startWithFile);

        Signals.Get<ResetTimerSignal>().Dispatch();
    }

    // Guarantees that gameobject is active
    void OnInTransitionFinished() {
        ShowContent(activeFile);
    }

    void ShowContent(MyFile content)
    {
        activeFile = content;

        if (!gameObject.activeInHierarchy) 
            return;

        DeactivateAll();

        switch (content)
        {
            case ImageFile file:
                imageContentContainer.gameObject.SetActive(true);
                imageContentContainer.ShowContent(file);
                if(popupWindow != null)
                    popupWindow.gameObject.SetActive(true);
                break;
            case VideoFile file:
                videoContentContainer.gameObject.SetActive(true);
                videoControlOverlay.SetActive(true);
                videoContentContainer.showControls = App.guideSync.isServer;
                videoContentContainer.ShowContent(file);
                if(popupWindow != null)
                    popupWindow.gameObject.SetActive(true);
                break;
            case ARFile file:
                // the AR scene will trigger arContentContainer to show
                foreach (var o in hideOnARShowContent)
                    o.SetActive(false);

                arRoot.ShowContent(file);
                break;
            case TextFile file:
                textContentContainer.gameObject.SetActive(true);
                textContentContainer.ShowContent(file);
                if(popupWindow != null)
                    popupWindow.gameObject.SetActive(true);
                break;
            case GameFile file:
                gameContentContainer.gameObject.SetActive(true);
                gameContentContainer.ShowContent();
                if(popupWindow != null)
                    popupWindow.gameObject.SetActive(false);
                break;
        }
    }

    public void OnEndTour()
    {
        App.uiFrame.CloseCurrentWindow();
        //App.uiFrame.OpenWindow(ScreenList.visitorSetupWindow);
    }
    public void EndTour()
    {
        App.uiFrame.CloseCurrentWindow();
    }

    public void ShowExplorerWindow(bool show)
    {
        if(gameObject.activeInHierarchy)
            tourExplorerWindow.gameObject.SetActive(show);
    }

    void ExplorerOnFileClicked(MyFile file)
    {
        App.guideSync.ShowContent(file);

        switch (file)
        {
            case ImageFile f:
                if (closeExplorerOnImageFileClicked)
                    ShowExplorerWindow(false);
                break;
            case VideoFile f:
                if (closeExplorerOnVideoFileClicked)
                    ShowExplorerWindow(false);
                break;
            case ARFile f:
                if (closeExplorerOnARFileClicked)
                    ShowExplorerWindow(false);
                break;
            case TextFile f:
                if (closeExplorerOnTextFileClicked)
                    ShowExplorerWindow(false);
                break;
            case GameFile f:
                if (closeExplorerOnGameFileClicked)
                    ShowExplorerWindow(false);
                break;
        }
    }

    void DeactivateAll()
    {
        foreach (var o in showOnAR)
            o.SetActive(false);
        foreach (var o in hideOnAR)
            o.SetActive(true);
        foreach (var o in hideOnARShowContent)
            o.SetActive(true);
        
        imageContentContainer.gameObject.SetActive(false);
        videoContentContainer.gameObject.SetActive(false);
        videoControlOverlay.SetActive(false);
        textContentContainer.gameObject.SetActive(false);
        gameContentContainer.gameObject.SetActive(false);
        arRoot.Deactivate();
    }

    // TODO improve code design
    // currently complicated through how the AR scene is set up:
    // since it contains networkbehaviours, and we do not spawn because it would
    // complicate things with the uiFramework,
    // the AR scene is always in the Main Scene hierarchy
    // the call to OnMarkerFound is also coming from there
    void OnStartAR()
    {
        if (!gameObject.activeInHierarchy)
            return;

        foreach (var o in hideOnAR)
            o.SetActive(false);
        foreach (var o in showOnAR)
            o.SetActive(true);

        ShowExplorerWindow(false);
    }

    public void StopAR()
    {
        foreach (var o in showOnAR)
            o.SetActive(false);
        foreach (var o in hideOnAR)
            o.SetActive(true);

        if (arRoot != null)
            arRoot.Reset();
    }
}
