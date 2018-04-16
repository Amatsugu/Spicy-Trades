using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomPropertyDrawer(typeof(SettlementEvent))]
public class EventDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty(position, label, property);

		// Draw label
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Calculate rects
		var goldRect = new Rect(position.x, position.y, 30, position.height);
		var silverRect = new Rect(position.x, position.y + position.height, 50, position.height);
		var chipRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);
		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		EditorGUI.PropertyField(goldRect, property.FindPropertyRelative("name"), new GUIContent("Name"));
		EditorGUI.PropertyField(goldRect, property.FindPropertyRelative("description"), new GUIContent("Name"));
		// Set indent back to what it was
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}
