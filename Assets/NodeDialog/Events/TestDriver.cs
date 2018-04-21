using System;
using System.Collections;
using System.Collections.Generic;
using NodeDialog.Events;
using UnityEngine;
using Object = UnityEngine.Object;

public class TestDriver : MonoBehaviour
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
}

namespace UnityEditor
{
	[CustomPropertyDrawer(typeof(NodeEvent))]
	public class NodeEventDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			NodeEvent nodeEvent = fieldInfo.GetValue(property.serializedObject.targetObject) as NodeEvent;
			if (nodeEvent != null)
			{
				NodeEvent.NodeEventBase inner = nodeEvent.innerEvent;

				if (inner == null)
					DrawUninitializedEvent(position, nodeEvent, property, label);
				if (inner is NodeEvent.NodeEventComponentNotStaticResolved)
					DrawReferenceComponentEvent(position, nodeEvent, property, label);
				if (inner is NodeEvent.NodeEventComponent)
					DrawComponentEvent(position, nodeEvent, property, label);
				if (inner is NodeEvent.NodeEventStatic)
					DrawStaticComponentEvent(position, nodeEvent, property, label);
			}
			else
				throw new NullReferenceException();
		}

		private void DrawUninitializedEvent(Rect position, NodeEvent nodeEvent, SerializedProperty property, GUIContent label)
		{
			Object haha = EditorGUI.ObjectField(position, null, typeof(Object), true);
			if (haha != null)
			{
				if (haha is MonoScript)
					nodeEvent.InitializeStatic(haha.GetType(), "");
				if (haha is Component)
					nodeEvent.InitializeComponent(haha as Component, "");
			}
		}

		private void DrawComponentEvent(Rect position, NodeEvent nodeEvent, SerializedProperty property, GUIContent label)
		{
			throw new System.NotImplementedException();
		}

		private void DrawReferenceComponentEvent(Rect position, NodeEvent nodeEvent, SerializedProperty property, GUIContent label)
		{
			throw new System.NotImplementedException();
		}

		private void DrawStaticComponentEvent(Rect position, NodeEvent nodeEvent, SerializedProperty property, GUIContent label)
		{
			throw new System.NotImplementedException();
		}
	}
}
