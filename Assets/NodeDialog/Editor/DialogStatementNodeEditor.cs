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
			DrawSingleProperty("mStatement");
			DrawUserVariables();

			serializedObject.ApplyModifiedProperties();
		}
	}
}
