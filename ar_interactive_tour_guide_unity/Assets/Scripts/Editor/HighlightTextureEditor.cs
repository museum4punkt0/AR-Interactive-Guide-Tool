using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


[CustomEditor(typeof(HighlightTexture))]
public class HighlightTextureEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var clicked = GUILayout.Button("Preview Highlight Texture");
        var clickedReset = GUILayout.Button("Reset Highlight Texture");
        var changed = DrawDefaultInspector();

        if (changed || clicked)
        {
            (target as HighlightTexture).PreviewTexture();  // set texture if a new texture is assigned in editor

            EditorApplication.QueuePlayerLoopUpdate();
        }
        if (changed || clickedReset)
        {
            (target as HighlightTexture).resetTexture();

            EditorApplication.QueuePlayerLoopUpdate();
        }
    }
}