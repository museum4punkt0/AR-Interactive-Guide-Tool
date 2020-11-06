using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRubyShared;
using System;

public class Scalable : MonoBehaviour
{
    [Header("Min and max scale. Max must be bigger than min. Set negative to disable.")]
    public float minScale;
    public float maxScale = 10;

    /// <summary>Additional multiplier for ScaleMultiplier. This will making scaling happen slower or faster.</summary>
    [Header("Scale gesture properties")]
    [Tooltip("Additional multiplier for ScaleMultiplier. This will making scaling happen slower or faster.")]
    [Range(0.0001f, 10.0f)]
    public float zoomSpeed = 1.0f;

    /// <summary>How many units the distance between the fingers must increase or decrease from the start distance to begin executing.</summary>
    [Tooltip("How many units the distance between the fingers must increase or decrease from the start distance to begin executing.")]
    [Range(0.01f, 1.0f)]
    public float thresholdUnits = 0.15f;

    private ScaleGestureRecognizer scaleGestureRecognizer;
    // Start is called before the first frame update
    void Awake()
    {
        scaleGestureRecognizer = new ScaleGestureRecognizer
        {
            ZoomSpeed = zoomSpeed,
            ThresholdUnits = thresholdUnits,

            MinimumNumberOfTouchesToTrack = 2,
            MaximumNumberOfTouchesToTrack = 2
        };

        scaleGestureRecognizer.AllowSimultaneousExecutionWithAllGestures();
        scaleGestureRecognizer.StateUpdated += ScaleStateUpdated;
        FingersScript.Instance.AddGesture(scaleGestureRecognizer);
    }

    private void ScaleStateUpdated(GestureRecognizer gesture)
    {
        if (!gameObject.activeInHierarchy || !isActiveAndEnabled)
            return;

        if (gesture.State == GestureRecognizerState.Executing)
        {
            Vector3 scale = transform.localScale * scaleGestureRecognizer.ScaleMultiplier;

            if (minScale >= 0)
            {
                scale.x = Mathf.Max(scale.x, minScale);
                scale.y = Mathf.Max(scale.y, minScale);
                scale.z = Mathf.Max(scale.z, minScale);
            }
            if (maxScale >= 0 && maxScale >= minScale)
            { 
                scale.x = Mathf.Min(scale.x, maxScale);
                scale.y = Mathf.Min(scale.y, maxScale);
                scale.z = Mathf.Min(scale.z, maxScale);
            }

            transform.localScale = scale;
        }
    }

    private void OnDestroy()
    {
        scaleGestureRecognizer.StateUpdated -= ScaleStateUpdated;
    }
}