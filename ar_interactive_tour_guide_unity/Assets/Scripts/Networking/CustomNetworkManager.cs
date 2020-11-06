using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

/// <summary>
/// A Network Manager that will try to reconnect to the same host (by uri) once disconnected. It will stop the client and restart.
/// 
/// This is sort of hacky, in two ways.
/// Firstly, the NetworkManager does not store the Uri passed in StartClient(Uri uri), hence here we require that SetUri is called before StartClient - with the same Uri!!
/// 
/// Secondly, it is not completely thought through who we reconnect to: right now we simply reconnect to the same uri.
/// This may 1) change if the guide wifi is lost and reconnected, and 2) not be a great idea if the guide device fails for whatever reason (ie not possible to take over tour with other guide device)
/// The better way would be not to reconnect via Uri but via an identifier of a guide device or profile, then start Network Discovery again and search for the appropriate host!
/// Best way: require that Profile Names be unique. Then, do all of this via profile names.
/// </summary>
public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private float tryReconnectPeriod = 1;

    Uri uri;
    bool hasCalledSetUri = false; // hack to remind user to set Uri before starting client. Need to send pullrequest to Mirror so they dont save just uri.Host but the whole uri in NetworkManager.

    public void SetUri(Uri uri)
    {
        this.uri = uri;
        hasCalledSetUri = true;
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        if (!hasCalledSetUri && mode == NetworkManagerMode.ClientOnly)
            Debug.LogWarning("You need to call SetUri before every call to StartClient! Ignore this if you called StartClient() without Uri");

        base.OnClientConnect(conn);
        hasCalledSetUri = false;
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        StopClient();
        StartCoroutine(TryReconnect());
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("ON CLIENT ERROR. errorCode: " + errorCode);
        StopClient(); // TODO I assume this will call OnClientDisconnect but have not tested this
    }

    // It is very important that WaitForSeconds comes before StartClient.
    // StartClient will call OnClientDisconnect if it fails!
    // This is somewhat cleaner than reconnection solutions offered at 
    // https://forum.unity.com/threads/reconnect-client-to-server.436033/
    // since it lets Mirror handle the destroying of the client.
    private IEnumerator TryReconnect()
    {
        yield return new WaitForSeconds(tryReconnectPeriod);

        SetUri(uri);
        StartClient(uri);
    }

    NetworkManagerMode resumeAs;

    void OnApplicationPause(bool pause)
    {   
        if(pause)
        {
            resumeAs = mode;

            switch (mode)
            {
                case NetworkManagerMode.Host:
                    StopHost();
                    break;
                case NetworkManagerMode.ServerOnly:
                    StopServer();
                    break;
                case NetworkManagerMode.ClientOnly:
                    StopClient();
                    break;
            }
        }

        else
        {
            switch (resumeAs)
            {
                case NetworkManagerMode.Host:
                    StartHost();
                    App.discovery.AdvertiseServer();
                    break;
                case NetworkManagerMode.ServerOnly:
                    StartServer();
                    App.discovery.AdvertiseServer();
                    break;
                case NetworkManagerMode.ClientOnly:
                    SetUri(uri); // only to fulfill our contract
                    StartClient(uri);
                    break;
            }
        }
    }
}