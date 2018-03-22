using NodeDialog;
using UnityEngine;

namespace UnityEditor
{
	public class DialogNodeGraphConnection
	{
		public DialogNodeConnection associatedConnection { get; private set; }

		public DialogNodeGraphConnection(DialogNodeConnection connection)
		{
			associatedConnection = connection;
		}

		public void Draw()
		{
			Handles.DrawLine(
				associatedConnection.inNode.rect.center,
				associatedConnection.outNode.rect.center);
		}
	}
}
