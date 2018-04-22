using System;
using System.Linq;
using System.Reflection;
using NodeDialog.Events;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NodeDialog.Editor.Events
{
	[CustomPropertyDrawer(typeof(NodeEvent))]
	public class NodeEventDrawer : PropertyDrawer
	{
		#region TextAsset Access

		private SerializedProperty GetTextAssetProp(SerializedProperty rootProperty)
		{
			return rootProperty.FindPropertyRelative("mTextAsset");
		}

		private Object GetTextAssetObject(SerializedProperty rootProperty)
		{
			return GetTextAssetProp(rootProperty).objectReferenceValue;
		}
		
		#endregion

		#region Type Access

		private SerializedProperty GetTargetTypeNameProp(SerializedProperty rootProp)
		{
			return rootProp.FindPropertyRelative("mTypeName");
		}

		private string GetTargetTypeName(SerializedProperty rootProp)
		{
			return GetTargetTypeNameProp(rootProp).stringValue;
		}

		private Type GetTargetType(SerializedProperty rootProp)
		{
			return Type.GetType(GetTargetTypeName(rootProp));
		}

		#endregion

		#region Method Access

		private SerializedProperty GetTargetMethodNameProp(SerializedProperty rootProp)
		{
			return rootProp.FindPropertyRelative("mMethodName");
		}

		private string GetTargetMethodName(SerializedProperty rootProp)
		{
			return GetTargetMethodNameProp(rootProp).stringValue;
		}

		private MethodInfo GetTargetMethod(SerializedProperty rootProp)
		{
			Type type = GetTargetType(rootProp);
			if (type == null)
				return null;

			return type.GetMethod(GetTargetMethodName(rootProp), BindingFlags.Static | BindingFlags.Public);
		}

		#endregion

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (GetTargetType(property) != null)
				return EditorGUIUtility.singleLineHeight * 3.0f;
			return EditorGUIUtility.singleLineHeight * 2.0f;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty textProp = GetTextAssetProp(property);
			SerializedProperty typeProp = GetTargetTypeNameProp(property);
			SerializedProperty methodProp = GetTargetMethodNameProp(property);

			bool drawMethod = false;
			if (GetTargetType(property) != null)
				position.height /= 3.0f;
			else
				position.height /= 2.0f;

			EditorGUI.LabelField(position, label);
			position.y += position.height;

			EditorGUI.indentLevel++;

			Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
			Rect propRect = new Rect(labelRect.x + labelRect.width, position.y, position.width - labelRect.width, position.height);

			EditorGUI.LabelField(labelRect, new GUIContent("Target Script"));
			MonoScript result = EditorGUI.ObjectField(propRect, GetTextAssetObject(property), typeof(MonoScript), false) as MonoScript;
			if (result == null)
			{
				textProp.objectReferenceValue = null;
				typeProp.stringValue = "";
				methodProp.stringValue = "";
			}
			else
			{
				// Only draw the method if the text asset was ALREADY not null.
				if (textProp.objectReferenceValue != null)
					drawMethod = true;

				textProp.objectReferenceValue = result;
				typeProp.stringValue = result.GetClass().AssemblyQualifiedName;
			}

			labelRect.y += position.height;
			propRect.y += position.height;
			if (drawMethod)
				DrawMethodZone(labelRect, propRect, property, methodProp, label);

			EditorGUI.indentLevel--;
		}

		private void DrawMethodZone(Rect labeRect, Rect propRect, SerializedProperty rootProp, SerializedProperty methodProp, GUIContent label)
		{
			// draw a dropdown and save the result into methodProp.stringvalue
			Type t = GetTargetType(rootProp);
			if (t == null)
				return;

			var validMethods = t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
			var methodNames = validMethods.Select(x => x.Name).ToList();
			methodNames.Insert(0, "   ");

			int currentIndex = methodNames.IndexOf(methodProp.stringValue);
			if (currentIndex < 0)
				currentIndex = 0;

			EditorGUI.LabelField(labeRect, "Target Method");
			currentIndex = EditorGUI.Popup(propRect, currentIndex, methodNames.ToArray());

			// >= 1 instead of 0 because of the blank we put at the front
			if (currentIndex >= 1)
				methodProp.stringValue = methodNames[currentIndex];
			else
				methodProp.stringValue = "";
		}
	}
}
