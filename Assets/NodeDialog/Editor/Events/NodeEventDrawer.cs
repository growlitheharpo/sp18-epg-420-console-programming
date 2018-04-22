using System;
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
				var newValue = methodNames[currentIndex];
				if (methodProp.stringValue == newValue)
					return;

				methodProp.stringValue = newValue;
				GenerateParameters(rootProp, validMethods[currentIndex - 1]);
			}
			else
			{
				methodProp.stringValue = "";
				ClearParameters(rootProp);
			}
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

			listProp.arraySize = 0;
			listProp.arraySize = parameters.Length;
			for (int i = 0; i < listProp.arraySize; ++i)
			{
				SerializedProperty typeProp = listProp.GetArrayElementAtIndex(i).FindPropertyRelative("mType");

				if (parameters[i].ParameterType == typeof(string))
					typeProp.enumValueIndex = (int)NodeEvent.ParameterType.String;
				else if (parameters[i].ParameterType == typeof(int))
					typeProp.enumValueIndex = (int)NodeEvent.ParameterType.Int;
				else
					typeProp.enumValueIndex = (int)NodeEvent.ParameterType.Float;
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

					// If it has a default value, we can safely ignore it.
					if ((param.Attributes & ParameterAttributes.HasDefault) != ParameterAttributes.None)
						continue;

					Type paramType = param.ParameterType;
					if (paramType != typeof(int) && paramType != typeof(string) && paramType != typeof(float))
						isValid = false;
				}

				if (isValid)
					result.Add(method);
			}

			return result;
		}
	}
}
