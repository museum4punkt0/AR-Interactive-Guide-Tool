using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using deVoid.UIFramework;

[Serializable]
public class CSVURLVerifyEvent : UnityEvent<bool> { }

[Serializable] public class StringEvent : UnityEvent<string> { }

public class NetworkSettingsWindowController : AWindowController
{
    public StringEvent onSetCSVURL = new StringEvent();
    public StringEvent onSetGuideVisitorPort = new StringEvent();
    public StringEvent onSetGuideVisitorDiscoveryPort = new StringEvent();

    // TODO verify connection to url
    //public CSVURLVerifyEvent onCSVURLVerify = new CSVURLVerifyEvent();

    private NetworkSettings settings;

    protected override void OnPropertiesSet()
    {
        base.OnPropertiesSet();

        // just a hint: never use ?? or ?. operators with unity gameobjects
        // https://answers.unity.com/questions/1378330/-operator-not-working-as-expected.html
        settings = Settings.LoadNetworkSettings();

        Populate();
    }

    public void SetCSVURL(string url)
    {
        settings.url = url;
        //onCSVURLVerify.Invoke( );
    }

    public void SetGuideVisitorPort(string port)
    {
        int.TryParse(port, out settings.guideVisitorPort);
    }

    public void SetGuideVisitorDiscoveryPort(string port)
    {
        int.TryParse(port, out settings.guideVisitorDiscoveryPort);
    }

    public void ResetCachedData()
    {
        App.cache.DeleteCache();
    }

    public void ResetConfiguration()
    {
        settings = Settings.LoadDefaultNetworkSettings();

        Populate();
    }

    public void SaveAndReload()
    {
        // TODO low prio: give feedback if save fails
        Settings.SaveNetworkSettings(settings);
        Reload();
    }

    // It is important to reload on close
    // since we might have deleted the cache data!
    // and our app isn't smart enough to work without downloaded data (yet)
    public void Reload()
    {
        App.uiFrame.CloseAllWindows();
        App.uiFrame.OpenWindow(ScreenList.loadingWindow);
    }

    void Populate()
    {
        onSetCSVURL.Invoke(settings.url);
        onSetGuideVisitorPort.Invoke(settings.guideVisitorPort.ToString());
        onSetGuideVisitorDiscoveryPort.Invoke(settings.guideVisitorDiscoveryPort.ToString());
    }
}

[Serializable]
public class NetworkSettings
{
    public string url = Paths.defaultNetworkSettingsPath;
    public int guideVisitorPort;
    public int guideVisitorDiscoveryPort;
}

public static class Settings
{
    public static NetworkSettings LoadNetworkSettings()
    {
        return XmlOperation.TryDeserialize<NetworkSettings>(Paths.networkSettingsSavePath)
            ?? LoadDefaultNetworkSettings();
    }

    public static NetworkSettings LoadDefaultNetworkSettings()
    {
        return XmlOperation.TryDeserialize<NetworkSettings>(Paths.defaultNetworkSettingsPath)
            ?? new NetworkSettings();
    }

    public static bool SaveNetworkSettings(NetworkSettings settings)
    {
        return XmlOperation.TrySerialize(settings, Paths.networkSettingsSavePath);
    }
}