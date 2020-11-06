using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Linq;
using System;
using deVoid.Utils;

// This is not a general purpose file explorer for the FileOrFolder structure. 
// It navigates specifically tours, which have a depth of two!

public class TourExplorer : MonoBehaviour
{
    public event Action<MyFile> onFileClicked;

    [SerializeField] private ExplorerFile filePrefabImage;
    [SerializeField] private ExplorerFile filePrefabVideo;
    [SerializeField] private ExplorerFile filePrefabAR;
    [SerializeField] private ExplorerFile filePrefabText;
    [SerializeField] private ExplorerFile filePrefabGame;

    [SerializeField] private ExplorerFolder themePrefab;
    [SerializeField] private ExplorerFolder chapterPrefab;

    [SerializeField] private GameObject themesContainer;
    [SerializeField] private GameObject chaptersContainer;
    [SerializeField] private GameObject filesContainer;

    [SerializeField] private Animator animator;

    private Tour tour;
    private Folder openTheme;
    private bool showHidden;
    // private Folder openChapter;
    private MyFile selectedFile;

    public void Initialize(Tour tour, bool showHidden)
    {
        Signals.Get<ShowContentSignal>().AddListener(SelectFile);

        this.tour = tour;
        this.showHidden = showHidden;
        Reset();
    }

    private void Reset()
    {
        ClearAll();
        PopulateThemes(tour.root.children.OfType<Folder>());
    }

    void ClearAll()
    {
        ClearThemes();
        ClearChapters();
        ClearFiles();
    }

    IEnumerable<ExplorerFolder> GetThemesInContainer() { return themesContainer.GetComponentsInChildren<ExplorerFolder>(); }
    IEnumerable<ExplorerFolder> GetChaptersInContainer() { return chaptersContainer.GetComponentsInChildren<ExplorerFolder>(); }
    IEnumerable<ExplorerFile> GetFilesInContainer() { return filesContainer.GetComponentsInChildren<ExplorerFile>(); }

    void ClearThemes()
    {
        foreach (var folder in GetThemesInContainer())
        {
            folder.transform.SetParent(this.transform); // hacky, to remove it from the ThemesContainer immediately
            Destroy(folder.gameObject);
        }
    }
    void ClearChapters()
    {
        foreach (var folder in GetChaptersInContainer())
        {
            folder.transform.SetParent(this.transform);
            Destroy(folder.gameObject);
        }
    }

    void ClearFiles()
    {
        foreach (var file in GetFilesInContainer())
        {
            file.transform.SetParent(this.transform);
            file.DoOnDestroy();
            Destroy(file.gameObject);
        }
    }

    int SelectItemInList(FileOrFolder item, IEnumerable<AExplorerItem> list)
    {
        var foundIndex = -1;
        var index = 0;

        var dimItem = true;
        foreach (var i in list)
        {
            i.Select(i.fileOrFolder == item);

            if (i.fileOrFolder == item)
            {
                foundIndex = index;
                dimItem = false;
            }

            i.Dim(dimItem);
            index++;
        }

        return foundIndex;
    }

    // TODO go over the whole OpenTheme / OpenChapter logic
    // make it less repetitive & cleaner
    void OpenTheme(Folder themeFolder, bool animateIn = true)
    {
        var index = SelectItemInList(themeFolder, GetThemesInContainer());
        //index = index + 1; // 0 indexed to 1 indexed

        ClearFiles();
        ClearChapters();
        var chapters = themeFolder.children.OfType<Folder>();
        PopulateChapters(chapters, index + ".");

        if (chapters.Any())
            OpenChapter(chapters.First());

        openTheme = themeFolder;

        if(animator != null && animateIn)
        {
            animator.ResetTrigger("To Themes");
            animator.SetTrigger("To Chapters");
        }
    }

