using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class OLDObjectPlacement : MonoBehaviour {

//    public enum State { INVALID, VALID }

//    [SerializeField]
//    public ARTrackedImageManager arImageManager;

//    private State placementState;

//    public ContentItemID[] contentItemObjects;
//    private ContentItemID chosenObject;

//    public delegate void OnTargetTracked();
//    public static event OnTargetTracked TargetTracked;

//    private void Start() {
//        foreach (ContentItemID contentItemID in contentItemObjects) {
//            contentItemID.RemoveObject();

//        }
//    }

//    void OnEnable() {
//        arImageManager.trackedImagesChanged += OnImagesStateChanged;
//    }

//    void OnDisable() {
//        arImageManager.trackedImagesChanged -= OnImagesStateChanged;
//    }

//    public int GetObjectIndexFromGameobject(GameObject hitGameObject) {
//        for (int i = 0; i < contentItemObjects.Length; i++) {
//            if (contentItemObjects[i].gameObject == hitGameObject) {
//                return i;
//            }
//        }
//        return -1;
//    }

//    //Call to choose object for display
//    public Texture2D ChooseObject(ContentItem item) {
//        chosenObject = null;
//        foreach (ContentItemID contentItemID in contentItemObjects) {
//            if (contentItemID.GetId == item.id) {
//                chosenObject = contentItemID;
//                return contentItemID.ArImageMarker;
//            }
//        }
//        return null;
//    }

//    //Call to enable placement
//    public void ChangeObjectPlacementState(State newState) {

//        placementState = newState;
//        if (newState == State.INVALID) {
//            foreach (ContentItemID arElement in contentItemObjects) {
                
//                arElement.RemoveObject();
//            }
//        } else {
//#if UNITY_EDITOR
//            if (chosenObject != null) {
//                chosenObject.PlaceArContentItem(new Vector3(), Quaternion.identity);

//            }
//#endif
//        }


//    }

//    private void OnImagesStateChanged(ARTrackedImagesChangedEventArgs args) {

//        if (placementState == State.VALID) {
//            foreach (ARTrackedImage trackedImage in args.added) {
//                UpdateImage(trackedImage);
//            }
//            foreach (ARTrackedImage trackedImage in args.updated) {
//                UpdateImage(trackedImage);
//            }
//            foreach (ARTrackedImage trackedImage in args.removed) {
//                UpdateImage(trackedImage);
//            }
//        }

//    }

//    private void UpdateImage(ARTrackedImage trackedImage) {

//        switch (trackedImage.trackingState) {
//            case UnityEngine.XR.ARSubsystems.TrackingState.None:

//                if (chosenObject != null) {
//                    chosenObject.RemoveObject();
//                }
//                break;
//            case UnityEngine.XR.ARSubsystems.TrackingState.Limited:
//                break;
//            case UnityEngine.XR.ARSubsystems.TrackingState.Tracking:
//                if (chosenObject != null) {
//                    chosenObject.PlaceArContentItem(trackedImage.transform.position, trackedImage.transform.rotation);

//                }
//                break;
//        }

//    }
}
