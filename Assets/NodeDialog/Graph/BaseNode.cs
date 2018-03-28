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

		// Temporary drag variable. Only usable in-editor.
		private Vector2 mDrag;

		/// <summary>
		/// The position of this node on the editor graph. Not useful at runtime.
		/// </summary>
		public Vector2 nodePosition
		{
			get { return mNodePosition; }
			set { mNodePosition = value; }
		}

		public Vector2 nodeDrag
		{
			get
			{
#if UNITY_EDITOR
				return mDrag;
#else
				return Vector2.zero;
#endif
			}
			set
			{
#if UNITY_EDITOR
				mDrag = value;
#else
				;
#endif
			}
		}

		/// <summary>
		/// The rect of this node (in the editor).
		/// </summary>
		public Rect rect
		{
			get { return new Rect(mNodePosition, new Vector2(WIDTH, HEIGHT)); }
		}

		/// <summary>
		/// The rect of this node after drag has been applied (in the editor).
		/// </summary>
		public Rect rectWithDrag
		{
			get { return new Rect(mNodePosition + mDrag, new Vector2(WIDTH, HEIGHT)); }
		}

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
