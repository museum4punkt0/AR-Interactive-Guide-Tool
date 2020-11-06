using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using System.IO;
using System.Xml.Serialization;

[Serializable]
public class Tour
{
    // Architecture: 
    // a profile is a copy of the default Folder
    // does NOT update with new content!
    // this is mildly inconvenient but otherwise we have to update in sync
    // which is a very hard task and will lead to incredibly annoying cornercases including bugs
    // note to self: dont get talked into trying to sync this
    public Folder root = new Folder();
    public long uniqueId;
    public string profileName;

    public Tour()
    {
        RenewUniqueId();
    }

    public void RenewUniqueId()
    {
        uniqueId = System.DateTime.UtcNow.Ticks;
    }

    public MyFile GetFirstFile(bool includeHidden){
        return root.GetAllDescendantFiles(includeHidden).FirstOrDefault();
    }
}

public static class TourHelpers
{
    static string CustomTourPath(Tour tour) {
        return Paths.customToursPath + "tour_" + tour.uniqueId + ".xml";
    }
    public static Tour NewTour() {
        var fullTour = XmlOperation.TryDeserialize<Tour>(Paths.defaultTourPath)
            ?? new Tour();
        fullTour.RenewUniqueId();
        fullTour.root.RepairParentsAfterDeserialization();
        return fullTour;
    }

    public static void SaveDefaultTour(Tour tour) {
        XmlOperation.TrySerialize(tour, Paths.defaultTourPath);
    }

    public static void SaveTour(Tour tour) {
        XmlOperation.TrySerialize(tour, CustomTourPath(tour));
    }

    public static void DeleteTour(Tour tour) {
        File.Delete(CustomTourPath(tour));
    }

    public static List<Tour> LoadTours() {
        Directory.CreateDirectory(Paths.customToursPath);
        
        return Directory.GetFiles(Paths.customToursPath)
                        .Where(p => Path.GetExtension(p) == ".xml")
                        .Select(p => XmlOperation.TryDeserialize<Tour>(p))
                        .Where(t => t != null)
                        .Select(t => { t.root.RepairParentsAfterDeserialization(); return t; })
                        .ToList();
    }

    public static void DeleteCustomTours() {
        Directory.CreateDirectory(Paths.customToursPath);

        var dir = new DirectoryInfo(Paths.customToursPath);

        foreach(System.IO.FileInfo file in dir.GetFiles()) 
            file.Delete();
    }

    public static List<Folder> GetThemes(this Folder root, bool showHidden) {
        return root.children.Where(f => !f.hidden || showHidden)
                            .OfType<Folder>()
                            .ToList();
    }

    public static List<Folder> GetChapters(this Folder root, bool showHidden) {
        return root.GetThemes(showHidden)
                    .SelectMany(c => c.children)
                    .Where(f => !f.hidden || showHidden)
                    .OfType<Folder>()
                    .ToList();
    }

    public static List<MyFile> GetContentItems(this Folder root, bool showHidden) {
        return root.GetChapters(showHidden)
                    .SelectMany(c => c.children)
                    .Where(f => !f.hidden || showHidden)
                    .OfType<MyFile>()
                    .ToList();
    }

    // TODO write tests
    public static bool Move(this FileOrFolder fileOrFolder, int amount)
    {
        if (fileOrFolder.parent == null) 
            return false;

        var children = fileOrFolder.parent.children;
        var index = children.IndexOf(fileOrFolder);
        var newIndex = index + amount;
        
        if(newIndex < 0 || newIndex >= children.Count)
            return false;

        children.RemoveAt(index);
        children.Insert(index+amount, fileOrFolder);
        return true;
    }

    public static MyFile NextContentItem(this MyFile input, bool showHidden)
    {
        return PrevNextContentItem(input, 1, showHidden);
    }

    public static MyFile PrevContentItem(this MyFile input, bool showHidden)
    {
        return PrevNextContentItem(input, -1, showHidden);
    }

    // TODO write tests
    static MyFile PrevNextContentItem(this MyFile input, int amount, bool showHidden)
    {
        var parent = input.parent;

        while(parent != null)
        {
            var allFiles = parent.GetContentItems(showHidden).ToList(); // inefficient
            var index = allFiles.IndexOf(input) + amount;
            
            if(index >= 0 && index < allFiles.Count)
                return allFiles[index];
            
            parent = parent.parent;
        }

        return null;
    }
}

