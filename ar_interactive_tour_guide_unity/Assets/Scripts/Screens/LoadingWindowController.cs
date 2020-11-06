using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using deVoid.UIFramework;
using System.Linq;
using UnityEngine.UI;

// TODO low prio ask user before replacing default tour ('update available. update?')
public class LoadingWindowController : AWindowController
{
    public GameObject showOnError;
    public Text errorMessageText;
    public GameObject showAfterWait;

    public int waitingTime = 10;

    string localThemes;
    string localChapters;
    string localItems;

    string remoteThemes;
    string remoteChapters;
    string remoteItems;

    string errorMessage = "";

    protected override void Awake(){
        base.Awake();
    }

    protected override void AddListeners() {
        InTransitionFinished = c => OnInTransitionFinished();
    }

    // guarantees that the gameobject is active
    void OnInTransitionFinished() {
        showOnError.SetActive(false);
        showAfterWait.SetActive(false);
        errorMessage = "";

        StartCoroutine(new WaitForSeconds(waitingTime).Do().Then(() => showAfterWait.SetActive(true)));

        var dataImporter = new DataImporterFromCSV();

        var csvUrl = Settings.LoadNetworkSettings().url;

        StartCoroutine(
        dataImporter.DoImport(csvUrl)
                    .Then(() => OnImportDone(dataImporter.doImportResult, dataImporter.errorOnWebRequest))
        );
    }

    void OnImportDone(Folder data, bool errorOnWebRequest) {

        if (errorOnWebRequest)
            errorMessage += "Could not connect to the server to load the Tour content csv file!";

        else if(data.children.Count == 0)
        {
            errorMessage += "The CSV import encountered an error or the CSV contained no themes. Make sure its content is properly formatted and its fields contain no double quotes!";
            Debug.Log("CSV import failed or CSV contained no themes.");
        }

        else
        {
            var localData = TourHelpers.NewTour().root;


            // This is my favourite hack so far
            // localData has been serialized, deserialized, serialized
            // but data has not
            // BUT that process changes the newline characters of some string values
            // in order to be able to compare the two, apply that process to both

            // TODO low prio proper equality comparison of FileAndFolder
            var localDataString = XmlOperation.Serialize(
                                  XmlOperation.TryDeserializeFromString<Folder>(
                                  XmlOperation.Serialize(localData)));

            var dataString = XmlOperation.Serialize(
                             XmlOperation.TryDeserializeFromString<Folder>(
                             XmlOperation.Serialize(data)));

            // since we have not implemented Equals in FileOrFolder, serialize and compare strings
            if (localDataString != dataString)
            {
                TourHelpers.DeleteCustomTours();

                var defaultTour = new Tour()
                {
                    root = data,
                    profileName = "Default Tour"
                };
                TourHelpers.SaveDefaultTour(defaultTour);
            }
        }
        LoadCache();
    }

    void LoadCache() {

        var fullTour = TourHelpers.NewTour();
        var allFiles = fullTour.root.GetAllDescendantFiles(true);
        //var allImageUrls = allFiles.OfType<ImageFile>();//.Select(file => file.url);
        //var allVideoUrls = allFiles.OfType<VideoFile>();//.Select(file => file.url);
        //var allUrls = allImageUrls.Concat(allVideoUrls);
        var allImageUrls = allFiles.Where(f => f is ImageFile);//.OfType<ImageFile>();
        var allVideoUrls = allFiles.Where(f => f is VideoFile);
        var allUrls = allImageUrls.Concat(allVideoUrls);

        StartCoroutine(App.cache.CacheOnly(allUrls)
                                .Then(() => OnCacheDone(App.cache.errorWhileCacheOnly)));
    }

    void OnCacheDone(bool errorWhileCacheOnly) {

        if (errorWhileCacheOnly)
            errorMessage += "There was an error while downloading and saving videos and photos.";

        if (!string.IsNullOrEmpty(errorMessage))
        {
            showOnError.gameObject.SetActive(true);
            errorMessageText.text = errorMessage;
        }
        else OpenModeSelectionWindow();
    }

    // To be called from a button in the showOnError gameobject
    public void OpenModeSelectionWindow()
    {
        App.uiFrame.OpenWindow(ScreenList.modeSelectionWindow);
    }
}