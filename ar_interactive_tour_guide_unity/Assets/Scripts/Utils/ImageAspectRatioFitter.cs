using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AspectRatioFitter))]
[ExecuteAlways]
public class ImageAspectRatioFitter : MonoBehaviour
{
    private Image image;
    private RawImage rawImage;
    private AspectRatioFitter aspectRatioFitter;

    void LateUpdate()
    {
        if(image == null && rawImage == null)
        {
            image = GetComponent<Image>();
            rawImage = GetComponent<RawImage>();
        }
        if(aspectRatioFitter == null)
            aspectRatioFitter = GetComponent<AspectRatioFitter>();

        if(aspectRatioFitter == null)
            Debug.Log("ImageAspectRatioFitter needs an aspect ratio fitter");

        if(aspectRatioFitter != null)
        {
            if(image != null)
            {
                if(image.sprite != null)
                    aspectRatioFitter.aspectRatio = image.sprite.rect.width / image.sprite.rect.height;
            }

            else if (rawImage != null)
            {
                if(rawImage.texture != null)
                    aspectRatioFitter.aspectRatio = (float) rawImage.texture.width / (float) rawImage.texture.height;
            }

            else Debug.Log("ImageAspectRatioFitter needs an image or rawImage");
        }
    }
}