using System;
using System.Collections.Generic;
using System.Linq;
using NodeDialog;
using NodeDialog.Graph;
using UnityEngine;

namespace UnityEditor
{
	/// <summary>
	/// The editor window for our dialog node graph.
	/// </summary>
	public class NodeGraphEditorWindow : EditorWindow
	{
		private static CharacterDialogAsset kEditingDialog;

		/// <summary>
		/// Create a new window for the provided character.
		/// </summary>
		public static void CreateNewWindow(DialogCharacter dialogCharacter)
		{
			kEditingDialog = dialogCharacter.dialogAsset;
			NodeGraphEditorWindow window = GetWindow<NodeGraphEditorWindow>();
			window.titleContent = new GUIContent("Node Dialog Editor");
		}

		private CharacterDialogAsset mCachedDialogAsset;
		private List<NodeGraphWindowBaseNode> mNodes;
		private List<NodeGraphWindowBaseConnection> mConnections;

		/// <summary>
		/// Setup all of the styles that we're going to use.
		/// </summary>
		private void OnEnable()
		{
			mNodes = new List<NodeGraphWindowBaseNode>();
			mConnections = new List<NodeGraphWindowBaseConnection>();
			
			InitializeFromCharacter();

			Undo.undoRedoPerformed += OnUndoRedoPerformed;
			Selection.selectionChanged += OnSelectionChanged;
		}

		/// <summary>
		/// Clear our delegate from undoRedo so that we aren't kept alive in GC.
		/// </summary>
		private void OnDisable()
		{
			Undo.undoRedoPerformed -= OnUndoRedoPerformed;
			Selection.selectionChanged -= OnSelectionChanged;

			AssetDatabase.SaveAssets();
		}

		/// <summary>
		/// Handle Unity notifiying us of an undo or redo.
		/// Because we don't get any more details, we're forced to fully regenerate
		/// the graph and do a repaint.
		/// </summary>
		private void OnUndoRedoPerformed()
		{
			GUI.changed = true;
			InitializeFromCharacter();
			Repaint();
		}

		/// <summary>
		/// Handle Unity notifiying us of an undo or redo.
		/// Force a repaint.
		/// </summary>
		private void OnSelectionChanged()
		{
			GUI.changed = true;
			Repaint();
		}

		/// <summary>
		/// Initialize the window from a character.
		/// </summary>
		private void InitializeFromCharacter()
		{
			mNodes = new List<NodeGraphWindowBaseNode>();
			mConnections = new List<NodeGraphWindowBaseConnection>();

			mCachedDialogAsset = kEditingDialog;
			if (mCachedDialogAsset == null)
				return;

			var nodes = mCachedDialogAsset.GetNodes_Editor();
			foreach (BaseNode n in nodes)
				mNodes.Add(new NodeGraphWindowBaseNode(n, GenerateNodeStyle(n.GetType()), OnRemoveNode, OnTryAddConnection));

			var conns = mCachedDialogAsset.GetConnections_Editor();
			foreach (BaseConnection c in conns)
				mConnections.Add(new NodeGraphWindowBaseConnection(c, OnRemoveConnection));
		}

		/// <summary>
		/// Generate a GUIStyle for the provided Node type.
		/// </summary>
		/// <param name="n">The type of the node that will be used.</param>
		private static GUIStyle GenerateNodeStyle(Type n)
		{
			BaseNodeVisualAttribute a = (BaseNodeVisualAttribute)Attribute.GetCustomAttribute(n, typeof(BaseNodeVisualAttribute));

			string normal = a == null ? BaseNodeVisualAttribute.DEFAULT_NORMAL_IMAGE : a.normalImage;
			string selected = a == null ? BaseNodeVisualAttribute.DEFAULT_SELECTED_IMAGE : a.selectedImage;

			return new GUIStyle
			{
				border = new RectOffset(25, 25, 7, 7),
				normal =
				{
					background = EditorGUIUtility.IconContent(normal).image as Texture2D,
					textColor = Color.black
				},
				focused =
				{
					background = EditorGUIUtility.IconContent(selected).image as Texture2D,
					textColor = Color.white,
				},
				alignment = TextAnchor.MiddleCenter,
			};
		}

