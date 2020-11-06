using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Mirror;
using deVoid.Utils;

// TODO very minor: store global freeExploration bool
// in the case a POI is not activated before FreeExplorationSignal is sent
// until now that was never a problem as all POIs call Awake when the app starts

[ExecuteAlways]
[RequireComponent(typeof(MySelectable))]
[RequireComponent(typeof(NetworkIdentity))]
public class POI : NetworkBehaviour
{
    public ScreenspacePOI screenspacePOIFreePrefab;
    public ScreenspacePOI screenspacePOIGuidedPrefab;

    ScreenspacePOI screenspacePOIPrefab;

    [Header("Text")]
    [SerializeField] protected string titleEN;
    [SerializeField] protected string titleDE;
    [SerializeField] protected string subtitleEN;
    [SerializeField] protected string subtitleDE;
    [SerializeField] [TextArea] protected string infoTextEN;
    [SerializeField] [TextArea] protected string infoTextDE;

    private MySelectable selectable;
    private ScreenspacePOIContainer screenspacePOIContainer;

    // SerializeField only as to see the connection in inspector
    [SerializeField] private ScreenspacePOI screenspacePOI;

    [SyncVar]
    bool freeExploration;

    void Awake()
    {
        Signals.Get<FreeExplorationSignal>().AddListener(SetFreeExploration);
        selectable = GetComponent<MySelectable>();
        screenspacePOIContainer = FindObjectOfType<ScreenspacePOIContainer>();

        Assert.IsNotNull(screenspacePOIContainer);
    }

    private void Start()
    {
        SelectScreenspacePOI();
        FixScreenspacePOI();

        // do not do this before InitializeScreenspacePOI as OnNormal, On.. should have screenspacePOI != null
        if (selectable != null)
        {
            selectable.onNormal += OnNormal;
            selectable.onSelect += OnSelect;
            selectable.onInactive += OnInactive;
            selectable.onHide += OnHide;
        }
    }

    private void InitializeScreenspacePOI()
    {
        if(screenspacePOI.screenspaceLabel != null)
        {
            screenspacePOI.screenspaceLabel.SetText(Language.EN, titleEN);
            screenspacePOI.screenspaceLabel.SetText(Language.DE, titleDE);
        }

        if (screenspacePOI.infoTitle != null)
        {
            screenspacePOI.infoTitle.SetText(Language.EN, titleEN);
            screenspacePOI.infoTitle.SetText(Language.DE, titleDE);
        }

        if (screenspacePOI.infoSubtitle != null)
        {
            screenspacePOI.infoSubtitle.SetText(Language.EN, subtitleEN);
            screenspacePOI.infoSubtitle.SetText(Language.DE, subtitleDE);
        }

        if (screenspacePOI.infoContent != null)
        {
            screenspacePOI.infoContent.SetText(Language.EN, infoTextEN);
            screenspacePOI.infoContent.SetText(Language.DE, infoTextDE);
        }

        if (selectable != null)
        {
            screenspacePOI.requestClose += selectable.Deselect;
            screenspacePOI.requestSelect += selectable.Select;

            switch (selectable.selectableState)
            {
                case MySelectable.SelectableState.Normal:
                    OnNormal(MySelectable.SelectableState.Normal); break;
                case MySelectable.SelectableState.Selected:
                    OnSelect(MySelectable.SelectableState.Selected); break;
                case MySelectable.SelectableState.Hidden:
                    OnHide(MySelectable.SelectableState.Hidden); break;
                case MySelectable.SelectableState.Inactive:
                    OnInactive(MySelectable.SelectableState.Inactive); break;
            }
        }

        screenspacePOI.setContentActive = gameObject.activeInHierarchy;
		screenspacePOI.follow = transform;
    }

    public void BuildScreenspacePOI()
    {
        if (Application.isPlaying)
            screenspacePOI = Instantiate(screenspacePOIPrefab, screenspacePOIContainer.transform);
        else
        {
            #if UNITY_EDITOR
            var go = UnityEditor.PrefabUtility.InstantiatePrefab(screenspacePOIPrefab.gameObject, screenspacePOIContainer.transform) as GameObject;
            screenspacePOI = go.GetComponent<ScreenspacePOI>();
            #endif
        }

        screenspacePOIContainer.Register(screenspacePOI);
    }

