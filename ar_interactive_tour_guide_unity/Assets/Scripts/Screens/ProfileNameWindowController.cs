using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using deVoid.UIFramework;
using System;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Events;



public class ProfileNameWindowController : AWindowController<ProfileNameWindowControllerProperties>
{

    [SerializeField] private InputField nameInputField;

    public void ChangeProfileName(string newName) {
        Properties.tour.profileName = newName;
    }

    public void SaveAndClose() {
        // TODO put a save tour method in a central location
        var path = Path.Combine(Paths.customToursPath, "tour_" + Properties.tour.uniqueId + ".xml");
        Debug.Log("saving to path " + path);
        XmlOperation.TrySerialize(Properties.tour, path);
        CloseRequest(this);
    }

    protected override void OnPropertiesSet() {
        nameInputField.text = Properties.tour.profileName;
        nameInputField.ActivateInputField();
        nameInputField.Select();
    }
}


[Serializable]
public class ProfileNameWindowControllerProperties : WindowProperties
{
    public readonly Tour tour;

    public ProfileNameWindowControllerProperties(Tour tour) {
        this.tour = tour;
    }
}