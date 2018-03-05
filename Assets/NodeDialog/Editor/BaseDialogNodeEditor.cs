using NodeDialog;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(BaseDialogNode))]
	public class BaseDialogNodeEditor : Editor
	{
		#region INSPECTOR
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
		}
		#endregion
	}
}
