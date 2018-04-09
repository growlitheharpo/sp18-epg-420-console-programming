using NodeDialog.Editor.Graph;
using NodeDialog.Interfaces;
using NodeDialog.Samples;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(SampleDialogCharacter))]
	public class SampleDialogCharacterEditor : Editor
	{
		/// <inheritdoc />
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			DrawDefaultInspector();

			if (GUILayout.Button("Edit Dialog Tree"))
				NodeGraphEditorWindow.CreateNewWindow(target as IDialogSpeaker);
		}
	}
}
