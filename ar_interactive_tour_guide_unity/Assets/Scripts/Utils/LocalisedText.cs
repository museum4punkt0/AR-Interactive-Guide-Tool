
﻿using UnityEngine;
using System;

using UnityEngine.UI;
using System.Collections.Generic;
using deVoid.Utils;
using UnityEngine.Assertions;
using TMPro;

public enum Language { DE, EN }

[ExecuteAlways]
public class LocalisedText : MonoBehaviour
{
    // default values to give in editor
    [TextArea]
    public string german;
    [TextArea]
    public string english;

    private Text text;
    private TextMesh textMesh;
    private TextMeshPro textMeshPro;

    private Dictionary<Language, string> labels = new Dictionary<Language, string>();

    public static Language activeLanguage;

    protected void Awake() {
        text = GetComponent<Text>();
        textMesh = GetComponent<TextMesh>();
        textMeshPro = GetComponent<TextMeshPro>();

        Assert.IsTrue(text != null || textMesh != null || textMeshPro != null, "LocalisedText must have either a Text or a TextMesh component!");

        if(! labels.ContainsKey(Language.DE))
            SetText(Language.DE, german);

        if(! labels.ContainsKey(Language.EN))
            SetText(Language.EN, english);

        Signals.Get<SetLanguageSignal>().AddListener(OnSetLanguage);
    }

    protected void Start()
    {
        OnSetLanguage(activeLanguage);
    }

    public void SetText(Language language, string text) {
        labels[language] = text;

        if (language == activeLanguage)
            OnSetLanguage(language);
    }

    public void SetTexts(Dictionary<Language, string> texts)
    {
        foreach(var pair in texts)
        {
            SetText(pair.Key, pair.Value);
        }
    }

    protected void OnSetLanguage (Language language) {

        if(!labels.ContainsKey(language)) {
            Debug.Log("Language text not set!");
            return;
        }

        // this is currently static and set before any of the Signals, in case there is no localisedText instance active
        // TODO find more elegant solution
        // activeLanguage = language; 

        if (text != null)
            text.text = labels[language];
        if (textMesh != null)
            textMesh.text = labels[language];
        if (textMeshPro != null)
            textMeshPro.text = labels[language];
    }
}