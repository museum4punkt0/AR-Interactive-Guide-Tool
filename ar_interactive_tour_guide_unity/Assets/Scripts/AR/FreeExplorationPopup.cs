using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using deVoid.Utils;

public class FreeExplorationPopup : NetworkBehaviour
{
    [SerializeField] GameObject popup;

    private void Awake()
    {
        Signals.Get<FreeExplorationSignal>().AddListener(OnFreeExploration);
    }

    private void OnDestroy()
    {
        Signals.Get<FreeExplorationSignal>().RemoveListener(OnFreeExploration);
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
            popup.SetActive(free);
    }
}