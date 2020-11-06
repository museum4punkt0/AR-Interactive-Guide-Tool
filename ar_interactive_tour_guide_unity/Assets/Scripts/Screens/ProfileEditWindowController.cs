using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using deVoid.UIFramework;
using System;
using System.IO;
using UnityEngine.UI;

public class ProfileEditWindowController : AWindowController<ProfileEditWindowControllerProperties>
{
    [SerializeField] private TourExplorer explorer;
    public void SaveProfile()
    {
        var path = Path.Combine(Paths.customToursPath, "tour_" + Properties.tour.uniqueId + ".xml");
        Debug.Log("saving to path " + path);
        XmlOperation.TrySerialize(Properties.tour, path);
        CloseRequest(this);
    }

    protected override void OnPropertiesSet()
    {
        explorer.Initialize(Properties.tour, true);
    }
}


[Serializable]
public class ProfileEditWindowControllerProperties : WindowProperties
{
    public readonly Tour tour;

    public ProfileEditWindowControllerProperties(Tour tour)
    {
        this.tour = tour;
    }
}