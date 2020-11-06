using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


[CustomEditor(typeof(POI))]
public class POIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var changed = DrawDefaultInspector();

        if (changed || GUILayout.Button("Rebuild Screenspace POI"))
        {
            (target as POI).DestroyScreenspacePOI();

            // ScreenspacePOI is rebuild in Update
            // This should trigger an update in the editor anyway, since the sceen changes
            // as a security (eg if there is no ScreenspacePOI to destroy):
            EditorApplication.QueuePlayerLoopUpdate();
        }
    }
}