using NodeDialog;
using NodeDialog.Editor.Graph;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(DialogCharacter))]
	public class DialogCharacterEditor : Editor
	{
		/// <inheritdoc />
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			DrawDefaultInspector();

			if (GUILayout.Button("Edit Dialog Tree"))
				NodeGraphEditorWindow.CreateNewWindow(target as DialogCharacter);
		}
	}
}
