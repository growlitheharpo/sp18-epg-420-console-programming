using UnityEngine;

namespace NodeDialog
{
	/// <summary>
	/// A serializable connection between two nodes.
	/// </summary>
	public class DialogNodeConnection : ScriptableObject
	{
		[SerializeField] private BaseDialogNode mInNode;
		[SerializeField] private BaseDialogNode mOutNode;
		[SerializeField] private string mConnectionText;

		/// <summary>
		/// The "in" node that starts this connection.
		/// </summary>
		public BaseDialogNode inNode
		{
			get { return mInNode; }
			set { mInNode = value; }
		}

		/// <summary>
		/// The "out" node that this connection leads to.
		/// </summary>
		public BaseDialogNode outNode
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
