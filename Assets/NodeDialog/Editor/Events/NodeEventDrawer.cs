﻿using System;
using System.Collections.Generic;
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
		private const float FOLDOUT_BUFFER = 4.0f;

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

		#region Parameter Access

		private SerializedProperty GetParameterListProp(SerializedProperty rootProperty)
		{
			return rootProperty.FindPropertyRelative("mParameters");
		}

		#endregion

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (!property.isExpanded)
				return EditorGUIUtility.singleLineHeight;

			if (GetTargetType(property) == null)
				return EditorGUIUtility.singleLineHeight * 2.0f + FOLDOUT_BUFFER;

			MethodInfo method = GetTargetMethod(property);
			if (method == null)
				return EditorGUIUtility.singleLineHeight * 3.0f + FOLDOUT_BUFFER;

			return EditorGUIUtility.singleLineHeight * (3.0f + method.GetParameters().Length) + FOLDOUT_BUFFER;
		}

		public override void OnGUI(Rect position, SerializedProperty rootProp, GUIContent label)
		{
			Rect r = new Rect(position) { height = EditorGUIUtility.singleLineHeight };

			if (!DrawLabel(r, rootProp, label))
				return;

			r.y += r.height + FOLDOUT_BUFFER;
			EditorGUI.indentLevel++;

			Rect labelRect = new Rect(r.x, r.y, EditorGUIUtility.labelWidth, r.height);
			Rect propRect = new Rect(labelRect.x + labelRect.width, r.y, r.width - labelRect.width, r.height);

			bool drawMethod = DrawScriptZone(labelRect, propRect, rootProp);

			labelRect.y += r.height;
			propRect.y += r.height;
			if (drawMethod)
			{
				bool drawParams = DrawMethodZone(labelRect, propRect, rootProp);

				if (drawParams)
				{
					labelRect.y += r.height;
					propRect.y += r.height;

					Rect wholeRect = new Rect(r.x, labelRect.y, r.width, position.height - labelRect.y);
					DrawParameterZone(wholeRect, rootProp);
				}
			}

			EditorGUI.indentLevel--;
		}

		private bool DrawLabel(Rect labelRect, SerializedProperty rootProp, GUIContent label)
		{
			string labelVal;
			if (GetTargetType(rootProp) != null)
				labelVal = label.text + ": " + GetTargetType(rootProp).Name + "::" + GetTargetMethodName(rootProp);
			else
				labelVal = label.text;

			rootProp.isExpanded = EditorGUI.Foldout(labelRect, rootProp.isExpanded, labelVal, true);
			return rootProp.isExpanded;
        }

		private bool DrawScriptZone(Rect labelRect, Rect propRect, SerializedProperty rootProperty)
		{
			SerializedProperty textProp = GetTextAssetProp(rootProperty);
			SerializedProperty typeProp = GetTargetTypeNameProp(rootProperty);
			SerializedProperty methodProp = GetTargetMethodNameProp(rootProperty);

			bool drawMethod = false;

			EditorGUI.LabelField(labelRect, new GUIContent("Target Script"));
			MonoScript result = EditorGUI.ObjectField(propRect, GetTextAssetObject(rootProperty), typeof(MonoScript), false) as MonoScript;
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

			return drawMethod;
		}

		private bool DrawMethodZone(Rect labeRect, Rect propRect, SerializedProperty rootProp)
		{
			SerializedProperty methodProp = GetTargetMethodNameProp(rootProp);

			// draw a dropdown and save the result into methodProp.stringvalue
			Type t = GetTargetType(rootProp);
			if (t == null)
				return false;

			IList<MethodInfo> validMethods = t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
			validMethods = ValidateMethods(validMethods);

			var methodNames = validMethods.Select(x => x.Name).ToList();
			methodNames.Insert(0, "   ");

			int currentIndex = methodNames.IndexOf(methodProp.stringValue);
			if (currentIndex < 0)
				currentIndex = 0;

			EditorGUI.LabelField(labeRect, "Target Method");
			currentIndex = EditorGUI.Popup(propRect, currentIndex, methodNames.ToArray());

			if (currentIndex >= 1)
			{
				string newValue = methodNames[currentIndex];
				if (methodProp.stringValue == newValue)
					return validMethods[currentIndex - 1].GetParameters().Length > 0;

				methodProp.stringValue = newValue;
				GenerateParameters(rootProp, validMethods[currentIndex - 1]);
				return validMethods[currentIndex - 1].GetParameters().Length > 0;
			}
			else
			{
				methodProp.stringValue = "";
				ClearParameters(rootProp);
				return false;
			}
		}

		private void DrawParameterZone(Rect wholeRect, SerializedProperty rootProp)
		{
			SerializedProperty listProp = GetParameterListProp(rootProp);
			var parameters = GetTargetMethod(rootProp).GetParameters();

			if (listProp.arraySize != parameters.Length)
				GenerateParameters(rootProp, GetTargetMethod(rootProp));
			
			// properties should be pre-filled with the correct type.

			EditorGUI.indentLevel++;
			Rect r = new Rect(wholeRect) { height = EditorGUIUtility.singleLineHeight };
			Rect labelRect = new Rect(r.x, r.y, EditorGUIUtility.labelWidth, r.height);
			Rect propRect = new Rect(labelRect.x + labelRect.width, r.y, r.width - labelRect.width, r.height);

			for (int i = 0; i < listProp.arraySize; ++i)
			{
				SerializedProperty innerParamProp = listProp.GetArrayElementAtIndex(i);

				NodeEvent.ParameterType paramType = (NodeEvent.ParameterType)innerParamProp.FindPropertyRelative("mType").enumValueIndex;

				string label = string.Format("({0}) {1}", paramType.ToString(), parameters[i].Name);
				EditorGUI.LabelField(labelRect, label);
				switch (paramType)
				{
					case NodeEvent.ParameterType.String:
						SerializedProperty strVal = innerParamProp.FindPropertyRelative("mStringValue");
						strVal.stringValue = EditorGUI.TextField(propRect, strVal.stringValue);
						break;
					case NodeEvent.ParameterType.Int:
						SerializedProperty intVal = innerParamProp.FindPropertyRelative("mIntValue");
						intVal.intValue = EditorGUI.IntField(propRect, intVal.intValue);
						break;
					case NodeEvent.ParameterType.Float:
						SerializedProperty floatVal = innerParamProp.FindPropertyRelative("mFloatValue");
						floatVal.floatValue = EditorGUI.FloatField(propRect, floatVal.floatValue);
					break;
					case NodeEvent.ParameterType.Bool:
						SerializedProperty boolVal = innerParamProp.FindPropertyRelative("mBoolValue");
						boolVal.boolValue = EditorGUI.Toggle(propRect, boolVal.boolValue);
						break;
				}

				labelRect.y += labelRect.height;
				propRect.y += propRect.height;
			}

			EditorGUI.indentLevel--;
		}

		/// <summary>
		/// Generate the sub-properties for all the parameters that we have.
		/// </summary>
		/// <param name="rootProp">The root property.</param>
		/// <param name="validMethod">The method that this event is going to call.</param>
		private void GenerateParameters(SerializedProperty rootProp, MethodInfo validMethod)
		{
			SerializedProperty listProp = GetParameterListProp(rootProp);
			var parameters = validMethod.GetParameters();

			listProp.arraySize = parameters.Length;
			for (int i = 0; i < listProp.arraySize; ++i)
			{
				SerializedProperty typeProp = listProp.GetArrayElementAtIndex(i).FindPropertyRelative("mType");

				if (parameters[i].ParameterType == typeof(string))
					typeProp.enumValueIndex = (int)NodeEvent.ParameterType.String;
				else if (parameters[i].ParameterType == typeof(int))
					typeProp.enumValueIndex = (int)NodeEvent.ParameterType.Int;
				else if (parameters[i].ParameterType == typeof(float))
					typeProp.enumValueIndex = (int)NodeEvent.ParameterType.Float;
				else
					typeProp.enumValueIndex = (int)NodeEvent.ParameterType.Bool;
			}
		}

		/// <summary>
		/// Clears the list of parameters that this node event has saved.
		/// </summary>
		/// <param name="rootProp">The root property.</param>
		private void ClearParameters(SerializedProperty rootProp)
		{
			SerializedProperty listProp = GetParameterListProp(rootProp);
			listProp.arraySize = 0;
		}

		/// <summary>
		/// Validate the list of methods that Reflection has found.
		/// Checks to ensure that their parameters are all something we can provide.
		/// </summary>
		private List<MethodInfo> ValidateMethods(IList<MethodInfo> validMethods)
		{
			var result = new List<MethodInfo>(validMethods.Count);
			foreach (MethodInfo method in validMethods)
			{
				bool isValid = true;
				var paramList = method.GetParameters();
				foreach (ParameterInfo param in paramList)
				{
					if (param.IsOut)
					{
						isValid = false;
						break;
					}

					Type paramType = param.ParameterType;
					if (paramType != typeof(int) && paramType != typeof(string) && paramType != typeof(float) && paramType != typeof(bool))
						isValid = false;
				}

				if (isValid)
					result.Add(method);
			}

			return result;
		}
	}
}
