using UnityEngine;

namespace NodeDialog.Graph
{
	/// <summary>
	/// A serializable connection between two nodes.
	/// </summary>
	public class BaseConnection : ScriptableObject
	{
		[SerializeField] private string mConnectionText;
		[SerializeField] [HideInInspector] private BaseNode mInNode;
		[SerializeField] [HideInInspector] private BaseNode mOutNode;

		/// <summary>
		/// The "in" node that starts this connection.
		/// </summary>
		public BaseNode inNode
		{
			get { return mInNode; }
			set { mInNode = value; }
		}

		/// <summary>
		/// The "out" node that this connection leads to.
		/// </summary>
		public BaseNode outNode
		{
			get { return mOutNode; }
			set { mOutNode = value; }
		}

		/// <summary>
		/// The text associated with this connection, such as a player choice.
		/// </summary>
		public string connectionText
		{
			get { return mConnectionText; }
			set { mConnectionText = value; }
		}
	}
}
