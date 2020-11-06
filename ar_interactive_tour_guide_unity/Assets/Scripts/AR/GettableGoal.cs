using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rotatable))]
public class GettableGoal : MonoBehaviour
{
    Quaternion initialRotation;
    Vector3 initialScale;
    Rotatable rotatable;
    Scalable scalable;

    private void Awake()
    {
        rotatable = GetComponent<Rotatable>();
        scalable = GetComponent<Scalable>();

        initialRotation = transform.localRotation;
        initialScale = transform.localScale;
        rotatable.enabled = false;
        scalable.enabled = false;
    }

    public void StartFreeRotation()
    {
        initialRotation = transform.localRotation;
        initialScale = transform.localScale;
        rotatable.enabled = true;
        scalable.enabled = true;
    }

    public void StopFreeRotation()
    {
        transform.localRotation = initialRotation;
        transform.localScale = initialScale;
        rotatable.enabled = false;
        scalable.enabled = false;
    }
}