using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NameProvider), true)]
class NameProviderUI : Editor
{
	private NameProvider nameProvider;
	private bool rawInput = true;
	private string rawData;
	private void OnEnable()
	{
		nameProvider = target as NameProvider;
		rawData = string.Join("\n", nameProvider.names ?? (new string[0]));
	}

	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();
		if(rawInput = GUILayout.Toggle(rawInput, "Raw Input"))
		{
			rawData = EditorGUILayout.TextArea(rawData);
		}else
		{
			DrawDefaultInspector();
		}
		if(EditorGUI.EndChangeCheck())
		{
			nameProvider.names = rawData.Replace(",", "\n").Split('\n');
			for (int i = 0; i < nameProvider.names.Length; i++)
			{
				var name = nameProvider.names[i];
				if (name[0] == ' ')
					nameProvider.names[i] = name.Remove(0, 1);
			}
			rawData = string.Join("\n", nameProvider.names ?? (new string[0]));
		}
	}
}
