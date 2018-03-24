using System;
using NodeDialog;
using UnityEngine;

namespace UnityEditor
{
	public class BaseDialogGraphNode
	{
		private readonly Action<BaseDialogGraphNode> mRemoveNodeCallback;
		private readonly Func<BaseDialogGraphNode, Vector2, bool> mTryAddConnectionCallback;
		private readonly GUIStyle mMasterStyle;

		private bool mIsDragged, mInConnectionMode;

		public BaseDialogNode associatedNode { get; private set; }
		private bool isSelected { get { return Selection.activeObject == associatedNode; } }

		public BaseDialogGraphNode(BaseDialogNode node, GUIStyle style, Action<BaseDialogGraphNode> removeNodeCallback, Func<BaseDialogGraphNode, Vector2, bool> addConnectionCallback)
		{
			associatedNode = node;
			mMasterStyle = style;
			mRemoveNodeCallback = removeNodeCallback;
			mTryAddConnectionCallback = addConnectionCallback;
		}

		public void Draw(Vector2 mousePos)
		{
			if (mInConnectionMode)
				DialogNodeGraphConnection.DrawLine(associatedNode.rect.center, mousePos);

			string nodeName = "NODE" + associatedNode.GetInstanceID();
			GUI.SetNextControlName(nodeName);
			
			// TODO: Why doesn't focusing work anymore?
			GUIStyle styleCopy = new GUIStyle(mMasterStyle);
			if (Selection.activeObject == associatedNode)
				styleCopy.normal = mMasterStyle.focused;

			GUI.Box(associatedNode.rect, nodeName + isSelected, styleCopy);
		}

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

		private bool HandleEventMouseDown(Event e)
		{
			if (mInConnectionMode)
			{
				if (!mTryAddConnectionCallback.Invoke(this, e.mousePosition))
				{
					Deselect();
					e.Use();
					return true;
				}

				mInConnectionMode = false;
			}

			bool insideRect = associatedNode.rect.Contains(e.mousePosition);

			if (e.button == 0)
			{
				if (insideRect)
				{
					Select();
					return true;
				}
				else if (isSelected)
				{
					Deselect();
					return true;
				}
			}
			else if (e.button == 1)
			{
				if (insideRect)
				{
					Select(false);

					ProcessContextMenu();
					e.Use();

					return true;
				}
				else if (isSelected)
				{
					Deselect();
					return true;
				}
			}

			return false;
		}

		private void Select(bool drag = true)
		{
			mIsDragged = drag;
			Selection.SetActiveObjectWithContext(associatedNode, associatedNode);
		}

		private void Deselect()
		{
			mIsDragged = false;

			if (Selection.activeObject == associatedNode)
				Selection.SetActiveObjectWithContext(null, null);
		}

		private void Drag(Vector2 delta)
		{
			associatedNode.nodePosition += delta;
		}

		private void ProcessContextMenu()
		{
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Remove Node"), false, OnClickRemoveNode);
			menu.AddItem(new GUIContent("Add connection"), false, OnClickAddConnection);
			menu.ShowAsContext();
		}

		private void OnClickRemoveNode()
		{
#if NET_4_6
			mRemoveNodeCallback?.Invoke(this);
#else
			if (mRemoveNodeCallback != null)
				mRemoveNodeCallback.Invoke(this);
#endif
		}

		private void OnClickAddConnection()
		{
			mInConnectionMode = true;
		}
	}
}
