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

		public void AddNode_Editor(DialogNodeConnection newNode)
		{
			if (mConnections == null)
				mConnections = new List<DialogNodeConnection>();

			mConnections.Add(newNode);
			OnValidate();
		}

		public void RemoveNode_Editor(DialogNodeConnection node)
		{
			if (mConnections == null)
				mConnections = new List<DialogNodeConnection>();

			mConnections.Remove(node);
			OnValidate();
		}

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

			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
		}
	}

#endif
}
