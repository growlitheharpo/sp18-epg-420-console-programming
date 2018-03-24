using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace NodeDialog
{
	[CreateAssetMenu(fileName = "DialogAsset", menuName = "Node Dialog/Dialog Asset File")]
	public class CharacterDialogAsset : ScriptableObject
	{
		[SerializeField] private List<BaseDialogNode> mNodes;
		[SerializeField] private List<DialogNodeConnection> mConnections;

#if UNITY_EDITOR

		/// <summary>
		/// Returns this asset's list of nodes.
		/// </summary>
		[NotNull] public List<BaseDialogNode> GetNodes_Editor()
		{
			return mNodes ?? (mNodes = new List<BaseDialogNode>());
		}

		/// <summary>
		/// Adds a new node to the list and returns it, but in a way that allows it to be undone.
		/// Registers all changes into the UnityEditor.Undo system.
		/// </summary>
		/// <returns>The newly created node.</returns>
		[NotNull] public BaseDialogNode AddNode_Editor()
		{
			if (mNodes == null)
				mNodes = new List<BaseDialogNode>();

			BaseDialogNode newNode = CreateInstance<BaseDialogNode>();

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
		public void RemoveNode_Editor(BaseDialogNode node)
		{
			if (mNodes == null)
				mNodes = new List<BaseDialogNode>();

			// Remove all this node's connections
			for (int i = 0; i < mConnections.Count; ++i)
			{
				DialogNodeConnection c = mConnections[i];
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
		[NotNull] public List<DialogNodeConnection> GetConnections_Editor()
		{
			return mConnections ?? (mConnections = new List<DialogNodeConnection>());
		}

		/// <summary>
		/// Adds a new connection to the list and returns it, but in a way that allows it to be undone.
		/// Registers all changes into the UnityEditor.Undo system.
		/// </summary>
		/// <param name="node1">The 'in' node of this connection</param>
		/// <param name="node2">The 'out' node of this connection.</param>
		/// <returns>The newly created connection</returns>
		[NotNull] public DialogNodeConnection AddConnection_Editor(BaseDialogNode node1, BaseDialogNode node2)
		{
			if (mConnections == null)
				mConnections = new List<DialogNodeConnection>();

			DialogNodeConnection newConnection = CreateInstance<DialogNodeConnection>();
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
		public void RemoveConnection_Editor(DialogNodeConnection connection)
		{
			if (mConnections == null)
				mConnections = new List<DialogNodeConnection>();

			BaseDialogNode inNode = connection.inNode;

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

			foreach (BaseDialogNode node in mNodes)
			{
				if (subassets.Contains(node))
					continue;

				UnityEditor.AssetDatabase.AddObjectToAsset(node, this);
				node.hideFlags = HideFlags.HideInHierarchy;
			}

			foreach (DialogNodeConnection conn in mConnections)
			{
				if (subassets.Contains(conn))
					continue;
				
				UnityEditor.AssetDatabase.AddObjectToAsset(conn, this);
				conn.hideFlags = HideFlags.HideInHierarchy;
			}
		}
	}

#endif
}
