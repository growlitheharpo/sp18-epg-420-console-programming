using System;
using NodeDialog.Graph;
using UnityEditor;
using UnityEngine;

namespace NodeDialog.Editor.Graph
{
	/// <summary>
	/// Representation of a BaseNode on the Node Graph Editor
	/// </summary>
	public class NodeGraphWindowBaseNode
	{
		private readonly Action<NodeGraphWindowBaseNode> mRemoveNodeCallback;
		private readonly Func<NodeGraphWindowBaseNode, Vector2, bool> mTryAddConnectionCallback;
		private readonly GUIStyle mMasterStyle;

		private bool mIsDragged, mInConnectionMode;

		/// <summary>
		/// Is our node selected in the Unity editor?
		/// </summary>
		private bool isSelected { get { return Selection.activeObject == associatedNode; } }

		/// <summary>
		/// The actual node asset associated with this graph node.
		/// </summary>
		public BaseNode associatedNode { get; private set; }

		/// <summary>
		/// Representation of a BaseNode on the NodeEditorGraph
		/// </summary>
		/// <param name="node">The actual node asset that this graph node will be associated with.</param>
		/// <param name="style">The GUIStyle for this node.</param>
		/// <param name="removeNodeCallback">The callback for when this node needs to be deleted.</param>
		/// <param name="addConnectionCallback">The callback for when this node wants to add a new connection.</param>
		public NodeGraphWindowBaseNode(BaseNode node, GUIStyle style, Action<NodeGraphWindowBaseNode> removeNodeCallback, Func<NodeGraphWindowBaseNode, Vector2, bool> addConnectionCallback)
		{
			associatedNode = node;
			mMasterStyle = style;
			mRemoveNodeCallback = removeNodeCallback;
			mTryAddConnectionCallback = addConnectionCallback;
		}

		/// <summary>
		/// Draw this node on the graph.
		/// </summary>
		public void Draw()
		{
			if (mInConnectionMode)
				NodeGraphWindowBaseConnection.DrawLine(associatedNode.rect.center, Event.current.mousePosition);

			GUI.SetNextControlName(associatedNode.GetInstanceID().ToString());
			
			// TODO: Why doesn't focusing work anymore?
			GUIStyle styleCopy = new GUIStyle(mMasterStyle);
			if (Selection.activeObject == associatedNode)
				styleCopy.normal = mMasterStyle.focused;

			GUI.Box(associatedNode.rect, associatedNode.name, styleCopy);
		}

		/// <summary>
		/// Handle any mouse or keyboard events that relate to this node.
		/// </summary>
		/// <param name="e">The current event being processed.</param>
		/// <returns>True if the canvas needs to be repainted because of this node, false otherwise.</returns>
		public bool ProcessEvents(Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
					return HandleEventMouseDown(e);
				case EventType.MouseUp:
					mIsDragged = false;
					break;
				case EventType.MouseDrag:
					if (e.button == 0 && mIsDragged)
					{
						Drag(e.delta);
						e.Use();
						return true;
					}

					break;
				case EventType.KeyDown: // check for the delete key being pressed
					if (e.keyCode == KeyCode.Delete && isSelected)
						OnClickRemoveNode();
					break;
			}

			// If we're in connection mode, we want to force a repaint, so return true.
			return mInConnectionMode;
		}

		/// <summary>
		/// Handle any mouse button being initially clicked.
		/// </summary>
		/// <param name="e">The event that is currently being processed.</param>
		/// <returns>True if the canvas needs to be repainted because of this node, false otherwise.</returns>
		private bool HandleEventMouseDown(Event e)
		{
			if (mInConnectionMode)
			{
				mInConnectionMode = false;

				// If this returns false, we failed in adding a connection, so deselect.
				if (!mTryAddConnectionCallback.Invoke(this, e.mousePosition))
				{
					Deselect();
					return true;
				}

				// We added a new connection, so repaint because of that.
				return true;
			}

			// If we're not in connection mode, we should check to see if we were selected/deselected.
			bool insideRect = associatedNode.rect.Contains(e.mousePosition);

			if (e.button == 0)
			{
				if (insideRect) // left click inside our rect -> select us.
				{
					Select();
					return true;
				}
				else if (isSelected) // left click not inside but we're selected -> deselect
				{
					Deselect();
					return true;
				}
			}
			else if (e.button == 1)
			{
				if (insideRect) // right click inside our rect -> select and open context
				{
					Select(false);

					ProcessContextMenu();
					e.Use();

					return true;
				}
				else if (isSelected) // right click outside our rect -> deselect (and let graph open context)
				{
					Deselect();
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Select this node.
		/// </summary>
		/// <param name="drag">Also flag us for being dragged (i.e., left clicked)</param>
		private void Select(bool drag = true)
		{
			mIsDragged = drag;
			Selection.SetActiveObjectWithContext(associatedNode, associatedNode);
		}

		/// <summary>
		/// Deselect this node.
		/// </summary>
		private void Deselect()
		{
			mIsDragged = false;

			if (Selection.activeObject == associatedNode)
				Selection.SetActiveObjectWithContext(null, null);
		}

		/// <summary>
		/// Drag the node's position based on the mouse's delta.
		/// </summary>
		/// TODO: Accumulate this in a local variable then apply it to the asset on mouse-up.
		private void Drag(Vector2 delta)
		{
			associatedNode.nodePosition += delta;
		}

		/// <summary>
		/// Open a right-click menu on the node.
		/// </summary>
		private void ProcessContextMenu()
		{
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Remove Node"), false, OnClickRemoveNode);
			menu.AddItem(new GUIContent("Add connection"), false, OnClickAddConnection);
			menu.ShowAsContext();
		}

		/// <summary>
		/// Handle the user clicking "remove node" in the right-click menu.
		/// </summary>
		private void OnClickRemoveNode()
		{
#if NET_4_6
			mRemoveNodeCallback?.Invoke(this);
#else
			if (mRemoveNodeCallback != null)
				mRemoveNodeCallback.Invoke(this);
#endif
		}

		/// <summary>
		/// Handle the user clicking "Add connection" in the right-click menu.
		/// </summary>
		private void OnClickAddConnection()
		{
			mInConnectionMode = true;
		}
	}
}
