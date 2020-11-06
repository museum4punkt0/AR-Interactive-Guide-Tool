using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideProfileListEntry : MonoBehaviour
{
    public Button startWithProfileButton;
    public Button editProfileButton;
    public Button deleteProfileButton;

    public void SetName(string profileName)
    {
        startWithProfileButton.GetComponentInChildren<Text>().text = profileName;
    }
}