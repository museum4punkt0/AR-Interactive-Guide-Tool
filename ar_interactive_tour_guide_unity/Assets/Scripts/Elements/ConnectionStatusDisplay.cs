using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable] public class BoolEvent : UnityEvent<bool> { }
[Serializable] public class IntEvent : UnityEvent<int> { }

public class ConnectionStatusDisplay : MonoBehaviour
{
    [SerializeField] private GameObject showWhenServerActive;
    [SerializeField] private GameObject showWhenServerInactive;

    [SerializeField] private GameObject showWhenClientConnected;
    [SerializeField] private GameObject showWhenClientDisconnected;

    [SerializeField] private string connectionCountPrefix;
    [SerializeField] private string connectionCountSuffix;

    [SerializeField] private Text connectionCountText;

    private void Start()
    {
        DoUpdate();
    }

    void Update()
    {
        DoUpdate();
    }

    void DoUpdate()
    {
        if (showWhenServerActive != null)
            showWhenServerActive.SetActive(IsServerActive());

        if (showWhenServerInactive != null)
            showWhenServerInactive.SetActive(!IsServerActive());

        if (showWhenClientConnected != null)
            showWhenClientConnected.SetActive(IsClientConnected());

        if (showWhenClientDisconnected != null)
            showWhenClientDisconnected.SetActive(!IsClientConnected());

        if (connectionCountText != null)
            connectionCountText.text = connectionCountPrefix + ServerConnectionCount() + connectionCountSuffix;
    }

    public bool IsServerActive()
    {
        return NetworkServer.active;
    }

    public int ServerConnectionCount()
    {
        
        if (NetworkServer.localConnection != null)
            return NetworkServer.connections.Count - 1; // not counting connection to local client
        else return NetworkServer.connections.Count;
    }

    public bool IsClientConnected()
    {
        return NetworkClient.isConnected;
    }
}
