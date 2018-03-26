using UnityEditor;

namespace NodeDialog.Editor
{
	[CustomEditor(typeof(DialogStatementNode))]
	public class DialogStatementNodeEditor : BaseNodeEditor
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
