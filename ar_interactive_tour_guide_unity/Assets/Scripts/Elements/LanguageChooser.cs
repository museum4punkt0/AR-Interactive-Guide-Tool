using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageChooser : MonoBehaviour
{
    private Dropdown _dd;
    private Dropdown dd { get {
            if (_dd != null) return _dd;
            else 
                return GetComponent<Dropdown>(); } }
    public void SetLanguage(int index)
    {
        Language l;
        if (index == 0)
            l = Language.DE;
        else
            l = Language.EN;

        SetLanguage(l);
    }
    public void Start()
    {
        InitiateWithActiveLanguage(App.guideSync.activeLanguage);
    }
    private void OnEnable()
    {
        InitiateWithActiveLanguage(App.guideSync.activeLanguage);
    }
    void InitiateWithActiveLanguage(Language l)
    {
        if(l == Language.DE)
            dd.SetValueWithoutNotify(0);
        if (l == Language.EN)
            dd.SetValueWithoutNotify(1);
    }
    public void SetLanguage(Language language)
    {   
        App.guideSync.SetActiveLanguage(language);
    }
}
