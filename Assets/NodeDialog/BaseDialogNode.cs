using System.Collections.Generic;
using UnityEngine;

namespace NodeDialog
{
	/// <summary>
	/// The base type that will represent the different nodes in the system.
	/// </summary>
	[BaseNodeVisual("node0", "node0 on")]
	public class BaseDialogNode : ScriptableObject
	{
		// These are used in the editor
		private const float WIDTH = 200.0f, HEIGHT = 50.0f;

		[SerializeField] private Vector2 mNodePosition;
		[SerializeField] private List<DialogNodeConnection> mOutConnections;

		/// <summary>
		/// The position of this node on the editor graph. Not useful at runtime.
		/// </summary>
		public Vector2 nodePosition
		{
			get { return mNodePosition; }
			set { mNodePosition = value; }
		}

		/// <summary>
		/// The connections leading out of this node.
		/// </summary>
		public List<DialogNodeConnection> outConnections
		{
			get { return mOutConnections ?? (mOutConnections = new List<DialogNodeConnection>()); }
			set { mOutConnections = value; }
		}

		/// <summary>
		/// The rect of this node (in the editor).
		/// </summary>
		public Rect rect
		{
			get { return new Rect(mNodePosition, new Vector2(WIDTH, HEIGHT)); }
		}
	}
}
