using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(LocalisedText))]
public class LocalisedTextEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Overwrite Text"))
        {
            var localisedText = (target as LocalisedText);

            localisedText.SetText(Language.DE, localisedText.german);
            localisedText.SetText(Language.EN, localisedText.english);

            EditorApplication.QueuePlayerLoopUpdate();
        }
    }
}