using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static MySelectable;

[RequireComponent(typeof(MySelectable))]
public class Gettable : MonoBehaviour
{
    [Header("Animation params")]
    public float duration = 1f;
    [Range(0.0f, 1.0f)]
    public float lerpParam = 0f;
    public bool controlChildrenAlpha = true;

    private Vector3 defaultPosition;
    private Vector3 defaultScale;
    private Quaternion defaultRotation;
    private GettableGoal goal;

    private MySelectable selectable;
    // private SelectableSystem subsystem;

    private Coroutine movement;

    // flags for what to do on OnDisable
    private bool isGetting;
    private bool isReturning;

    private Renderer[] childrenRenderer;

    private Transform originalParent;

    private void Awake()
    {
        originalParent = transform.parent;

        selectable = GetComponent<MySelectable>();

        selectable.onSelect += OnSelect;
        selectable.onNormal += OnNormal;
        selectable.onInactive += OnInactive;

        //childrenRenderer = GetComponentsInChildren<Renderer>();
        childrenRenderer = GetComponentsInChildren<Renderer>();//.ToList().Where(t => t.transform.GetComponent<KeepTransparent>() == null).ToArray();//material.GetFloat("") != (int)StandardShaderUtils.BlendMode.Fade).ToArray();
    }

    void Start()
	{
        SetDefaultPositionRotation(transform);
        SetGoalTransform(FindObjectOfType<GettableGoal>());
	}

    public void SetDefaultPositionRotation(Transform t)
    {
        defaultPosition = t.position;
        defaultScale = t.lossyScale;
        defaultRotation = t.rotation;
    }

    public void SetGoalTransform(GettableGoal goal)
    {
        this.goal = goal;
    }

	private void OnSelect(SelectableState oldState)
	{
        if (movement != null) StopCoroutine(movement);

        if(gameObject.activeInHierarchy)
            movement = StartCoroutine(DoGetObject());
        else
        {
            lerpParam = 1;
            goal.StartFreeRotation();
        }
	}

    private IEnumerator DoGetObject()
    {
        isGetting = true;
        while (lerpParam <= 1f)
        {
            lerpParam += Time.deltaTime / duration;
            yield return null;
        }
        DoGetObjectImmediately();
    }

    private void DoGetObjectImmediately()
    {
        lerpParam = 1;
        goal.StartFreeRotation();
        isGetting = false;
        foreach (var r in childrenRenderer) // set the rendermode to opaque to enshure a nice rendered model without transparency glitches
            if(r.transform.GetComponent<KeepTransparent>() == null) //only change the rendermode on not transparent objects - especially for puti feathers
                foreach (var mat in r.materials)    // some of the meshes have mutliple materials assigned
                    StandardShaderUtils.ChangeRenderMode(mat, StandardShaderUtils.BlendMode.Opaque);
    }

    private void OnNormal(SelectableState oldState)
	{
        if (oldState == SelectableState.Selected)
        {
            if (movement != null) StopCoroutine(movement);

            if(gameObject.activeInHierarchy)
                movement = StartCoroutine(DoReturnObject());
            else
            {
                lerpParam = 0;
                goal.StopFreeRotation();
            }
        }
    }

    private IEnumerator DoReturnObject()
    {
        isReturning = true;
        foreach (var r in childrenRenderer) // set the rendermode back to fade to enable transparency
            if (r.transform.GetComponent<KeepTransparent>() == null) //only change the rendermode on not transparent objects - especially for puti feathers
                foreach (var mat in r.materials)    // some of the meshes have mutliple materials assigned
                    StandardShaderUtils.ChangeRenderMode(mat, StandardShaderUtils.BlendMode.Fade);
                

        while (lerpParam >= 0f)
        {
            lerpParam -= Time.deltaTime / duration;
            yield return null;
        }
        DoReturnObjectImmediately();
    }

    private void DoReturnObjectImmediately()
    {
        lerpParam = 0;
        goal.StopFreeRotation();
        isReturning = false; 
    }

    private void OnInactive(SelectableState oldState)
	{

    }

    void LateUpdate()
    {
        if(lerpParam < 1)
        {
            if (transform.parent != originalParent)
                transform.SetParent(originalParent, false);

            transform.position = Vector3.Lerp(defaultPosition, goal.transform.position, Mathf.SmoothStep(0, 1, lerpParam));
            transform.SetLossyGlobalScale(Vector3.Lerp(defaultScale, goal.transform.lossyScale, Mathf.SmoothStep(0, 1, lerpParam)));
            transform.rotation = Quaternion.Lerp(defaultRotation, goal.transform.rotation, Mathf.SmoothStep(0, 1, lerpParam));
        }
        else
        {
            if (transform.parent != goal.transform)
                transform.SetParent(goal.transform, false);

            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;
        }

        if (controlChildrenAlpha)
            foreach (var r in childrenRenderer)
                foreach (var mat in r.materials)
                    mat.color = Alpha(mat.color, lerpParam);
    }

    Color Alpha(Color c, float alpha)
    {
        c.a = alpha;
        return new Color(c.r, c.g, c.b, alpha);
    }

    private void OnDisable()
    {
        if (isGetting)
            DoGetObjectImmediately();
        if (isReturning)
            DoReturnObjectImmediately();
    }
}

public static class TransformHelper
{
    // Warning, this is lossy, can be slow and can lead to unwanted feedback loops
    public static void SetLossyGlobalScale(this Transform transform, Vector3 globalScale)
    {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
    }
}