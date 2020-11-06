using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Xml.Serialization;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

// Simple tree implementation for a general File/Folder system
// TODO add guaranteed safety of children/parent relationship
// TODO tests

[Serializable()]
public abstract class FileOrFolder
{
    // The import from csv system relies on unique item names!
    public string nodeName;
    public string title_de;
    public string title_en; // TODO low prio generic localisation
    public string location;
    public string date;
    public string copyright;
    public Color color;

    [XmlIgnore]
    [NonSerialized]
    public Folder parent;

    public bool hidden = false;
    public abstract List<FileOrFolder> GetAllFilesAndFolders(bool includeHidden);
}

[Serializable]
public class Folder : FileOrFolder
{
    [XmlArray("children")]
    [XmlArrayItem("ImageFile", typeof(ImageFile))]
    [XmlArrayItem("VideoFile", typeof(VideoFile))]
    [XmlArrayItem("ARFile", typeof(ARFile))]
    [XmlArrayItem("TextFile", typeof(TextFile))]
    [XmlArrayItem("GameFile", typeof(GameFile))]
    [XmlArrayItem("Folder", typeof(Folder))]
    public List<FileOrFolder> children = new List<FileOrFolder>();

    public override List<FileOrFolder> GetAllFilesAndFolders(bool includeHidden) 
    {
        if (hidden && !includeHidden)
            return new List<FileOrFolder>();

        var list = new List<FileOrFolder>(){this};
        list.AddRange(children.SelectMany(c => c.GetAllFilesAndFolders(includeHidden)));
        return list;
    }
    
    public List<MyFile> GetAllDescendantFiles(bool includeHidden)
    {
        return GetAllFilesAndFolders(includeHidden).OfType<MyFile>().ToList();
    }

    public void AddChild(FileOrFolder child) 
    {
        children.Add(child);
        child.parent = this;
    }

    // HACK TODO implement XmlSerializable instead to deserialize properly in the first place
    public void RepairParentsAfterDeserialization()
    {
        foreach(var child in children)
        {
            child.parent = this;
            (child as Folder)?.RepairParentsAfterDeserialization();
        }
    }
}

// TODO find better name
public abstract class MyFile : FileOrFolder {
    public string url = "";
    public string thumbnailURL = "";
    public override List<FileOrFolder> GetAllFilesAndFolders(bool includeHidden) {
        if (hidden && !includeHidden)
            return new List<FileOrFolder>();

        return new List<FileOrFolder>(){this}; 
    }
}

[Serializable]
public class ImageFile : MyFile
{
}

[Serializable]
public class VideoFile : MyFile
{
}

[Serializable]
public class ARFile : MyFile
{
    public string uniqueName;
}

[Serializable]
public class TextFile : MyFile
{
    public string textDE;
    public string textEN;
}

[Serializable]
public class GameFile : MyFile
{
}