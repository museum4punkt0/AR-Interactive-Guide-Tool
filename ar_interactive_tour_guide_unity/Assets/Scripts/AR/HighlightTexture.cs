using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class HighlightTexture : MonoBehaviour
{
    [Header("assign Highlight Texture")]
    public Texture2D highlightTex;

    [Header("assign if specific 3D Model")] // some 3d models contain multiple renderer so here you can assing a specific renderer
    public Renderer _objectRenderer;


    [Header("animation settings")]      // tested and found the values offset:0.75, amplitude:1, maxIntensity:2 and speed3
    [Range(-1.0f, 1.0f)]
    public float offset = 0.75f;
    [Range(0.0f, 1.0f)]
    public float ampitude = 1f;
    public float maxIntensity = 2f;
    public float speed = 3;

    private Color highColor = new Color(0.75f, 0.75f, 0.75f, 1);
    
    private Renderer objectRenderer
    {
        //set { _objectRenderer = value; }
        get
        {
            if (_objectRenderer == null)    // if no specific renderer was assigned find the closest one
            {
                _objectRenderer = findNearest3DObject();
            }
            return _objectRenderer;
        }
    }

    private MySelectable selectable;

    private float osc;
    private Material m;
    private bool doUpdate = false;

    private void Awake()
    {
        selectable = GetComponent<MySelectable>();

        selectable.onSelect += OnSelect;
        selectable.onNormal += OnNormal;
        selectable.onInactive += OnNormal;
        selectable.onHide += OnNormal;

        var sharedmaterials = new List<Material>();
        objectRenderer.GetSharedMaterials(sharedmaterials);

        foreach (var mat in sharedmaterials)
        {
            mat.SetTexture("_EmissionMap", Texture2D.blackTexture);
            mat.EnableKeyword("_EMISSION");
        }
           

        Assert.IsNotNull(selectable);
        Assert.IsNotNull(objectRenderer);
        if (highlightTex == null) highlightTex = Texture2D.whiteTexture;    // dummy white texture 
        //Assert.IsNotNull(highlightTex);
    }

    private void OnSelect(MySelectable.SelectableState oldState)
    {
        doUpdate = true;
        if (objectRenderer != null && highlightTex != null)
        {
            var sharedmaterials = new List<Material>();
            objectRenderer.GetSharedMaterials(sharedmaterials);
            foreach (var mat in sharedmaterials)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetTexture("_EmissionMap", highlightTex);
                mat.SetColor("_EmissionColor", highColor);
            }
        }
    }

    private void OnNormal(MySelectable.SelectableState oldState)
    {
        doUpdate = false;
        if (objectRenderer != null)
        {
            var sharedmaterials = new List<Material>();
            objectRenderer.GetSharedMaterials(sharedmaterials);
            foreach (var mat in sharedmaterials)
                mat.SetTexture("_EmissionMap", Texture2D.blackTexture);
        }
    }

    public void PreviewTexture()
    {
        if (highlightTex != null)
        {
            List<Material> mats = new List<Material>();
            objectRenderer.GetSharedMaterials(mats);
            foreach (var mat in mats)
            {
                mat.SetFloat("_EmissionIntensity", maxIntensity);
                mat.SetTexture("_EmissionMap", highlightTex);
                //objectRenderer.sharedMaterial = m;
            }
            //m = objectRenderer.sharedMaterial;
            //m.SetFloat("_EmissionIntensity", maxIntensity);
            //m.SetTexture("_EmissionMap", highlightTex);
            //objectRenderer.sharedMaterial = m;
            //StartCoroutine(resetTexture());
        }
        else if (objectRenderer != null)
        {
            List<Material> mats = new List<Material>();
            objectRenderer.GetSharedMaterials(mats);
            foreach (var mat in mats)
            {
                mat.SetFloat("_EmissionIntensity", maxIntensity);
                mat.SetColor("_EmissionColor", highColor);
                mat.SetTexture("_EmissionMap", Texture2D.whiteTexture);
            }

            //m = objectRenderer.sharedMaterial;
            //m.SetFloat("_EmissionIntensity", maxIntensity);
            //m.SetColor("_EmissionColor", highColor);
            //m.SetTexture("_EmissionMap", Texture2D.whiteTexture);
            //objectRenderer.sharedMaterial = m;
        }

    }
    public void resetTexture()
    {
        List<Material> mats = new List<Material>();
        objectRenderer.GetSharedMaterials(mats);
        foreach (var mat in mats)
        {
            //mat.SetFloat("_EmissionIntensity", 0);
            mat.SetColor("_EmissionColor", highColor);
            mat.SetTexture("_EmissionMap", Texture2D.blackTexture);
        }

        //m = objectRenderer.sharedMaterial;
        //m.SetFloat("_EmissionIntensity", 0);
        //m.SetTexture("_EmissionMap", Texture2D.blackTexture);

        //objectRenderer.sharedMaterial = m;
    }
    void Update()
    {
        if (doUpdate)
        {
            osc = Mathf.Max(Mathf.Sin(Time.realtimeSinceStartup * speed) * ampitude + offset, 0);
            var sharedmaterials = new List<Material>();
            objectRenderer.GetSharedMaterials(sharedmaterials);
            foreach (var mat in sharedmaterials)
            {
                mat.SetFloat("_EmissionIntensity", osc * maxIntensity);             // set the emissionintensity to make the highlight blink
                mat.SetColor("_EmissionColor", highColor);
            }
        }
    }

    private Renderer findNearest3DObject()
    {
        Renderer rend = null;
        // traverse to the 'nearest' renderer -> important that the scene is built correctly
        if(transform.parent != null)        
            rend = transform.parent.GetComponentInChildren<Renderer>();
        if(rend == null && transform.parent.parent != null)
            rend = transform.parent.parent.GetComponentInChildren<Renderer>();
        return rend;
    }
}