		/// <summary>
		/// Draw the background and the nodes and then process events.
		/// </summary>
		private void OnGUI()
		{
			DrawBackground();
			DrawGrid(12.0f, Color.white * 0.420f);
			DrawGrid(120.0f, Color.white * 0.29f);
			
			if (mCachedDialogAsset == null)
				return;

			if (mCachedDialogAsset != kEditingDialog || mCachedDialogAsset.GetNodes_Editor().Count != mNodes.Count)
				InitializeFromCharacter();

			DrawConnections();
			DrawNodes();

			ProcessConnectionEvents(Event.current);
			ProcessNodeEvents(Event.current);
			ProcessEvents(Event.current);

			GUI.changed = true;

			if (GUI.changed)
				Repaint();
		}

		/// <summary>
		/// Draw a dark textured background across the entire window.
		/// </summary>
		private void DrawBackground()
		{
			Color oldCol = GUI.color;
			GUI.color = new Color(0.451f, 0.451f, 0.451f, 1.0f);
			GUI.DrawTexture(new Rect(0.0f, 0.0f, position.width, position.height), EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);
			GUI.color = oldCol;
		}

		/// <summary>
		/// Draw a grid at the given intervals and of the provided color across the entire window.
		/// </summary>
		/// <param name="spacing">How many pixels between each line.</param>
		/// <param name="col">The color of the line.</param>
		private void DrawGrid(float spacing, Color col)
		{
			int widthDivs = Mathf.CeilToInt(position.width / spacing);
			int heightDivs = Mathf.CeilToInt(position.height / spacing);

			Handles.BeginGUI();
			Handles.color = new Color(col.r, col.g, col.b, 1.0f);

			// Draw the vertical lines
			for (int i = 0; i < widthDivs; i++)
				Handles.DrawLine(new Vector3(spacing * i, 0.0f, 0), new Vector3(spacing * i, position.height, 0.0f));

			// Draw the horizontal lines
			for (int i = 0; i < heightDivs; i++)
				Handles.DrawLine(new Vector3(0.0f, spacing * i, 0), new Vector3(position.width, spacing * i, 0.0f));

			Handles.color = Color.white;
			Handles.EndGUI();
		}

		/// <summary>
		/// Loop through every connection and draw each.
		/// </summary>
		private void DrawConnections()
		{
			if (mConnections == null)
				return;
			
			foreach (NodeGraphWindowBaseConnection c in mConnections)
				c.Draw();
		}

		/// <summary>
		/// Loop through every node and draw each.
		/// </summary>
		private void DrawNodes()
		{
			if (mNodes == null)
				return;

			foreach (NodeGraphWindowBaseNode node in mNodes)
				node.Draw();
		}

		/// <summary>
		/// Loop backwards through all of the connections and allow them to provess their events.
		/// NOTE: We go backwards because the ones at the end are drawn last (i.e., on top)
		/// </summary>
		private void ProcessConnectionEvents(Event current)
		{
			if (mConnections == null)
				return;

			for (int i = mConnections.Count - 1; i >= 0; --i)
				GUI.changed = mConnections[i].ProcessEvents(current) || GUI.changed;
		}

		/// <summary>
		/// Loop backwards through all of the nodes and allow them to process their events.
		/// NOTE: We go backwards because the ones at the end are drawn last (i.e., on top)
		/// </summary>
		private void ProcessNodeEvents(Event current)
		{
			if (mNodes == null)
				return;

			for (int i = mNodes.Count - 1; i >= 0; --i)
				GUI.changed = mNodes[i].ProcessEvents(current) || GUI.changed;
		}

