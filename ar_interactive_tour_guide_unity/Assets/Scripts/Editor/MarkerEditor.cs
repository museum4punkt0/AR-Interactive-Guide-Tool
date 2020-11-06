using UnityEditor;
using UnityEditor.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditor(typeof(Marker))]
public class MarkerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if (GUILayout.Button("Marker Found"))
		{
            (target as Marker).OnMarkerFound();
		}
	}
}