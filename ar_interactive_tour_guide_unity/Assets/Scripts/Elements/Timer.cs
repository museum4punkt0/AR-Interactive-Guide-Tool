using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using deVoid.Utils;

[Serializable] public class ResetTimerSignal : ASignal { }

[Serializable] public class TimerUpdate : UnityEvent<string> { }
public class Timer : MonoBehaviour
{
    public TimerUpdate onUpdate = new TimerUpdate();

    private DateTime startTime;
    private int lastSeconds;

    private void Awake()
    {
        Signals.Get<ResetTimerSignal>().AddListener(Reset);
    }
    void Start()
    {
        Reset();
    }

    private void Update()
    {
        var span = DateTime.UtcNow - startTime;

        int newSeconds = (int) Math.Floor(span.TotalSeconds);

        if (newSeconds != lastSeconds)
            UpdateText();

        lastSeconds = newSeconds;
    }

    public void Reset()
    {
        startTime = DateTime.UtcNow;
        lastSeconds = 0;
        UpdateText();
    }

    void UpdateText()
    {
        var span = DateTime.UtcNow - startTime;
        string format;

        if (span.TotalHours >= 1)
            format = @"h\:mm\:ss";
        else format = @"mm\:ss";

        onUpdate.Invoke(span.ToString(format));
    }

    private void OnDestroy()
    {
        Signals.Get<ResetTimerSignal>().RemoveListener(Reset);
    }
}