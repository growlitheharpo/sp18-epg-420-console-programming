using System;
using NodeDialog;
using UnityEngine;

namespace UnityEditor
{
	public class BaseDialogGraphNode
	{
		private const float WIDTH = 200.0f, HEIGHT = 50.0f;

		private readonly Action<BaseDialogGraphNode> mRemoveNodeCallback;
		private readonly GUIStyle mMasterStyle;
		private bool mIsDragged, mIsSelected;

		public Rect rect { get { return new Rect(associatedNode.nodePosition, new Vector2(WIDTH, HEIGHT)); } }
		public BaseDialogNode associatedNode { get; private set; }

		public BaseDialogGraphNode(BaseDialogNode node, GUIStyle style, Action<BaseDialogGraphNode> removeNodeCallback)
		{
			associatedNode = node;
			mMasterStyle = style;
			mRemoveNodeCallback = removeNodeCallback;
		}

		public void Draw()
		{
			string nodeName = "NODE" + associatedNode.GetInstanceID();
			GUI.SetNextControlName(nodeName);
			
			// TODO: Why doesn't focusing work anymore?
			GUIStyle styleCopy = new GUIStyle(mMasterStyle);
			if (mIsDragged || mIsSelected)
				styleCopy.normal = mMasterStyle.focused;

			GUI.Box(rect, "Wow" + mIsSelected, styleCopy);
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
			}

			return false;
		}

		private bool HandleEventMouseDown(Event e)
		{
			if (e.button == 0) // left click
			{
				if (rect.Contains(e.mousePosition))
				{
					Select();
					return true;
				}
				else if (mIsSelected)
				{
					Deselect();
					return true;
				}
			}
			else if (e.button == 1 && rect.Contains(e.mousePosition)) // right click
			{
				ProcessContextMenu();
				e.Use();
			}
			
			return false;
		}

		private void Select()
		{
			mIsDragged = true;
			mIsSelected = true;

			Selection.SetActiveObjectWithContext(associatedNode, associatedNode);
		}

		private void Deselect()
		{
			mIsSelected = false;
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
	}
}
