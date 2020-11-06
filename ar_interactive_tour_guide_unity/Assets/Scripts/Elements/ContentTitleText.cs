using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using deVoid.Utils;

[RequireComponent(typeof(LocalisedText))]
public class ContentTitleText : MonoBehaviour
{
    [SerializeField] bool titleFromParent = true;
    LocalisedText title;

    void Awake()
    {
        title = GetComponent<LocalisedText>();

        if (App.activeFile != null)
            OnShowContent(App.activeFile);

        Signals.Get<ShowContentSignal>().AddListener(OnShowContent);
    }

    void OnShowContent(MyFile content)
    {
        if (titleFromParent)
        {
            if (content.parent != null)
            {
                title.SetText(Language.EN, content.parent.title_en);
                title.SetText(Language.DE, content.parent.title_de);
            }
            else Debug.Log("File does not have a parent!");
        }

        else
        {
            title.SetText(Language.EN, content.title_en);
            title.SetText(Language.DE, content.title_de);
        }
        // TODO low prio dynamic languages
    }
}