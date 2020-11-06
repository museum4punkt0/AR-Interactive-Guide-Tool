using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OLDUserInput : MonoBehaviour {
    //[SerializeField]
    //private Camera arCamera;
    //[SerializeField]
    //private ARSessionManager aRSessionManager;
    //[SerializeField]
    //private LayerMask highlightableObjectsLayerMask;
    //[SerializeField]
    //private bool allowedUserInput = true;

    //private Vector2 startTouchPosition;
    //private float minMoveDistance = 5.0f;

    //private float pinchDistanceDelta;
    //private float pinchDistance;
    //const float pinchTurnRatio = Mathf.PI / 2;
    //const float pinchRatio = 1;
    //const float minPinchDistance = 0;

    //void Update() {

    //    if (Input.GetMouseButtonDown(0))
    //    {
           
    //        RaycastHit hit = new RaycastHit();

    //        if (Physics.Raycast(arCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, highlightableObjectsLayerMask)) {
    //            TouchedOnScreen(hit, true);
    //        }
    //            else if (Physics.Raycast(arCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity)) {
    //                    TouchedOnScreen(hit, false);
    //        }

    //    }


    //    if (Input.touchCount == 1) {
           
    //        Touch touchZero = Input.GetTouch(0);

    //        RaycastHit hit = new RaycastHit();

    //        switch (touchZero.phase) {
    //            case TouchPhase.Began:
    //                if (Physics.Raycast(arCamera.ScreenPointToRay(touchZero.position), out hit, Mathf.Infinity, highlightableObjectsLayerMask)) {
    //                    TouchedOnScreen(hit, true);
    //                } else if (Physics.Raycast(arCamera.ScreenPointToRay(touchZero.position), out hit, Mathf.Infinity)) {
    //                    if (EventSystem.current.IsPointerOverGameObject(touchZero.fingerId) == true) {
    //                        return;
    //                    }
    //                    TouchedOnScreen(hit, false);
    //                }
    //                startTouchPosition = touchZero.position;

    //                break;
    //            case TouchPhase.Moved:
    //            case TouchPhase.Stationary:

    //                Vector3 touchDelta = touchZero.position - startTouchPosition;
    //                MovingOnScreen(touchDelta);
    //                startTouchPosition = touchZero.position;

    //                break;
    //            case TouchPhase.Ended:
    //                startTouchPosition = new Vector2();
    //                break;
    //            case TouchPhase.Canceled:
    //                break;
    //            default:
    //                break;
    //        }
    //    }

    //    pinchDistance = 1;
    //    pinchDistanceDelta = 0;

    //    if (Input.touchCount == 2) {

    //        Touch touchZeroP = Input.GetTouch(0);
    //        Touch touchOneP = Input.GetTouch(1);

    //        if (EventSystem.current.IsPointerOverGameObject(touchZeroP.fingerId) == true || EventSystem.current.IsPointerOverGameObject(touchZeroP.fingerId) == true) {
    //            return;
    //        }

    //        if (touchZeroP.phase == TouchPhase.Moved || touchOneP.phase == TouchPhase.Moved) {
    //            // Delta distance
    //            pinchDistance = Vector2.Distance(touchZeroP.position, touchOneP.position);
    //            float prevDistance = Vector2.Distance(touchZeroP.position - touchZeroP.deltaPosition,
    //                                                  touchOneP.position - touchOneP.deltaPosition);
    //            pinchDistanceDelta = pinchDistance - prevDistance;

    //            // Greater than a minimum threshold - pinch!
    //            if (Mathf.Abs(pinchDistanceDelta) > minPinchDistance) {
    //                pinchDistanceDelta *= pinchRatio;
    //            } else {
    //                pinchDistance = pinchDistanceDelta = 0;
    //            }
    //        }

    //        if (Mathf.Abs(pinchDistanceDelta) > 0) {
    //            Pinching(pinchDistanceDelta);
    //        }
    //    }
    //}

    //public void AllowUserInput(bool allow) {
    //    allowedUserInput = allow;
    //}

    //public bool GetAllowUserInputState() {
    //    return allowedUserInput;
    //}

    //private void TouchedOnScreen(RaycastHit hit, bool highlight) {
    //    Debug.Log("TouchedOnScreen highlight: " + highlight);
    //    if (allowedUserInput == false) {
    //        return;
    //    }

    //    if (highlight == true) {
    //        aRSessionManager.appFlowManager.HighLightArObject(hit.transform.gameObject.name);
    //    } else {
    //        if (hit.transform.gameObject != null) {
    //            aRSessionManager.appFlowManager.FocusOnArObject(hit.transform.gameObject.name);
    //        }
    //    }
    //}

    //private void MovingOnScreen(Vector3 delta) {
    //    aRSessionManager.objectManipulation.RotateObject(delta*.1f);
    //}

    //private void Pinching(float pinchFactor) {
    //    aRSessionManager.objectManipulation.scaleObject(pinchFactor);
    //}

    //[ContextMenu("TestSelect")]
    //public void TestSelect() {
    //    aRSessionManager.appFlowManager.FocusOnArObject("ArElement2");
    //}
}
