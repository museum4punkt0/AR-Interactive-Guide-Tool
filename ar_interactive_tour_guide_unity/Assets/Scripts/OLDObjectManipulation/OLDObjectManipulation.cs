using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OLDObjectManipulation : MonoBehaviour {

    //[SerializeField]
    //private ARSessionManager aRSessionManager;
    //[SerializeField]
    //private Transform arCameraAnchor;
    //[SerializeField]
    //private ArElement currentArElement;

    //private float speed = 0.5f;
    //private float maxScaleFactor = 2.0f;
    //private float minScaleFactor = 0.5f;
    //private float scaleSpeed = 0.001f;
    //private float scalingPercentage;

    //public void SelectObject(string name) {
    //    //if there is already something selected dont select a new one
    //    if (currentArElement != null) {
    //        return;
    //    }
    //    GameObject selectedGameObject = GameObject.Find(name);
    //    if (selectedGameObject != null) {
    //        Label label = selectedGameObject.GetComponent<Label>();

    //        if (label != null) {
    //            currentArElement = GameObject.Find(label.GetObjectName).GetComponent<ArElement>();
    //        } else {
    //            currentArElement = GameObject.Find(name).GetComponent<ArElement>();
    //        }

    //        if (currentArElement != null && currentArElement.GetCurrentState == ArElement.State.AR) {
    //            ManageArElementLabels(false);
    //            if (currentArElement.IsAbleToBringOnScreen == true) {
    //                StartCoroutine(TransitionToScreen(currentArElement, arCameraAnchor.transform, ArElement.State.SCREEN));
    //            } else {
    //                currentArElement.ActivateState(ArElement.State.SCREEN);
    //            }
    //            aRSessionManager.SetArPanel(currentArElement.title, currentArElement.subTitle, currentArElement.GetPointsOfInterest);
    //        }
    //    }
    //}

    //public void scaleObject(float scaleFactor) {
    //    if (currentArElement != null && currentArElement.IsAbleToBringOnScreen == true) {
    //        //Vector3 startScale = currentArElement.GetRelativeScale;
    //        //if (scaleFactor > 0) {
    //        //    if (currentArElement.transform.localScale.x + scaleFactor * speed > startScale * maxScaleFactor) {
    //        //        return;
    //        //    }
    //        //} else {
    //        //    if (currentArElement.transform.localScale.x + scaleFactor * speed < startScale * minScaleFactor) {
    //        //        return;
    //        //    }
    //        //}

    //        var tempScale =  currentArElement.scaleFocus + Vector3.one * scaleFactor * scaleSpeed;

    //        if (tempScale.x < .1f || tempScale.x > 1.1f) return;

    //        currentArElement.scaleFocus = tempScale;
    //        currentArElement.transform.localScale = tempScale;
    //    }
    //}

    //public void Highlight(string name) {
    //    if (currentArElement != null) {
    //        foreach (PointOfInterest point in currentArElement.GetPointsOfInterest) {
    //            if (point.name == name) {
    //                currentArElement.HighlightObject(point.highlightTexture);
    //            }
    //        }
    //    }
    //}

    //public void DeHighlightObject() {
    //    currentArElement.DeHighlight();
    //}

    //public void RotateObject(Vector3 rotateFactor) {
    //    if (currentArElement != null && currentArElement.GetCurrentState == ArElement.State.SCREEN && currentArElement.IsAbleToBringOnScreen == true) {
    //        currentArElement.transform.rotation = Quaternion.Euler(rotateFactor.y, -rotateFactor.x, 0) * currentArElement.transform.rotation;
    //    }
    //}

    //[ContextMenu("Release AR Element")]
    //public void Release() {
    //    if (currentArElement != null && currentArElement.GetCurrentState == ArElement.State.SCREEN) {
    //        ManageArElementLabels(true);
    //        aRSessionManager.visitorUiManager.ShowFindArTargetPanel(false);
    //        StartCoroutine(TransitionToScreen(currentArElement, currentArElement.GetAnchor.transform, ArElement.State.AR));
    //    }
    //}

    //IEnumerator TransitionToScreen(ArElement arElementToMove, Transform targetTransform, ArElement.State targetState) {

    //    float timestart = Time.time;
    //    float timeToTarget = 0.5f;
    //    float step = 0;

    //    arElementToMove.ActivateState(ArElement.State.TRANSITION);
    //    arElementToMove.transform.parent = null;

    //    Vector3 startPosition = arElementToMove.transform.position;
    //    Quaternion startRotation = arElementToMove.transform.rotation;
    //    Vector3 startScale = arElementToMove.transform.localScale;
     
    //    Vector3 targetPosition = new Vector3();
    //    Vector3 targetScale = new Vector3();
        
    //    if (targetState == ArElement.State.AR) {
    //       // transition back
    //        targetPosition = targetTransform.position + arElementToMove.GetAnchorRelativePosition;
    //        targetScale = arElementToMove.scale;
    //    } else {
    //       // transition in
    //        targetPosition = targetTransform.position;
    //        targetScale = arElementToMove.scaleFocus;

    //        // ScaleToScreen(arElementToMove);
    //    }

    //    while (step < 1.0f) {

    //        step += Time.deltaTime / timeToTarget;

    //        arElementToMove.transform.position = Vector3.Lerp(startPosition, targetPosition, step);
    //        arElementToMove.transform.localScale =  Vector3.Lerp(startScale, targetScale, step);
    //        if (targetState == ArElement.State.AR) {
               
    //            arElementToMove.transform.rotation = Quaternion.Lerp(startRotation, arElementToMove.GetAnchorRelativeRotation, step);
    //        }
    //        yield return null;
    //    }

    //    arElementToMove.transform.parent = targetTransform;
    //    if (targetState == ArElement.State.AR) {
           
    //        arElementToMove.transform.localPosition = arElementToMove.GetAnchorRelativePosition;
    //        arElementToMove.transform.rotation = arElementToMove.GetAnchorRelativeRotation;
        

    //        DeHighlightObject();

    //    } else {
          
    //        arElementToMove.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    //    }

    //    arElementToMove.ActivateState(targetState);
    //    if (targetState == ArElement.State.AR) {
    //        currentArElement = null;
    //    }
    //}
    ///*
    //private void ScaleToScreen( ArElement arElementToMove)
    //{
    //    Bounds bounds = arElementToMove.modelRenderer.bounds;
    //    Vector3 center = bounds.center;
    //    float radius = bounds.extents.magnitude;
    //    // Vector3 top = bounds.center + Camera.main.transform.up * magnitude;



    //}*/

    //private void ManageArElementLabels(bool enable) {
    //    foreach (ContentItemID item in aRSessionManager.objectPlacement.contentItemObjects) {
    //        if (item.isActiveAndEnabled) {
    //            foreach (ArElement element in item.arElements) {
    //                element.label.gameObject.SetActive(enable);
    //            }
    //        }
    //    }
    //}
}
