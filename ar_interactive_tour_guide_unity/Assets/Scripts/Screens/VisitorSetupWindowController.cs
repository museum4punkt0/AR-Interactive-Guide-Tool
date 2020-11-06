using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using deVoid.UIFramework;
using deVoid.Utils;
using Mirror;

public class VisitorSetupWindowController : AWindowController
{
    private readonly Dictionary<long, DiscoveryResponse> discoveredServers = new Dictionary<long, DiscoveryResponse>();

    [SerializeField] private GameObject serverListContainer;
    [SerializeField] private Button serverListItemPrefab;

    protected override void AddListeners() {
        App.discovery.OnServerFound.AddListener(OnDiscoveredServer);
    }

    protected override void RemoveListeners() {
        App.discovery?.OnServerFound.RemoveListener(OnDiscoveredServer);
    }

    protected override void OnPropertiesSet() {

        NetworkManager.singleton.StopHost(); // stops server and client

        discoveredServers.Clear();
        foreach (var button in serverListContainer.GetComponentsInChildren<Button>())
            Destroy(button.gameObject);

        App.discovery.StartDiscovery();
    }

    void OnDiscoveredServer(DiscoveryResponse info)
    {
        if(!discoveredServers.ContainsKey(info.serverId))
        {
            var newButton = Instantiate(serverListItemPrefab);
            newButton.transform.SetParent(serverListContainer.transform, false);
            newButton.GetComponentInChildren<Text>().text = info.serverName;
            newButton.onClick.AddListener(() => Connect(info.serverId));
        }
        // Note that you can check the versioning to decide if you can connect to the server or not using this method
        discoveredServers[info.serverId] = info;
    }

    void Connect(long serverId)
    {
        var info = discoveredServers[serverId];
        App.activeTour = TourHelpers.NewTour(); // important to do this before StartClient, as StartClient synchronises state with server!
        (NetworkManager.singleton as CustomNetworkManager).SetUri(info.uri); // hack, as NetworkManager does not save whole uri which we need to reconnect
        NetworkManager.singleton.StartClient(info.uri);
        StartCoroutine(WaitForConnection());
    }

    IEnumerator WaitForConnection()
    {
        // TODO add timeout
        // NOT WORKING!! if started in player, changetoursignal hasnt been subscribed yet (ie networkbehaviours havent been set active)
        // possible workarounds: subscribe in a monobehaviour
        // find way of telling whether it's been subscribed
        // ugly: wait for a second
        yield return new WaitUntil(() => NetworkClient.isConnected && NetworkClient.connection.isReady);

        var props = new ContentWindowControllerProperties() { tour = App.activeTour };

        App.uiFrame.OpenWindow(ScreenList.visitorContentWindow, props);

        // TODO more elegant solution for first starting file
        // right now Properties.startWithFile is null
        // and GuideSync sets the first content in a hook on clients
    }
}