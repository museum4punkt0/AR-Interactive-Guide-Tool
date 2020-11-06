using System.Net;
using System;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using Mirror.Discovery;

/*
	Discovery Guide: https://mirror-networking.com/docs/Guides/NetworkDiscovery.html
    Documentation: https://mirror-networking.com/docs/Components/NetworkDiscovery.html
    API Reference: https://mirror-networking.com/docs/api/Mirror.Discovery.NetworkDiscovery.html
*/

[Serializable]
public class ServerFoundEvent : UnityEvent<DiscoveryResponse> { };

public class DiscoveryRequest : MessageBase
{
    // Add properties for whatever information you want sent by clients
    // in their broadcast messages that servers will consume.
}

public class DiscoveryResponse : MessageBase
{
    // The server that sent this
    // this is a property so that it is not serialized,  but the
    // client fills this up after we receive it
    public IPEndPoint EndPoint { get; set; }

    public Uri uri;

    public string serverName;

    // Prevent duplicate server appearance when a connection can be made via LAN on multiple NICs
    public long serverId;
}

public class CustomNetworkDiscovery : NetworkDiscoveryBase<DiscoveryRequest, DiscoveryResponse>
{
    public ServerFoundEvent OnServerFound = new ServerFoundEvent();

    public long serverId { get; private set; }
    public string serverName;

    Transport transport;

    public override void Start()
    {
        base.Start();
        serverId = RandomLong();
        transport = Transport.activeTransport;
    }

    #region Server

    /// <summary>
    /// Process the request from a client
    /// </summary>
    /// <remarks>
    /// Override if you wish to provide more information to the clients
    /// such as the name of the host player
    /// </remarks>
    /// <param name="request">Request comming from client</param>
    /// <param name="endpoint">Address of the client that sent the request</param>
    /// <returns>A message containing information about this server</returns>
    protected override DiscoveryResponse ProcessRequest(DiscoveryRequest request, IPEndPoint endpoint)
    {
        try
        {
            return new DiscoveryResponse
            {
                serverId = serverId,
                uri = transport.ServerUri(),
                serverName = this.serverName
            };
        }
        catch (NotImplementedException)
        {
            Debug.LogError($"Transport {transport} does not support network discovery");
            throw;
        }
    }

    #endregion

    #region Client

    /// <summary>
    /// Process the answer from a server
    /// </summary>
    /// <remarks>
    /// A client receives a reply from a server, this method processes the
    /// reply and raises an event
    /// </remarks>
    /// <param name="response">Response that came from the server</param>
    /// <param name="endpoint">Address of the server that replied</param>
    protected override void ProcessResponse(DiscoveryResponse response, IPEndPoint endpoint)
    {
        // we received a message from the remote endpoint
        response.EndPoint = endpoint;

        // although we got a supposedly valid url, we may not be able to resolve
        // the provided host
        // However we know the real ip address of the server because we just
        // received a packet from it,  so use that as host.
        UriBuilder realUri = new UriBuilder(response.uri)
        {
            Host = response.EndPoint.Address.ToString()
        };
        response.uri = realUri.Uri;

        OnServerFound.Invoke(response);
    }
    #endregion
}