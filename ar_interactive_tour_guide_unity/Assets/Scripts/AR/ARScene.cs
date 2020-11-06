using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

// not elegant: this is only a NetworkBehaviour in order
// to know whether we should open the Guide or Visitor AR Screen

[RequireComponent(typeof(SelectableSystem))]
public class ARScene : MonoBehaviour
{
    public string uniqueName;

    [SerializeField] private Animator animator;

    SelectableSystem selectableSystem;

    private void Awake()
    {
        selectableSystem = GetComponent<SelectableSystem>();
    }

    public void Reset()
    {
        if(animator != null)
        {
            animator.ResetTrigger("Marker Found");
            animator.SetTrigger("Reset");
        }
        selectableSystem.ResetAll();
    }

    public void MarkerFound()
    {
        if(animator != null)
        {
            animator.ResetTrigger("Reset");
            animator.SetTrigger("Marker Found");
        }
    }
}
