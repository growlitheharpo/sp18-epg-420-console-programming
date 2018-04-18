using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NodeDialog.Events;
using UnityEditor;
using UnityEngine;

public class CustomEventDriver : MonoBehaviour
{
	public NodeEvent mEvent;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void DoSomething()
	{
		Debug.Log("We're doing something!");

	}

	public void DoSomethingElse()
	{
		Debug.Log("We're doing something else!");
	}
}

[CustomEditor(typeof(CustomEventDriver))]
public class DriverInspector : Editor
{
	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI();
		serializedObject.Update();

		DrawDrawerForEvent();

		if (GUILayout.Button("Test"))
		{
			((CustomEventDriver)target).mEvent.Invoke(null);
		}

		serializedObject.ApplyModifiedProperties();
	}

	private void DrawDrawerForEvent()
	{
		SerializedProperty prop = serializedObject.FindProperty("mEvent");

		SerializedProperty objectProp = prop.FindPropertyRelative("resolvedObject"); // UnityEngine.Object
		SerializedProperty scriptType = prop.FindPropertyRelative("mScriptType"); // string
		SerializedProperty methodName = prop.FindPropertyRelative("mMethodName"); // string

		EditorGUILayout.PropertyField(objectProp, new GUIContent("Target"), true);

		if (objectProp.objectReferenceValue == null)
			return;

		if (objectProp.objectReferenceValue is GameObject)
		{
			// we need to get all components, then display all valid methods on them.
		}
		else if (objectProp.objectReferenceValue is Component)
		{
			// we need to display all of its functions
			Type componentType = objectProp.objectReferenceValue.GetType();
			scriptType.stringValue = componentType.Name;

			var methods = componentType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly).ToList();
			var methodNames = methods.Select(x => x.Name).ToList();
			int currentChoice = methodNames.IndexOf(methodName.stringValue);
			if (currentChoice < 0)
				currentChoice = 0;

			int choice = EditorGUILayout.Popup(currentChoice, methodNames.ToArray());
			methodName.stringValue = methodNames[choice];
		}
	}
}
