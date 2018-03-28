using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NodeDialog.Editor
{
	public abstract class BaseNodeEditor : UnityEditor.Editor
	{
		private ReorderableList mUserVariableList;

		/// <summary>
		/// Handle the Editor being enabled.
		/// </summary>
		protected virtual void OnEnable()
		{
			mUserVariableList = new ReorderableList(serializedObject, serializedObject.FindProperty("mUserVariables"))
			{
				drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "User Variables"); },
				drawElementCallback = (rect, i, active, focused) =>
				{
					SerializedProperty prop = mUserVariableList.serializedProperty.GetArrayElementAtIndex(i);
					EditorGUI.PropertyField(rect, prop, true);
				},
				elementHeightCallback = (i) =>
				{ 
					SerializedProperty prop = mUserVariableList.serializedProperty.GetArrayElementAtIndex(i);
					return DialogNodeUserVariableDrawer.GetHeight(prop);
				}
			};
		}

		/// <summary>
		/// Draw the name field for this dialog box.
		/// </summary>
		protected void DrawNameField()
		{
			string newName = EditorGUILayout.DelayedTextField("Name", target.name);
			if (newName == target.name)
				return;

			Undo.RecordObject(target, "Change name");
			target.name = newName;
		}

		/// <summary>
		/// Shorthand for EditorGUILayout.PropertyField
		/// </summary>
		/// <param name="name">The name of the property to draw.</param>
		/// <param name="withChildren">Whether to include the property's children.</param>
		protected void DrawSingleProperty(string name, bool withChildren = true)
		{
			var prop = serializedObject.FindProperty(name);
			EditorGUILayout.PropertyField(prop, withChildren);
		}

		/// <summary>
		/// Draw the user variables section for this dialog node.
		/// </summary>
		protected void DrawUserVariables()
		{
			mUserVariableList.DoLayoutList();
			//var prop = serializedObject.FindProperty("mUserVariables");
			//EditorGUILayout.PropertyField(prop, true);
			//	EditorGUI.indentLevel++;

			//	prop.arraySize = EditorGUILayout.DelayedIntField(prop.arraySize, "Count");

			//	for (int i = 0; i < prop.arraySize; ++i)
			//	{
			//		var inner = prop.GetArrayElementAtIndex(i);
			//		EditorGUILayout.PropertyField(inner);
			//	}

			//	EditorGUI.indentLevel--;
		}
	}

	[CustomPropertyDrawer(typeof(DialogBaseNode.UserVariable))]
	public class DialogNodeUserVariableDrawer : PropertyDrawer
	{
		public static float GetHeight(SerializedProperty property, GUIContent label = null)
		{
			return EditorGUIUtility.singleLineHeight + 10.0f;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return GetHeight(property, label);
		}

		public override void OnGUI(Rect p, SerializedProperty property, GUIContent label)
		{
			float padding = (p.height - EditorGUIUtility.singleLineHeight) * 0.5f;

			Rect r1 = new Rect(p.x, p.y + padding, p.width * 0.3f, EditorGUIUtility.singleLineHeight);
			Rect r2 = new Rect(r1.x + r1.width + 20, r1.y, p.width - r1.width - 20, r1.height);

			EditorGUI.PropertyField(r1, property.FindPropertyRelative("mName"), GUIContent.none);
			EditorGUI.PropertyField(r2, property.FindPropertyRelative("mValue"), GUIContent.none);
		}
	}
}
