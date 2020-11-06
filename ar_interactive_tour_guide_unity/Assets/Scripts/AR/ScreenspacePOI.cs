using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// This is a container for Canvas elements
// that will follow a POI on the screen
// the setContentAtive flag and the content Transform is a workaround for a bug in Unity
// where calling SetActive in some other go's OnDisable will result in a OutOfBounds exception

// A ScreenspacePOI MUST NOT contain any other components. All functionality other than the ScreenspacePOI
// must be in its ONLY child, the content transform

[ExecuteAlways]
public class ScreenspacePOI : MonoBehaviour
{
    public Action requestSelect;
    public Action requestClose;

    public Transform content;
    public bool setContentActive = true;

    [SerializeField] public LocalisedText screenspaceLabel;
    [SerializeField] public LocalisedText infoTitle;
    [SerializeField] public LocalisedText infoSubtitle;
    [SerializeField] public LocalisedText infoContent;

   
    // public Button labelButton;
    // public Button deselectButton;
    public RectTransform labelContainer;

    [Header("visibility behaviour")]
    [Header("MUST NOT contain the labelContainer directly")]

    [SerializeField] public GameObject[] showOnSelect = new GameObject[0];
    [SerializeField] public GameObject[] showOnNormal = new GameObject[0];
    [SerializeField] public GameObject[] showOnInactive = new GameObject[0];

    [Header("Occlusion Vehabiour")]
    [Tooltip("bool not implemented yet WIP")]
    [SerializeField] public bool tagOccludingBehaviour = true;
    [SerializeField] public string OccludableTag = "Occludable";

    //public Vector3 poiPosition { get; set; }
    public Transform follow;
    private string occludableHitTag;
    private string _occludableHitTag;

    void Start()
    {
        //OnNormal(MySelectable.SelectableState.Inactive);
        SetValues();
    }

    void Update()
    {
        SetValues();
        CheckAndSetVisibility();

        OccludableTag = follow.tag;
    }

    private void CheckAndSetVisibility()
    {
        if (occludableHitTag != OccludableTag)
        {
            // dont show UI element
            //gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            if (TryGetComponent(out Animator animator)) {

                animator.SetBool("isVisible", false);

            } 
            

        }

        // setting up a ray from 
        var ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(follow.position));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {

            if (hit.transform.CompareTag(OccludableTag))
            {
                // show UI element

                if (TryGetComponent(out Animator animator))
                {

                    animator.SetBool("isVisible", true);

                }
                //gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);

            }

            occludableHitTag = hit.transform.tag;
            
        }

    }

    void SetValues()
    {
        if(content != null)
            content.gameObject.SetActive(setContentActive);
           

        if (labelContainer != null && follow != null)
        {
            var cam = Camera.main;

            // if label is in front of camera, not behind
            if (Vector3.Dot(cam.transform.forward, follow.position - cam.transform.position) >= 0)
            {
                labelContainer.gameObject.SetActive(true);
                Vector2 viewportPoint = WorldToViewportPointAR(Camera.main, follow.position);

                labelContainer.anchorMin = viewportPoint;
                labelContainer.anchorMax = viewportPoint;
            }

            else labelContainer.gameObject.SetActive(false);
        }
    }
    
    public void Select()
    {
        requestSelect?.Invoke();
    }

    public void Close()
    {
        requestClose?.Invoke();
    }

    Vector2 WorldToViewportPointAR(Camera cam, Vector3 worldPosition)
    {
        var view = cam.worldToCameraMatrix;
        var proj = cam.projectionMatrix;

        var screenPoint = (Vector2) proj.MultiplyPoint(view.MultiplyPoint(worldPosition));
        screenPoint *= App.POIpositionScalingFactor;

        return screenPoint * 0.5f + new Vector2(0.5f, 0.5f);
    }
}