using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using deVoid.UIFramework;
using deVoid.Utils;

// TODO minor: make the ExplorerWindow an acutal window
// -> make sure the content window does not reload upon closing the explorer

public class ShowExplorerWindowSignal : ASignal<bool> { }
public class HeaderPanel { 

    [SerializeField] private LocalisedText title;
}