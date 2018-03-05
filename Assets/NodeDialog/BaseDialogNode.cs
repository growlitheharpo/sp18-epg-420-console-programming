using System.Collections.Generic;
using UnityEngine;

namespace NodeDialog
{
	/// <summary>
	/// The base type that will represent the different nodes in the system.
	/// </summary>
	public class BaseDialogNode : ScriptableObject
	{
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
			get { return mOutConnections; }
			set { mOutConnections = value; }
		}
	}
}
