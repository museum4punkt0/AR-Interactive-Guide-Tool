using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DigitalRubyShared;
using System;

public class Rotatable : MonoBehaviour
{
    Coroutine spinOutCoroutine;
    Quaternion spinOutRotation;
    float spinOutDragFactor = 0.95f;

    Vector3 lastRotationLever;

    private PanGestureRecognizer panGestureRecognizer;
    private RotateGestureRecognizer rotateGestureRecognizer;

    void Awake()
    {
        panGestureRecognizer = new PanGestureRecognizer
        {
            MinimumNumberOfTouchesToTrack = 1,
            MaximumNumberOfTouchesToTrack = 1
        };

        panGestureRecognizer.AllowSimultaneousExecutionWithAllGestures();
        panGestureRecognizer.StateUpdated += PanStateUpdated;
        FingersScript.Instance.AddGesture(panGestureRecognizer);

        rotateGestureRecognizer = new RotateGestureRecognizer
        {
            MinimumNumberOfTouchesToTrack = 2,
            MaximumNumberOfTouchesToTrack = 2
        };

        rotateGestureRecognizer.AllowSimultaneousExecutionWithAllGestures();
        rotateGestureRecognizer.StateUpdated += RotateStateUpdated;
        FingersScript.Instance.AddGesture(rotateGestureRecognizer);

        rotateGestureRecognizer.DisallowSimultaneousExecution(panGestureRecognizer);
    }

    private void PanStateUpdated(GestureRecognizer gesture)
    {
        switch(panGestureRecognizer.State)
        {
                case GestureRecognizerState.Began:
                    OnDragBegin(new Vector2(panGestureRecognizer.FocusX, panGestureRecognizer.FocusY));
                    break;
                case GestureRecognizerState.Executing:
                    OnDrag(panGestureRecognizer.Focus(), panGestureRecognizer.Drag());
                    break;
                case GestureRecognizerState.Ended:
                    OnDragEnd(panGestureRecognizer.Focus());
                    break;
        }
    }

    private void RotateStateUpdated(GestureRecognizer r)
    {
        switch(rotateGestureRecognizer.State)
        {
            case GestureRecognizerState.Began:
                OnRotateBegin();
                break;
            case GestureRecognizerState.Executing:
                OnRotate(rotateGestureRecognizer.RotationDegreesDelta);
                break;
            case GestureRecognizerState.Ended:
                OnRotateEnd();
                break;
        }
    }

    public void OnDragBegin(Vector2 position)
    {
        if (spinOutCoroutine != null)
            StopCoroutine(spinOutCoroutine);

        lastRotationLever = RotationLever(position);
    }

    public void OnDrag(Vector2 position, Vector2 drag)
    {
        // naive version
        // transform.Rotate(transform.parent.transform.up, - dragFactor * drag.x, Space.World);
        // transform.Rotate(transform.parent.transform.right, dragFactor * drag.y, Space.World);

        // MOCO version
        var lever = RotationLever(position);
        var rotate = Quaternion.FromToRotation(lastRotationLever, lever);

        transform.rotation = rotate * transform.rotation;

        lastRotationLever = lever;

        spinOutRotation = rotate;
    }

    private Vector3 RotationLever(Vector2 mousePosition)
    {
        float distance = Vector3.Distance(Camera.main.transform.position, transform.position);

        distance /= 1.5f; // chosen arbitrarily to enable good interaction

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        ray.origin = Camera.main.transform.position; // default is point on near plane

        var interactionPoint = ray.GetPoint(distance);
        return interactionPoint - transform.position;
    }

    public void OnDragEnd(Vector2 position)
    {
        if (spinOutCoroutine != null)
            StopCoroutine(spinOutCoroutine);

        if(isActiveAndEnabled)
            spinOutCoroutine = StartCoroutine(SpinOut());
    }

    private void OnRotateBegin()
    {
        if (spinOutCoroutine != null)
            StopCoroutine(spinOutCoroutine);
    }

    private void OnRotate(float degreesDelta)
    {
        var cam = Camera.main;

        var rotate = Quaternion.AngleAxis(degreesDelta, cam.transform.forward);

        transform.rotation = rotate * transform.rotation;

        spinOutRotation = rotate;
    }

    private void OnRotateEnd()
    {
        if (spinOutCoroutine != null)
            StopCoroutine(spinOutCoroutine);

        if(isActiveAndEnabled)
            spinOutCoroutine = StartCoroutine(SpinOut());
    }

    IEnumerator SpinOut()
    {
        while (Quaternion.Angle(Quaternion.identity, spinOutRotation) > 0.01)
        {
            transform.rotation = spinOutRotation * transform.rotation;
            spinOutRotation = Quaternion.Lerp(Quaternion.identity, spinOutRotation, spinOutDragFactor);
            yield return new WaitForFixedUpdate();
        }
    }

    void OnDestroy()
    {
        panGestureRecognizer.StateUpdated -= PanStateUpdated;
        rotateGestureRecognizer.StateUpdated -= RotateStateUpdated;
    }
}

public static class FingersExtensions
{
    static public Vector2 Focus(this PanGestureRecognizer r)
    {
        return new Vector2(r.FocusX, r.FocusY);
    }

    static public Vector2 Drag(this PanGestureRecognizer r)
    {
        return new Vector2(r.DistanceX, r.DistanceY);
    }
}