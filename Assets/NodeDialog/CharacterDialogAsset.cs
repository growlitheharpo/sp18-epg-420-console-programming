using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using NodeDialog.Graph;
using UnityEngine;

namespace NodeDialog
{
	/// <summary>
	/// A dialog asset. Holds the necessary information to construct a directed dialog graph
	/// and traverse it at runtime.
	/// </summary>
	[CreateAssetMenu(fileName = "DialogAsset", menuName = "Node Dialog/Dialog Asset File")]
	public class CharacterDialogAsset : ScriptableObject
	{
		[SerializeField] private List<BaseConnection> mConnections;
		[SerializeField] private List<BaseNode> mNodes;

		/// <summary>
		/// A read-only copy of this asset's dialog nodes. Can be traversed in-game.
		/// </summary>
		public ReadOnlyCollection<BaseNode> nodes { get { return mNodes.AsReadOnly(); } }

		/// <summary>
		/// A read-only copy of this asset's connections. Generally not useful in-game.
		/// </summary>
		public ReadOnlyCollection<BaseConnection> connections { get { return mConnections.AsReadOnly(); } }

		/// <summary>
		/// Get the first node in this asset.
		/// TODO: Make this safer by flagging the node OR finding the one with no "in" connections
		/// </summary>
		public DialogBaseNode rootNode { get { return mNodes[0] as DialogBaseNode; } }

#if UNITY_EDITOR

		/// <summary>
		/// Returns this asset's list of nodes.
		/// </summary>
		[NotNull]
		public List<BaseNode> GetNodes_Editor()
		{
			return mNodes ?? (mNodes = new List<BaseNode>());
		}

		/// <summary>
		/// Adds a new node to the list and returns it, but in a way that allows it to be undone.
		/// Registers all changes into the UnityEditor.Undo system.
		/// </summary>
		/// <returns>The newly created node.</returns>
		[NotNull]
		public BaseNode AddNode_Editor<T>() where T : BaseNode
		{
			if (mNodes == null)
				mNodes = new List<BaseNode>();

			BaseNode newNode = CreateInstance<T>();

			UnityEditor.Undo.RegisterCreatedObjectUndo(newNode, "Create New Node");
			UnityEditor.Undo.RecordObject(this, "Create New Node");

			mNodes.Add(newNode);
			OnValidate();

			UnityEditor.EditorUtility.SetDirty(this);
			return newNode;
		}

		/// <summary>
		/// Removes the given node from our list, but in a way that allows it to be undone.
		/// Registers all changes into the UnityEditor.Undo system.
		/// </summary>
		/// <param name="node">The node to remove.</param>
		public void RemoveNode_Editor(BaseNode node)
		{
			if (mNodes == null)
				mNodes = new List<BaseNode>();

			// Remove all this node's connections
			for (int i = 0; i < mConnections.Count; ++i)
			{
				BaseConnection c = mConnections[i];
				if (c.inNode != node && c.outNode != node)
					continue;

				RemoveConnection_Editor(c);
				--i;
			}

			// Delete the asset, but in a way that allows it to be undone.
			UnityEditor.Undo.RegisterCompleteObjectUndo(this, "Delete Node");
			UnityEditor.Undo.DestroyObjectImmediate(node);

			mNodes.Remove(node);
			OnValidate();

			// Flag our asset as dirty so that Unity updates it.
			UnityEditor.EditorUtility.SetDirty(this);
		}

		/// <summary>
		/// Returns this asset's list of connections.
		/// </summary>
		[NotNull]
		public List<BaseConnection> GetConnections_Editor()
		{
			return mConnections ?? (mConnections = new List<BaseConnection>());
		}

		/// <summary>
		/// Adds a new connection to the list and returns it, but in a way that allows it to be undone.
		/// Registers all changes into the UnityEditor.Undo system.
		/// </summary>
		/// <param name="node1">The 'in' node of this connection</param>
		/// <param name="node2">The 'out' node of this connection.</param>
		/// <returns>The newly created connection</returns>
		[NotNull]
		public BaseConnection AddConnection_Editor(BaseNode node1, BaseNode node2)
		{
			if (mConnections == null)
				mConnections = new List<BaseConnection>();

			BaseConnection newConnection = CreateInstance<BaseConnection>();
			newConnection.inNode = node1;
			newConnection.outNode = node2;

			UnityEditor.Undo.RegisterCreatedObjectUndo(newConnection, "Create New Connection");
			UnityEditor.Undo.RecordObject(node1, "Create New Connection");
			UnityEditor.Undo.RecordObject(this, "Create New Connection");

			node1.outConnections.Add(newConnection);
			mConnections.Add(newConnection);
			OnValidate();

			UnityEditor.EditorUtility.SetDirty(this);
			return newConnection;
		}

		/// <summary>
		/// Removes the given connection from our list, but in a safe way that allows it to be undone.
		/// Registers all changes into the UnityEditor.Undo system.
		/// </summary>
		/// <param name="connection">The connection to remove.</param>
		public void RemoveConnection_Editor(BaseConnection connection)
		{
			if (mConnections == null)
				mConnections = new List<BaseConnection>();

			BaseNode inNode = connection.inNode;

			UnityEditor.Undo.RegisterCompleteObjectUndo(this, "Delete Connection");
			UnityEditor.Undo.RegisterCompleteObjectUndo(inNode, "Delete Connection");
			UnityEditor.Undo.DestroyObjectImmediate(connection);

			connection.inNode.outConnections.Remove(connection);
			mConnections.Remove(connection);
			OnValidate();

			UnityEditor.EditorUtility.SetDirty(this);
		}

		/// <summary>
		/// Finds and validates any data referenced by this object in the above lists
		/// and ensures that it is properly serialized into the same asset.
		/// Also automatically called by Unity on Undo/Redo.
		/// </summary>
		private void OnValidate()
		{
			string path = UnityEditor.AssetDatabase.GetAssetPath(this);
			var subassets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);

			foreach (BaseNode node in mNodes)
			{
				if (subassets.Contains(node))
					continue;

				UnityEditor.AssetDatabase.AddObjectToAsset(node, this);
				node.hideFlags = HideFlags.HideInHierarchy;
			}

			foreach (BaseConnection conn in mConnections)
			{
				if (subassets.Contains(conn))
					continue;

				UnityEditor.AssetDatabase.AddObjectToAsset(conn, this);
				conn.hideFlags = HideFlags.HideInHierarchy;
			}
		}
#endif
	}
}
