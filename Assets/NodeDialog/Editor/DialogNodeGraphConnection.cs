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
			Vector2 pos1 = associatedConnection.inNode.nodePosition;
			Vector2 pos2 = associatedConnection.outNode.nodePosition;

			Handles.DrawLine(pos1, pos2);
		}
	}
}
