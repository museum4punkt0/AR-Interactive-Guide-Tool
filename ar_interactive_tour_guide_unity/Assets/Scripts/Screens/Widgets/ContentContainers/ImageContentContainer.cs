using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.IO;
using System;

public class ImageContentContainer : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private AspectRatioFitter aspectRatioFitter;

    private Coroutine loadImage;
    private ImageFile activeImage;

    public void ShowContent(ImageFile file)
    {
        if (activeImage == file)
            return;
        activeImage = file;

        var url = file.url;
        if(App.cache.HasCached(url))
            url = App.cache.GetCachedPath(url);

        rawImage.canvasRenderer.SetAlpha(0);
        
        if(loadImage != null)
            StopCoroutine(loadImage);
        StartCoroutine(Load(url));
    }

    // SOURCE
    // https://stackoverflow.com/questions/51598519/read-load-image-file-from-the-streamingassets-folder
    IEnumerator Load(string url)
    {        
        Texture2D tex = new Texture2D(2, 2);

        using (UnityWebRequest req = UnityWebRequest.Get(new Uri(url))) {

            req.timeout = 10;
            yield return req.SendWebRequest();

            if(req.isNetworkError || req.isHttpError)
                Debug.Log(req.error);
            
            else {
                byte[] data = req.downloadHandler.data;
                tex.LoadImage(data);
                tex.hideFlags = HideFlags.HideAndDontSave;

                Destroy(rawImage.texture);

                rawImage.texture = tex;
                rawImage.CrossFadeAlpha(1, 0.5f, true);
                aspectRatioFitter.aspectRatio = (float)tex.width / (float)tex.height;
            }
        }
    }
}