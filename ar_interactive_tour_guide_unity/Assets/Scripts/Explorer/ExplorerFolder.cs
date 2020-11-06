using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ExplorerFolder : AExplorerItem
{
    [SerializeField] private Text indexText;
    [SerializeField] private Image indexBackground;

    private Folder _folder;
    public Folder folder {
        set 
        {
            if(indexBackground != null)
                indexBackground.color = value.color;
            fileOrFolder = value;
            _folder = value;
            SetFromFileOrFolder(value);
        }
        get { return _folder; }
    }

    public void SetIndexText(string text)
    {
        indexText.text = text;
    }
}