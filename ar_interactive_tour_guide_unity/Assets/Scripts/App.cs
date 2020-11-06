using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.Events;
using System.IO;
using deVoid.UIFramework;
using deVoid.Utils;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ScreenList))]
public class App : MonoBehaviour
{
    // there are surely more elegant solutions to assign static fields in Unity
    [SerializeField] private UIFrame _uiFrame;
    [SerializeField] private CustomNetworkDiscovery _discovery;
    [SerializeField] private GuideSync _guideSync;

    [Header("HACK. Adjust so that POIs on iOS are aligned properly.")]
    [SerializeField] private float _POIpositionScalingFactor = 0.81f;

    public static UIFrame uiFrame;
    public static CustomNetworkDiscovery discovery;
    public static GuideSync guideSync;
    public static float POIpositionScalingFactor = 0.81f;

    public static Tour activeTour;
    public static MyFile activeFile;

    public static Cache cache;

    private void Awake() {
        // Assign Globals
        uiFrame = _uiFrame;
        discovery = _discovery;
        guideSync = _guideSync;
        POIpositionScalingFactor = _POIpositionScalingFactor;
        cache = new Cache();

        Signals.Get<ShowContentSignal>().AddListener(OnShowContent);
    }

    void Start() {
        var screenList = GetComponent<ScreenList>();
        screenList.InstantiateAndRegisterScreens(uiFrame); // do NOT do this in Awake (or disable Initialize On Awake in uiFrame and Initialize yourself)

        uiFrame.OpenWindow(ScreenList.loadingWindow);
    }

    void OnShowContent(MyFile file)
    {
        activeFile = file;
    }

}

public static class Paths
{
    static public string defaultNetworkSettingsPath { get
        {
            return Path.GetFullPath(Application.streamingAssetsPath + "/defaultNetworkSettings.xml");
        } }

    static public string networkSettingsSavePath { get
        {
            return Path.GetFullPath(Application.persistentDataPath + "/networkSettings.xml");
        } }
    static public string customToursPath { get {
            return Path.GetFullPath(Application.persistentDataPath + "/customTours/");
        } }

    static public string defaultTourPath { get {
            return Path.GetFullPath(Application.persistentDataPath + "/fullTour/fullTour.xml");
        } }

    static public string cachePath { get {
            return Path.GetFullPath(Application.persistentDataPath + "/cache/");
        } }

}

public static class ExtensionMethods {

    #region Coroutine Helpers
    public static Coroutine StartCoroutineSync(this MonoBehaviour monoBehaviour, IEnumerator func)
    {
        while(func != null && func.MoveNext())
              if(func.Current != null)
                    monoBehaviour.StartCoroutineSync ((IEnumerator)func.Current);
        return null;
    }

    public static IEnumerator Then (this IEnumerator coroutine, Action callback) {
        yield return coroutine;
        callback();
    }

    public static IEnumerator Then (this IEnumerator coroutine, IEnumerator next) {
        yield return coroutine;
        yield return next;
    }

    public static IEnumerator Do(this YieldInstruction yieldInstruction)
    {
        yield return yieldInstruction;
    }
    #endregion

    // copied from System.Linq source code:
    // https://github.com/microsoft/referencesource/blob/master/System.Core/System/Linq/Enumerable.cs
    // with added parametric default value
    public static TSource ElementAtOrDefault<TSource>(this IEnumerable<TSource> source, int index, TSource @default)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (index >= 0)
        {
            IList<TSource> list = source as IList<TSource>;
            if (list != null)
            {
                if (index < list.Count) return list[index];
            }
            else
            {
                using (IEnumerator<TSource> e = source.GetEnumerator())
                {
                    while (true)
                    {
                        if (!e.MoveNext()) break;
                        if (index == 0) return e.Current;
                        index--;
                    }
                }
            }
        }
        return @default;
    }

    public static IEnumerable<GameObject> Children(this Transform parent)
    {
        for (var i = 0; i < parent.childCount; i++)
            yield return parent.GetChild(i).gameObject;
    }
}

public static class Helpers
{
    static public string TimeSpanToString(TimeSpan span)
    {
        string format;

        if (span.TotalHours >= 1)
            format = @"h\:mm\:ss";
        else format = @"mm\:ss";

        return span.ToString(format);
    }
}