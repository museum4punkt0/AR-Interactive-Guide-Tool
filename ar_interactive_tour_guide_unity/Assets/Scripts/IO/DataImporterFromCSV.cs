using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.Networking;

public class DataImporterFromCSV {

    public Folder doImportResult;
    public bool errorOnWebRequest;

    public IEnumerator DoImport(string folderUrl) {

        doImportResult = null;
        errorOnWebRequest = false;

        Folder root = new Folder();
        root.nodeName = "root";

        // Load themes
        string dataPathThemes =     folderUrl + "themes.csv"; 
        string dataPathChapters =   folderUrl + "chapters.csv"; 
        string dataPathItems =      folderUrl + "items.csv"; 

        // The order here is crucial!
        // chapters are children of themes, hence themes MUST be added first!
        yield return AddDataFrom(dataPathThemes, root);
        yield return AddDataFrom(dataPathChapters, root);
        yield return AddDataFrom(dataPathItems, root);

        doImportResult = root;
    }

    IEnumerator AddDataFrom(string csv, Folder root)
    {
        yield return LoadData(csv);
        List<List<string>> strings = loadDataResult;

        // Data structure
        // 0       1         2       3        4              5     6    7               8           9       10          11          12
        // type    parent    name    color    content        DE    EN   thumbnailurl    location    date    copyright   Text DE     Text EN
        foreach(List<string> data in strings)
        {
            FileOrFolder newF = null;
            switch (data[0])
            {
                case "folder":
                    var newFolder = new Folder();
                    newF = newFolder;
                    break;
                case "image":
                    var newImage = new ImageFile
                    {
                        url = data.ElementAtOrDefault(4, ""),
                        thumbnailURL = data.ElementAtOrDefault(7, ""),
                        location = data.ElementAtOrDefault(8, ""),
                        date = data.ElementAtOrDefault(9, ""),
                        copyright = data.ElementAtOrDefault(10, "")
                    };
                    newF = newImage;
                    break;
                case "video":
                    var newVideo = new VideoFile
                    {
                        url = data.ElementAtOrDefault(4, ""),
                        thumbnailURL = data.ElementAtOrDefault(7, ""),
                        location = data.ElementAtOrDefault(8, ""),
                        date = data.ElementAtOrDefault(9, ""),
                        copyright = data.ElementAtOrDefault(10, "")
                    };
                    newF = newVideo;
                    break;
                case "ar":
                    var newAR = new ARFile
                    {
                        uniqueName = data.ElementAtOrDefault(4, "")
                    };
                    newF = newAR;
                    break;
                case "text":
                    var newText = new TextFile
                    {
                        textDE = data.ElementAtOrDefault(11, ""),
                        textEN = data.ElementAtOrDefault(12, "")
                    };
                    newF = newText;
                    break;
                case "game":
                    newF = new GameFile();
                    break;
                default:
                    break;
            }
            // TODO make parser more generic
            
            if(newF != null)
            {
                // these datapoints apply to all cases:
                var parent = root.GetAllFilesAndFolders(true).Find(f => f.nodeName == data.ElementAtOrDefault(1, "")) as Folder;
                parent?.AddChild(newF);

                Color defaultColor;
                if (parent != null)
                    defaultColor = parent.color;
                else defaultColor = Color.gray;

                newF.nodeName = data.ElementAtOrDefault(2, "");
                newF.color = ColorFromString(data.ElementAtOrDefault(3, ""), defaultColor);
                newF.title_de = data.ElementAtOrDefault(5, "");
                newF.title_en = data.ElementAtOrDefault(6, "");
            }
        }
    }

    Color ColorFromString(string color_str, Color defaultColor)
    {
        Color color;
        if (!ColorUtility.TryParseHtmlString(color_str, out color)) {
            color = defaultColor;

            // empty string is the usual case for files
            if(!string.IsNullOrWhiteSpace(color_str))
            {
                Debug.LogWarning("Couldn't parse color string: " + color_str);
            }
        }
        return color;
    }

    // TODO unify this with the loading in Loading Window Controller!
    List<List<string>> loadDataResult;
    IEnumerator LoadData(string url)
    {
        loadDataResult = new List<List<string>>();

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.timeout = 5;
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log(req.error);
                errorOnWebRequest = true;
            }

            else
            {
                foreach (var line in CsvParser2.Parse(req.downloadHandler.text))
                    loadDataResult.Add(line.ToList());
            }
        }
        yield return null;
    }
}