using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.IO;

public class ExplorerFile : AExplorerItem
{
    [SerializeField] private RawImage thumbnail;
    private MyFile _file;
    public MyFile file
    {
        set
        {
            fileOrFolder = value;
            _file = value;

            ApplyThumbnail(value, thumbnail);
           
            SetFromFileOrFolder(value);
        }
        get { return _file; }
    }

    public void ApplyThumbnail(MyFile f, RawImage thumbnail)
    {
        if (!App.cache.HasCached(f.url))
            return;

        var thumbURL = App.cache.GetCachedThumbnailPath(f.url);

        if (!File.Exists(thumbURL))
            return;
        
        if(thumbnail.texture != null)
            Destroy(thumbnail.texture);

        Texture2D texture = new Texture2D(5, 2, TextureFormat.RGB24, false);
        texture.hideFlags = HideFlags.HideAndDontSave;

        texture.filterMode = FilterMode.Bilinear;
        byte[] bytes = File.ReadAllBytes(thumbURL);
        texture.LoadImage(bytes);
        
        thumbnail.texture = texture;
    }

    // as OnDestroy will be called only if Awake has been called (not always the case for us!)
    // you must call this when destroying an explorer file!
    public void DoOnDestroy()
    {
        OnDestroy();
    }

    private void OnDestroy()
    {
        if(thumbnail.texture != null)
        {
            Destroy(thumbnail.texture);
        }
    }
}

public abstract class AExplorerItem : MonoBehaviour, IPointerClickHandler //, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    // TODO consistency for events, should they be C# event Action, event EventHandler<> or Unity Events
    public event Action onPointerClick;
    public event Action onMoveUp;
    public event Action onMoveDown;

    public FileOrFolder fileOrFolder; // TODO improve. low prio. to be set by ExplorerFile and ExplorerFolder.

    [SerializeField] private LocalisedText titleText;

    [SerializeField] private Image background;
    [SerializeField] private GameObject dimmer;

    // in case there's a gameobject that should be shown when the explorerfile is hidden, such as another dimmer
    // or a grey-out filter
    // [SerializeField] private GameObject showOnlyWhenHidden;

    [Serializable]
    public class InitializeToggleEvent : UnityEvent<bool> { }
    [SerializeField] private InitializeToggleEvent initializeHiddenToggle = new InitializeToggleEvent();

    private Color selectedColor;
    private Color backgroundColor;
    private bool cachedBackgroundColor = false; // Cannot rely on Awake since it's not reliably called at Instantiate(prefab)

    // To be called by the Unity EventSystem
    // TODO make this be called by a button in the prefab instead for consistency
    public void OnPointerClick(PointerEventData eventData)
    {
        onPointerClick?.Invoke();
    }

    // To be called by a button in the prefab
    public void MoveUp()
    {
        onMoveUp?.Invoke();
    }

    public void MoveDown()
    {
        onMoveDown?.Invoke();
    }

    public void SetHidden(bool hidden)
    {
        // SetHidden is called by a toggle
        // a toggle sends OnValueChanged in it's Start
        // hence sometimes there is no fileOrFolder assigned here
        if(fileOrFolder != null)
            fileOrFolder.hidden = hidden;

        // if(showOnlyWhenHidden != null)
        //     showOnlyWhenHidden.SetActive(hidden);
    }

    public void Dim(bool dim)
    {
        if(dimmer != null)
            dimmer.SetActive(dim);
    }

    // TODO instead of setting the backgroundcolor
    // use a second Image instead to set the selected color to
    // and activate / deactivate that
    public void Select(bool select)
    {
        if(!cachedBackgroundColor)
        {
            backgroundColor = background.color;
            cachedBackgroundColor = true;
        }

        if (select)
            background.color = selectedColor;
        else
            background.color = backgroundColor;
    }

    protected void SetFromFileOrFolder(FileOrFolder f)
    {
        if(titleText != null)
        {
            titleText.SetText(Language.EN, f.title_en);
            titleText.SetText(Language.DE, f.title_de);
        }
        
        selectedColor = f.color;
        SetHidden(f.hidden);
        initializeHiddenToggle.Invoke(f.hidden);
    }
}