    void OpenChapter(Folder chapterFolder, bool animateIn = true)
    {
        // OpenTheme(chapterFolder.parent, false);

        var themeFolder = chapterFolder.parent;
        var index = SelectItemInList(themeFolder, GetThemesInContainer());
        //index = index + 1; // 0 indexed to 1 indexed

        ClearFiles();
        ClearChapters();
        PopulateChapters(themeFolder.children.OfType<Folder>(), index + ".");

        openTheme = themeFolder;


        SelectItemInList(chapterFolder, GetChaptersInContainer());

        PopulateFiles(chapterFolder.children.OfType<MyFile>());

        //openChapter = chapterFolder;
    }

    void OpenFile(MyFile file)
    {
        SelectFile(file);

        // openFile = file; 
        onFileClicked?.Invoke(file);
    }

    // select Theme, Chapter not needed so far (but would be analogous)
    public void SelectFile(MyFile file)
    {
        selectedFile = file;
        OpenChapter(file.parent, false);
        SelectItemInList(file, GetFilesInContainer());
    }

    public void Up()
    {
        if (animator != null)
        {
            animator.ResetTrigger("To Chapters");
            animator.SetTrigger("To Themes");
        }
    }

    public void EndTour()
    {
        // TODO low prio tell clients that tour has ended
        App.uiFrame.CloseCurrentWindow();
    }

    void PopulateThemes(IEnumerable<Folder> themeFolders)
    {
        var i = 0;
        foreach (var themeFolder in themeFolders)
        {
            if (!showHidden && themeFolder.hidden)
                continue;

            var newTheme = Instantiate(themePrefab, themesContainer.transform);
            newTheme.folder = themeFolder;
            newTheme.SetIndexText(i++.ToString());
            newTheme.onPointerClick += () => OpenTheme(newTheme.folder);
            newTheme.onMoveUp += () =>
            {
                MoveFolder(newTheme.folder, -1);
                OpenTheme(newTheme.folder);
            };
            newTheme.onMoveDown += () =>
            {
                MoveFolder(newTheme.folder, 1);
                OpenTheme(newTheme.folder);
            };
        }
    }

    void PopulateChapters(IEnumerable<Folder> chapterFolders, string indexTextPrefix)
    {
        var i = 1;
        foreach(var chapterFolder in chapterFolders)
        {
            if (!showHidden && chapterFolder.hidden)
                continue;

            var newChapter = Instantiate(chapterPrefab, chaptersContainer.transform);
            newChapter.folder = chapterFolder;
            newChapter.SetIndexText(indexTextPrefix + i++);
            newChapter.onPointerClick += () => OpenChapter(newChapter.folder);
            newChapter.onMoveUp += () =>
            {
                MoveFolder(newChapter.folder, -1);
                OpenChapter(newChapter.folder);
            };
            newChapter.onMoveDown += () =>
            {
                MoveFolder(newChapter.folder, 1);
                OpenChapter(newChapter.folder);
            };
        }
    }

    void PopulateFiles(IEnumerable<MyFile> files)
    {
        foreach (var file in files)
        {
            if (!showHidden && file.hidden)
                continue;


            ExplorerFile newFile = null;

            switch (file)
            {
                case ImageFile f:
                    newFile = Instantiate(filePrefabImage, filesContainer.transform);
                    break;
                case VideoFile f:
                    newFile = Instantiate(filePrefabVideo, filesContainer.transform);
                    break;
                case ARFile f:
                    newFile = Instantiate(filePrefabAR, filesContainer.transform);
                    break;
                case TextFile f:
                    newFile = Instantiate(filePrefabText, filesContainer.transform);
                    break;
                case GameFile f:
                    newFile = Instantiate(filePrefabGame, filesContainer.transform);
                    break;
            }

            newFile.file = file;
            newFile.onPointerClick += () => OpenFile(newFile.file);
        }

        if(selectedFile != null)
            SelectItemInList(selectedFile, GetFilesInContainer());
    }

    void MoveFolder(Folder folder, int amount)
    {
        folder.Move(amount);

        Reset();

        if (openTheme != null)
            OpenTheme(openTheme);
    }

    void OnDestroy()
    {
        Signals.Get<ShowContentSignal>().RemoveListener(SelectFile);
    }
}