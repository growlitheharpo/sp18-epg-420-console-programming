using UnityEditor;

namespace NodeDialog.Editor
{
	public abstract class BaseNodeEditor : UnityEditor.Editor
	{
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
	}
}
