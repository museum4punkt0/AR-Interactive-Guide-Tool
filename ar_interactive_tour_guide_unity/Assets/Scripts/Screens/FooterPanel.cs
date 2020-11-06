using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using deVoid.Utils;

// TODO this should not be called FooterPanel
// it should just be called PrevNextController or similarly
// as it is also eg in the End AR button, which also goes to the next content item

// TODO improve design - standardise procedure of static fields assigned by signals
// ie why does this both have an activeFile field and find App.activeFile ?
public class FooterPanel : MonoBehaviour
{
    private MyFile activeFile;

    private void Awake()
    {
        if (App.activeFile != null)
            OnShowContent(App.activeFile);

        Signals.Get<ShowContentSignal>().AddListener(OnShowContent);
    }

    void OnShowContent(MyFile file)
    {
        activeFile = file;
    }

    private void OnDestroy()
    {
        Signals.Get<ShowContentSignal>().RemoveListener(OnShowContent);
    }

    public void Next()
    {
        if (activeFile == null)
            activeFile = App.activeFile;

        if (activeFile == null)
            return;

        var next = activeFile.NextContentItem(false);

        if (next != null)
            App.guideSync.ShowContent(next);
    }

    public void Prev()
    {
        if (activeFile == null)
            activeFile = App.activeFile;

        if (activeFile == null)
            return;

        var prev = activeFile.PrevContentItem(false);

        if (prev != null)
            App.guideSync.ShowContent(prev);
    }

}
