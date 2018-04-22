using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NodeDialog.Editor
{
	public abstract class BaseNodeEditor : UnityEditor.Editor
	{
		private ReorderableList mUserVariableList;
		private ReorderableList mPreEventList;
		private ReorderableList mPostEventList;

        /// <summary>
        /// Handle the Editor being enabled.
        /// </summary>
        protected virtual void OnEnable()
		{
			mUserVariableList = InitializeList("mUserVariables", "User Variables");
			mPreEventList = InitializeList("mOnStartEvents", "OnEnter Events", 8.0f);
			mPostEventList = InitializeList("mOnCompleteEvents", "OnExit Events", 8.0f);
		}

		private ReorderableList InitializeList(string propertyName, string label, float sizeAdjust = 0.0f)
		{
			return new ReorderableList(serializedObject, serializedObject.FindProperty(propertyName))
			{
				drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, label); },
				drawElementCallback = (rect, i, active, focused) =>
				{
					SerializedProperty prop = serializedObject.FindProperty(propertyName).GetArrayElementAtIndex(i);

					rect.x += sizeAdjust;
					rect.width -= sizeAdjust;

					EditorGUI.PropertyField(rect, prop, true);
				},
				elementHeightCallback = i =>
				{
					SerializedProperty prop = serializedObject.FindProperty(propertyName).GetArrayElementAtIndex(i);
                    return EditorGUI.GetPropertyHeight(prop, true);
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
		/// <param name="propName">The name of the property to draw.</param>
        /// <param name="withChildren">Whether to include the property's children.</param>
        protected void DrawSingleProperty(string propName, bool withChildren = true)
		{
			SerializedProperty prop = serializedObject.FindProperty(propName);
			EditorGUILayout.PropertyField(prop, withChildren);
		}

		/// <summary>
		/// Draw the user variables section for this dialog node.
		/// </summary>
		protected void DrawUserVariables()
		{
			mUserVariableList.DoLayoutList();
		}

		/// <summary>
        /// Draw the event lists for this dialog node.
        /// </summary>
		protected void DrawEvents()
		{
			mPreEventList.DoLayoutList();
			mPostEventList.DoLayoutList();
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
