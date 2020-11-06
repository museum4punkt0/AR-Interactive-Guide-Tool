using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using deVoid.Utils;


// TODO code design: unite this w FreeExplorationPopup, FreeExplorationListener
// makes no sense to have all these
public class GuidedExplorationInfo : MonoBehaviour
{
    public CanvasGroup info;  
    [SerializeField] float stayDuration = 1;
    [SerializeField] float fadeDuration = 0.3f;

    bool freeExploration;
    
    Coroutine fade;

    private void Awake()
    {
        Signals.Get<FreeExplorationSignal>().AddListener(OnFreeExploration);
    }

    private void OnEnable()
    {
        info.gameObject.SetActive(/*!isServer &&*/ !freeExploration);
        info.alpha = 0;

    }

    private void OnDestroy()
    {
        Signals.Get<FreeExplorationSignal>().RemoveListener(OnFreeExploration);
    }

    void OnFreeExploration(bool free)
    {
        freeExploration = free;
        info.gameObject.SetActive(/*!isServer &&*/ !freeExploration);
    }

    // void OnFreeExploration(bool setFree)
    // {
    //     if(isServer)
    //         RpcOnFreeExploration(setFree);
    // }

    // [ClientRpc]
    // public void RpcOnFreeExploration(bool setFree)
    // {
    //     if(!isServer)
    //         info.gameObject.SetActive(!setFree);
    // }

    public void ShowImage()
    {
        info.alpha = 1;
        if (fade != null)
            StopCoroutine(fade);
        fade = StartCoroutine(new WaitForSeconds(stayDuration).Do().
                              Then(FadeOut(info, fadeDuration)));
    }

    IEnumerator FadeOut(CanvasGroup i, float duration)
    {
        var startAlpha = i.alpha;
        var start = Time.time;

        while(start + duration > Time.time)
        {
            var lerpParam = (Time.time - start) / duration;
            i.alpha = Mathf.Lerp(startAlpha, 0, lerpParam);
            yield return null;
        }
        i.alpha = 0;
    }
}