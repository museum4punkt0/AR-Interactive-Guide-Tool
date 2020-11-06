using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
[ExecuteAlways]
public class ToggleGraphic : MonoBehaviour
{
    public Image onImage;
    public Image offImage;

    [Header("Optional GameObjects")]
    public GameObject[] onObjects;
    public GameObject[] offObjects;

    [Header("Optional lists")]
    public Image[] onImages;
    public Image[] offImages;

    private Toggle toggle;
    void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnValueChanged);
    }

    private void Start()
    {
        OnValueChanged(toggle.isOn);
    }

    void OnValueChanged(bool value)
    {
        var intValue = value ? 1 : 0;

        if (onImage != null)
            onImage.canvasRenderer.SetAlpha(intValue);
        if (offImage != null)
            offImage.canvasRenderer.SetAlpha(1 - intValue);

        foreach(var o in onObjects)
            o.SetActive(value);
        foreach(var o in offObjects)
            o.SetActive(!value);

        foreach (var i in onImages)
            i.canvasRenderer.SetAlpha(intValue);
        foreach(var i in offImages)
            i.canvasRenderer.SetAlpha(1 - intValue);
    }
}
