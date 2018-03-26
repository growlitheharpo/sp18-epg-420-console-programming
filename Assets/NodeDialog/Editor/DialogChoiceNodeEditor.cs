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
			DrawDefaultInspector();

			serializedObject.ApplyModifiedProperties();
		}
	}
}
