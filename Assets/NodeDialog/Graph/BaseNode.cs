using System.Collections.Generic;
using UnityEngine;

namespace NodeDialog.Graph
{
	/// <summary>
	/// The base type that will represent the different nodes in the system.
	/// </summary>
	[BaseNodeVisual("node0", "node0 on")]
	public class BaseNode : ScriptableObject
	{
		// These are used in the editor
		private const float WIDTH = 200.0f, HEIGHT = 50.0f;

		[SerializeField] [HideInInspector] private Vector2 mNodePosition;
		[SerializeField] [HideInInspector] private List<BaseConnection> mOutConnections;

#if UNITY_EDITOR

		/// <summary>
		/// EDITOR ONLY.
		/// Temporary drag variable. Not serialized.
		/// </summary>
		public Vector2 nodeDrag { get; set; }

		/// <summary>
		/// EDITOR ONLY.
		/// The rect of this node after non-serialized drag has been applied.
		/// </summary>
		public Rect rectWithDrag
		{
			get { return new Rect(mNodePosition + nodeDrag, new Vector2(WIDTH, HEIGHT)); }
		}

		/// <summary>
		/// EDITOR ONLY.
		/// The position of this node on the editor graph.
		/// </summary>
		public Vector2 nodePosition
		{
			get { return mNodePosition; }
			set { mNodePosition = value; }
		}

		/// <summary>
		/// EDITOR ONLY.
		/// The rect of this node (in the editor).
		/// </summary>
		public Rect rect
		{
			get { return new Rect(mNodePosition, new Vector2(WIDTH, HEIGHT)); }
		}

#endif

		/// <summary>
		/// The connections leading out of this node.
		/// </summary>
		public List<BaseConnection> outConnections
		{
			get { return mOutConnections ?? (mOutConnections = new List<BaseConnection>()); }
			set { mOutConnections = value; }
		}
	}
}
