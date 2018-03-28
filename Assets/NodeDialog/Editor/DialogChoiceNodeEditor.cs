using UnityEditor;

namespace NodeDialog.Editor
{
	[CustomEditor(typeof(DialogChoiceNode))]
	public class DialogChoiceNodeEditor : BaseNodeEditor
	{
		/// <inheritdoc />
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			DrawNameField();
			DrawSingleProperty("mPrompt");
			DrawUserVariables();

			serializedObject.ApplyModifiedProperties();
		}
	}
}
