using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using deVoid.Utils;
using System;

public class SetLanguageSignal : ASignal<Language> { }

public class ShowContentSignal : ASignal<MyFile> { }

public class ScrubVideoSignal : ASignal<double> { }

public class PlayVideoSignal : ASignal { }

public class PauseVideoSignal : ASignal { }

public class StartARSignal : ASignal {  }

public class StopARSignal : ASignal { }

public class EndTourSignal : ASignal { }

public class GuideSync : NetworkBehaviour
{

    // TODO low prio
    // instead of centrally handling language changes here
    // make all LocalisedTexts NetworkBehaviours

    // this is REALLY hacky.
    // I'm just doing this because the hook does not get called before the network is connected
    // TODO make the whole structure better, ie don't use a central GuideSync object for most tasks
    // instead use decentralised NetworkBehaviours as it's intended!

    public void SetActiveLanguage(Language language)
    {
        if (!isServer)
            OnLanguageChange(activeLanguage, language);

        activeLanguage = language;
    }

    [SyncVar(hook = nameof(OnLanguageChange))]
    public Language activeLanguage;

    protected void OnLanguageChange(Language oldLanguage, Language newLanguage) {
        LocalisedText.activeLanguage = newLanguage; // possibly find better pattern for this! note this is only needed for the case that there is no LocalisedText instance active.
        Signals.Get<SetLanguageSignal>().Dispatch((Language) newLanguage);
    }

    [SyncVar(hook = nameof(OnContentChange))]
    string activeContentNodeName;

    protected void OnContentChange(string oldNodeName, string newNodeName)
    {   
            // Hook might be called on initialization
            if (string.IsNullOrEmpty(newNodeName))
                return;

            var newFile = App.activeTour?
                            .root?
                            .GetContentItems(true)
                            .Find(f => f.nodeName == newNodeName);

        if (newFile != null)
            try
            {
                // should be in try/ catch since this hook is called OnDeserialize
                // by Mirror, and it messes with mirror if this throws an exception
                Signals.Get<ShowContentSignal>().Dispatch(newFile);
            }
            catch (Exception e)
            {
                Debug.Log("There was an error when showing content. Error: " + e);
            }
        else
            Debug.Log("The content requested to be shown by Guide was not found! NodeName: " + newNodeName);
    }

    ///////////////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        Signals.Get<FreeExplorationSignal>().AddListener(OnFreeExploration);
    }

    void OnFreeExploration(bool free)
    {
        if (isServer)
            RpcOnFreeExploration(free);
    }

    [ClientRpc]
    public void RpcOnFreeExploration(bool free)
    {
        if (!isServer)
        {
            GameObject guidedExpoInfo = null;
            guidedExpoInfo  = GameObject.FindWithTag("GEI");
            if(guidedExpoInfo != null)
                guidedExpoInfo.GetComponent<GuidedExplorationInfo>().info.gameObject.SetActive(!free);
        }
    }


    private void OnDestroy()
    {
        Signals.Get<FreeExplorationSignal>().RemoveListener(OnFreeExploration);
    }
    ///////////////////////////////////////////////////////////////////////////////////


    public void EndTour()
    {
        if(isServer)
            RpcEndTour();
    }

    [ClientRpc]
    void RpcEndTour()
    {
        Signals.Get<EndTourSignal>().Dispatch();
    }


    // called by GuideVisitorSync (monobehaviour)
    public void ShowContent(MyFile content)
    {
        activeContentNodeName = content.nodeName;
    }

    public void SetVideoTime(double time) {
        if (isServer)
            RpcSetVideoTime(time);
    }

    [ClientRpc]
    void RpcSetVideoTime(double time)
    {
        Signals.Get<ScrubVideoSignal>().Dispatch(time);
    }

    public void PlayVideo() {
        if (isServer)
            RpcPlayVideo();
    }

    [ClientRpc]
    void RpcPlayVideo() { 
        Signals.Get<PlayVideoSignal>().Dispatch();
    }

    public void PauseVideo()
    {
        if (isServer)
            RpcPauseVideo();
    }

    [ClientRpc]
    void RpcPauseVideo()
    {
        Signals.Get<PauseVideoSignal>().Dispatch();
    }

    public void StartAR()
    {
        if (isServer)
            RpcStartAR();
        else Signals.Get<StartARSignal>().Dispatch();
    }

    [ClientRpc]
    void RpcStartAR()
    {
        Signals.Get<StartARSignal>().Dispatch();
    }

    public void StopAR()
    {
        if (isServer)
            RpcStopAR();
        else Signals.Get<StopARSignal>().Dispatch();
    }

    [ClientRpc]
    void RpcStopAR()
    {
        Signals.Get<StopARSignal>().Dispatch();
    }
}