		/// <summary>
		/// Process events for the canvas as a whole.
		/// </summary>
		private void ProcessEvents(Event current)
		{
			switch (current.type)
			{
				case EventType.MouseDown:
					if (current.button == 1)
						ProcessContextMenu(current.mousePosition);
					break;
			}
		}

		/// <summary>
		/// Creates a right-click menu with the associated options.
		/// </summary>
		private void ProcessContextMenu(Vector2 mousePosition)
		{
			GenericMenu rightClickMenu = new GenericMenu();
			rightClickMenu.AddItem(new GUIContent("Add statement node"), false, () => OnClickAddStatementNode(mousePosition));
			rightClickMenu.AddItem(new GUIContent("Add choice node"), false, () => OnClickAddChoiceNode(mousePosition));
			rightClickMenu.ShowAsContext();
		}

		/// <summary>
		/// Attempt to add a new connection between the provided node and whichever is under the mouse.
		/// </summary>
		/// <param name="startNode">The node we are drawing the connection FROM</param>
		/// <param name="mousePosition">The current mouse position.</param>
		/// <returns>True if this was successful, otherwise false.</returns>
		private bool OnTryAddConnection(NodeGraphWindowBaseNode startNode, Vector2 mousePosition)
		{
			NodeGraphWindowBaseNode target = mNodes.LastOrDefault(x => x.associatedNode.rect.Contains(mousePosition) && x != startNode);

			// If target is null, or we already have a connection to it, "fail" this connection.
			if (target == null || startNode.associatedNode.outConnections.Any(x => x.outNode == target.associatedNode))
				return false;

			// Create a new connection
			BaseConnection connection = mCachedDialogAsset.AddConnection_Editor(startNode.associatedNode, target.associatedNode);
			mConnections.Add(new NodeGraphWindowBaseConnection(connection, OnRemoveConnection));

			return true;
		}

		/// <summary>
		/// Handle the user clicking on "Add node" in the context menu.
		/// </summary>
		/// <param name="mousePosition">The position of the mouse when right click was first pressed.</param>
		private void OnClickAddStatementNode(Vector2 mousePosition)
		{
			// Create a new asset, but allow it to be undone.
			BaseNode newRealNode = mCachedDialogAsset.AddNode_Editor<DialogStatementNode>();
			newRealNode.nodePosition = mousePosition;

			mNodes.Add(new NodeGraphWindowBaseNode(newRealNode, GenerateNodeStyle(newRealNode.GetType()), OnRemoveNode, OnTryAddConnection));
		}

		/// <summary>
		/// Handle the user clicking on "Add node" in the context menu.
		/// </summary>
		/// <param name="mousePosition">The position of the mouse when right click was first pressed.</param>
		private void OnClickAddChoiceNode(Vector2 mousePosition)
		{
			// Create a new asset, but allow it to be undone.
			BaseNode newRealNode = mCachedDialogAsset.AddNode_Editor<DialogChoiceNode>();
			newRealNode.nodePosition = mousePosition;

			mNodes.Add(new NodeGraphWindowBaseNode(newRealNode, GenerateNodeStyle(newRealNode.GetType()), OnRemoveNode, OnTryAddConnection));
		}

		/// <summary>
		/// Handle the user attempting to remove a node.
		/// </summary>
		/// <param name="n">The node that is being removed.</param>
		private void OnRemoveNode(NodeGraphWindowBaseNode n)
		{
			// Delete the asset, but allow it to be undone.
			mCachedDialogAsset.RemoveNode_Editor(n.associatedNode);

			// Reinitialize because this might've caused connections to be deleted too.
			InitializeFromCharacter();
		}

		/// <summary>
		/// Handle the user attempting to remove a connection.
		/// </summary>
		/// <param name="connection">The connection to remove.</param>
		private void OnRemoveConnection(NodeGraphWindowBaseConnection connection)
		{
			// Delete the asset, but allow it to be undone.
			mCachedDialogAsset.RemoveConnection_Editor(connection.associatedConnection);

			// Rebuild our graph form the character because this might've caused other effects.
			InitializeFromCharacter();
		}
	}
}
