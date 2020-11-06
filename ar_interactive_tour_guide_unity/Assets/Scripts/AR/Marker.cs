using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Linq;
using deVoid.Utils;

[Serializable]
public class ARMarkerFoundSignal : ASignal { }

public class Marker : MonoBehaviour
{
    [Header("Enable to reposition world only the first frame a marker is detected")]
    public bool trackOnlyFirstFrame;

    [Header("Must be the same as the corresponding name in the Reference Image Library")]
    public string referenceImageName;
    [SerializeField] private GameObject showWhenTracked;

    private ARSessionOrigin sessionOrigin;
    private ARCameraPositioner cameraPositioner;
    private ARTrackedImageManager imageManager;

    void Awake()
    {
        sessionOrigin = FindObjectOfType<ARSessionOrigin>();
        cameraPositioner = FindObjectOfType<ARCameraPositioner>();

        imageManager = FindObjectOfType<ARTrackedImageManager>();

        Assert.IsNotNull(sessionOrigin);
        Assert.IsNotNull(imageManager);

        imageManager.trackedImagesChanged += OnTrackablesChanged;

        if (showWhenTracked != null)
            showWhenTracked.SetActive(false);
    }

    ARTrackedImage lastTrackedImage;

    private void OnTrackablesChanged(ARTrackedImagesChangedEventArgs args)
    {
        if (!isActiveAndEnabled)
            return;

        ARTrackedImage trackedImage = args.updated.Concat(args.added)
                                              .FirstOrDefault(i => i.referenceImage.name == referenceImageName && i.trackingState == TrackingState.Tracking);

        if (trackedImage != null)
        {

            //GameObject unscaledImage = new GameObject();
            //unscaledImage.transform.SetPositionAndRotation(trackedImage.transform.position,trackedImage.transform.rotation);
            //unscaledImage.transform.localScale = Vector3.one;

            if(trackOnlyFirstFrame)
            {
                // this check is a hack:
                // we want to updated only when an image is newly tracked
                // as otherwise the accuracy might be quite bad
                // however, this is not reflected in the updated vs added lists (images are in updated way too often)
                if (trackedImage != lastTrackedImage)
                {
                    // this moves the _whole_ Session Origin, not this marker
                    // this marker will appear where the tracked image is found
                    // and everything else will follow
                    //sessionOrigin.MakeContentAppearAt(transform, trackedImage.transform.position, trackedImage.transform.localRotation);
                    cameraPositioner.MakeContentAppearAt(transform, Space.World, trackedImage.transform, Space.Self);
                    
                    
                }
            }

            else
            {
                cameraPositioner.MakeContentAppearAt(transform, Space.World, trackedImage.transform, Space.Self);

            }
            
            if (trackedImage != lastTrackedImage)
            {
                Signals.Get<ARMarkerFoundSignal>().Dispatch();

                var scene = gameObject.GetComponentInParent<ARScene>();
                if (scene != null)
                    scene.MarkerFound();
            }

            if (showWhenTracked != null)
                showWhenTracked.SetActive(true);
        }

        else if (showWhenTracked != null)
            showWhenTracked.SetActive(false);

        lastTrackedImage = trackedImage;
    }

    // called from editor script!
    public void OnMarkerFound()
    {
        if (showWhenTracked != null)
            showWhenTracked.SetActive(true);

        var scene = gameObject.GetComponentInParent<ARScene>();
        if (scene != null)
            scene.MarkerFound();

        Signals.Get<ARMarkerFoundSignal>().Dispatch();
    }
}