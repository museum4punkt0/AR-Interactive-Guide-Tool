using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextContentContainer : MonoBehaviour
{
    // for editor
    [SerializeField] private LocalisedText text;

    public void ShowContent(TextFile file)
    {
        text.SetText(Language.DE, file.textDE);
        text.SetText(Language.EN, file.textEN);
    }
}
