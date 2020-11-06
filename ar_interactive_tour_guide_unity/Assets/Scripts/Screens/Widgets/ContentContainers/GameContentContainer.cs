using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameContentContainer : MonoBehaviour
{
    public void ShowContent()
    {
        foreach(var o in transform.Children())
        {
            o.SetActive(true);
        }
    }
}
