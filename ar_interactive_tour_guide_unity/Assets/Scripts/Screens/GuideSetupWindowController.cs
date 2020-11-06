using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using deVoid.UIFramework;
using deVoid.Utils;
using Mirror;

public class GuideSetupWindowController : AWindowController
{
    [SerializeField] private GameObject guideProfileListContainer;

    [SerializeField] private GuideProfileListEntry guideProfileListEntryPrefab;

    public void CreateNewTour()
    {
        var newTour = TourHelpers.NewTour();
        newTour.profileName = "Your Name";
        App.uiFrame.OpenWindow(ScreenList.profileNameWindow, new ProfileNameWindowControllerProperties(newTour));
    }

    protected override void OnPropertiesSet() {

        NetworkManager.singleton.StopHost();

        foreach(var entry in guideProfileListContainer.GetComponentsInChildren<GuideProfileListEntry>())
            Destroy(entry.gameObject);
        
        foreach(var tour in TourHelpers.LoadTours())
        {
            var newEntry = Instantiate(guideProfileListEntryPrefab);
            newEntry.transform.SetParent(guideProfileListContainer.transform, false);
            newEntry.transform.SetAsFirstSibling();
            // it is important to set as first sibling because of unity ui restrictions
            // ie having dynamically sized children of a layout group is very buggy if working at all
            // and we want a new profile button as last entry

            newEntry.SetName(tour.profileName);
            
            newEntry.startWithProfileButton.onClick.AddListener(() => StartTour(tour));
            newEntry.editProfileButton.onClick.AddListener(() => EditProfile(tour));
            newEntry.deleteProfileButton.onClick.AddListener(() => DeleteProfile(tour, newEntry));
        }
    }

    void StartTour(Tour tour)
    {
        NetworkManager.singleton.StartHost();
        App.discovery.serverName = tour.profileName;
        App.discovery.AdvertiseServer();
        StartCoroutine(WaitForLocalConnection(tour));
    }

    void EditProfile(Tour tour)
    {
        App.uiFrame.OpenWindow(ScreenList.profileEditWindow, new ProfileEditWindowControllerProperties(tour));
    }

    void DeleteProfile(Tour tour, GuideProfileListEntry entry)
    {
        Destroy(entry.gameObject);
        try 
        {
            TourHelpers.DeleteTour(tour);
        }
        catch
        {
            Debug.Log("Could not delete tour with uniqueId: " + tour.uniqueId);
        }
    }

    IEnumerator WaitForLocalConnection(Tour tour)
    {
        yield return new WaitUntil(() => NetworkServer.localConnection.isReady);
        App.activeTour = tour; // TODO make this irrelevant - we're passing the tour to the content window already
        // App.guideSync.ShowContent(App.activeTour.GetFirstFile());

        var props = new ContentWindowControllerProperties() { startWithFile = tour.GetFirstFile(false), tour = tour };

        App.uiFrame.OpenWindow(ScreenList.guideContentWindow, props);
    }
}