    public void DestroyScreenspacePOI()
    {
        // always keep this check as POIEditor calls this function
        if (screenspacePOI == null)
            return;

        if(screenspacePOIContainer != null)
            screenspacePOIContainer.Unregister(screenspacePOI);

		if (Application.isPlaying)
        {
            Destroy(screenspacePOI.gameObject);
        }
        else
        {
            DestroyImmediate(screenspacePOI.gameObject);
        }
    }

    void Update()
    {
        SelectScreenspacePOI();
        FixScreenspacePOI();
    }

    void SelectScreenspacePOI()
    {
        // Check we have the right ScreenspacePOIPrefab
        // do NOT do this is a hook (eg of freeExploration), as the hook gets called before awake on (some?) clients
        ScreenspacePOI newPrefab;
        if (!isServer && !freeExploration && Application.isPlaying)
            newPrefab = screenspacePOIGuidedPrefab;
        else newPrefab = screenspacePOIFreePrefab;

        if (newPrefab != screenspacePOIPrefab)
        {
            screenspacePOIPrefab = newPrefab;
            DestroyScreenspacePOI();
            BuildScreenspacePOI();
            InitializeScreenspacePOI();
        }
    }

    void FixScreenspacePOI()
    {
        // Repair ScreenspacePOI in any case
        if (screenspacePOI == null && screenspacePOIPrefab != null)
        {
            BuildScreenspacePOI();
            InitializeScreenspacePOI();
        }
    }

    public void SetFreeExploration(bool free)
    {
        freeExploration = free;
    }

    // outside the editor ondisable and onenable are not currently in use
    // they do not syncronise with visitors over the network (the selectable methods do)
    private void OnDisable()
    {
        // do NOT do the following, as it causes a nasty OutOfBounds in UI.Selectable.OnDisable:
        // if (screenspacePOI != null)
        //    screenspacePOI.gameObject.SetActive(false);      // disable companion UI

        

        if (screenspacePOI != null)
            screenspacePOI.setContentActive = false;
    }

    private void OnEnable()
    {
        if (screenspacePOI != null)
            screenspacePOI.setContentActive = true;
    }

    private void OnDestroy()
    {
        Signals.Get<FreeExplorationSignal>().RemoveListener(SetFreeExploration);

        if (selectable != null)
        {
            selectable.onNormal -= OnNormal;
            selectable.onSelect -= OnSelect;
            selectable.onInactive -= OnInactive;
        }

        if (screenspacePOI != null)
        {
            if(selectable != null)
            {
                screenspacePOI.requestClose -= selectable.Deselect;
                screenspacePOI.requestSelect -= selectable.Select;
            }

            DestroyScreenspacePOI();
        }
    }

    private void OnSelect(MySelectable.SelectableState oldState)
    {
        if (screenspacePOI == null)
            return;

        foreach (var go in screenspacePOI.showOnInactive)
            go.SetActive(false);

        foreach (var go in screenspacePOI.showOnNormal)
            go.SetActive(false);

        foreach (var go in screenspacePOI.showOnSelect)
            go.SetActive(true);

        screenspacePOI.transform.SetAsLastSibling();
    }

    private void OnNormal(MySelectable.SelectableState oldState)
    {
        if (screenspacePOI == null)
            return;

        foreach (var go in screenspacePOI.showOnInactive)
            go.SetActive(false);

        foreach (var go in screenspacePOI.showOnSelect)
            go.SetActive(false);

        foreach (var go in screenspacePOI.showOnNormal)
            go.SetActive(true);
    }

    private void OnInactive(MySelectable.SelectableState oldState)
    {
        if (screenspacePOI == null)
            return;

        foreach (var go in screenspacePOI.showOnSelect)
            go.SetActive(false);

        foreach (var go in screenspacePOI.showOnNormal)
            go.SetActive(false);

        foreach (var go in screenspacePOI.showOnInactive)
            go.SetActive(true);
    }

    private void OnHide(MySelectable.SelectableState oldState)
    {
        if (screenspacePOI == null)
            return;

        foreach (var go in screenspacePOI.showOnSelect)
            go.SetActive(false);

        foreach (var go in screenspacePOI.showOnNormal)
            go.SetActive(false);

        foreach (var go in screenspacePOI.showOnInactive)
            go.SetActive(false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.05f);
    }
}