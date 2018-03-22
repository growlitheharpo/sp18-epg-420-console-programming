using System.Collections.Generic;
using NodeDialog;
using UnityEngine;

namespace UnityEditor
{
	public class NodeEditorWindow : EditorWindow
	{
		private static CharacterDialogAsset kEditingDialog;

		public static void CreateNewWindow(DialogCharacter dialogCharacter)
		{
			kEditingDialog = dialogCharacter.dialogAsset;
			NodeEditorWindow window = GetWindow<NodeEditorWindow>();
			window.titleContent = new GUIContent("Node Dialog Editor");
		}

		private CharacterDialogAsset mCachedDialogAsset;
		private List<BaseDialogGraphNode> mNodes;
		private List<DialogNodeGraphConnection> mConnections;
		private GUIStyle mNodeStyle;
		private Vector2 mDrag;

		/// <summary>
		/// Setup all of the styles that we're going to use.
		/// </summary>
		private void OnEnable()
		{
			mDrag = Vector2.zero;
			mNodeStyle = new GUIStyle
			{
				border = new RectOffset(25, 25, 7, 7),
				normal =
				{
					background = EditorGUIUtility.IconContent("node0 hex").image as Texture2D,
					textColor = GUI.skin.box.normal.textColor
				},
				focused =
				{
					background = EditorGUIUtility.IconContent("node0 hex on").image as Texture2D,
					textColor = Color.white,
				},
				alignment = TextAnchor.MiddleCenter,
			};

			mNodes = new List<BaseDialogGraphNode>();
			mConnections = new List<DialogNodeGraphConnection>();
			
			InitializeFromCharacter();

			Undo.undoRedoPerformed += OnUndoRedoPerformed;
		}

		/// <summary>
		/// Clear our delegate from undoRedo so that we aren't kept alive in GC.
		/// </summary>
		private void OnDisable()
		{
			Undo.undoRedoPerformed -= OnUndoRedoPerformed;
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
		/// Initialize the window from a character.
		/// </summary>
		private void InitializeFromCharacter()
		{
			mNodes = new List<BaseDialogGraphNode>();
			mConnections = new List<DialogNodeGraphConnection>();

			mCachedDialogAsset = kEditingDialog;
			if (mCachedDialogAsset == null)
				return;

			var nodes = mCachedDialogAsset.GetNodes_Editor();
			foreach (BaseDialogNode n in nodes)
				mNodes.Add(new BaseDialogGraphNode(n, mNodeStyle, OnRemoveNode));

			var conns = mCachedDialogAsset.GetConnections_Editor();
			foreach (DialogNodeConnection c in conns)
				mConnections.Add(new DialogNodeGraphConnection(c));
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

			DrawNodes();
			DrawConnections();

			ProcessNodeEvents(Event.current);
			ProcessEvents(Event.current);

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
			
			foreach (DialogNodeGraphConnection c in mConnections)
				c.Draw();
		}

		/// <summary>
		/// Loop through every node and draw each.
		/// </summary>
		private void DrawNodes()
		{
			if (mNodes == null)
				return;

			foreach (BaseDialogGraphNode node in mNodes)
				node.Draw();
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
			rightClickMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
			rightClickMenu.ShowAsContext();
		}

		/// <summary>
		/// Handle the user clicking on "Add node" in the context menu.
		/// </summary>
		/// <param name="mousePosition">The position of the mouse when right click was first pressed.</param>
		private void OnClickAddNode(Vector2 mousePosition)
		{
			// Create a new asset, but allow it to be undone.
			BaseDialogNode newRealNode = mCachedDialogAsset.AddNode_Editor();
			newRealNode.nodePosition = mousePosition;

			mNodes.Add(new BaseDialogGraphNode(newRealNode, mNodeStyle, OnRemoveNode));
		}

		/// <summary>
		/// Handle the user attempting to remove a node.
		/// </summary>
		/// <param name="n">The node that is being removed.</param>
		private void OnRemoveNode(BaseDialogGraphNode n)
		{
			// Delete the asset, but allow it to be undone.
			mCachedDialogAsset.RemoveNode_Editor(n.associatedNode);

			// Remove the graph node from our list.
			mNodes.Remove(n);
		}
	}
}
