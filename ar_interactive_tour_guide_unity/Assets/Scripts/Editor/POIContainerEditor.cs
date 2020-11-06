using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


[CustomEditor(typeof(ScreenspacePOIContainer))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // ObjectBuilderScript myScript = (ObjectBuilderScript)target;

        if (GUILayout.Button("Rebuild POIs"))
        {
            (target as ScreenspacePOIContainer).DestroyScreenspacePOIs();

            // This should trigger an update in the editor anyway, since the sceen changes
            // as a security (eg if there are no ScreenspacePOIs to destroy):
            EditorApplication.QueuePlayerLoopUpdate();
        }
    }
}