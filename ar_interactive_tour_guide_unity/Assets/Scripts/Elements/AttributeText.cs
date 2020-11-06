using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using deVoid.Utils;
using UnityEngine.UI;

public enum FileAttributes { Location, Date, Copyright }
[RequireComponent(typeof(Text))]
public class AttributeText : MonoBehaviour
{

    [SerializeField] FileAttributes attribute;
    [SerializeField] Image background;
    Text text;

    void Awake()
    {
        text = GetComponent<Text>();

        if (App.activeFile != null)
            OnShowContent(App.activeFile);

        Signals.Get<ShowContentSignal>().AddListener(OnShowContent);
    }

    void OnShowContent(MyFile content)
    {
        switch (attribute)
        {
            case FileAttributes.Location:
                text.text = content.location;
                break;
            case FileAttributes.Date:
                text.text = content.date;
                break;
            case FileAttributes.Copyright:
                text.text = content.copyright;
                break;
        }

        if (background != null)
        {
            if (string.IsNullOrWhiteSpace(text.text))
                background.canvasRenderer.SetAlpha(0);
            else background.canvasRenderer.SetAlpha(1);
        }
        // TODO low prio: languages
    }
}