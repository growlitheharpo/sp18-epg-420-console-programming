using System;
using System.Collections;
using System.Collections.Generic;
using NodeDialog;
using UnityEngine;

namespace UnityEditor
{
	public class BaseDialogGraphNode
	{
		private const float WIDTH = 200.0f, HEIGHT = 50.0f;

		private GUIStyle mStyle;
		private bool mIsDragged, mIsSelected;
		private Action<BaseDialogGraphNode> mRemoveNodeCallback;

		public Rect rect { get { return new Rect(attachedNode.nodePosition, new Vector2(WIDTH, HEIGHT)); } }
		public BaseDialogNode attachedNode { get; private set; }

		public BaseDialogGraphNode(BaseDialogNode node, GUIStyle style, Action<BaseDialogGraphNode> removeNodeCallback)
		{
			attachedNode = node;
			mStyle = style;
			mRemoveNodeCallback = removeNodeCallback;
		}

		public void Draw()
		{
			if (mIsDragged || mIsSelected)
			{
				GUI.SetNextControlName("MyNodeName");
				GUI.FocusControl("MyNodeName");
			}
			else
				GUI.FocusControl("");

			GUI.Box(rect, "Wow", mStyle);
		}

		public bool ProcessEvents(Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
					if (e.button == 0)
					{
						if (rect.Contains(e.mousePosition))
						{
							mIsDragged = true;
							mIsSelected = true;
							GUI.changed = true;
							return true;
						}

						mIsDragged = false;
						if (mIsSelected)
						{
							mIsSelected = false;
							return true;
						}
					}
					else if (e.button == 1 && rect.Contains(e.mousePosition))
					{
						ProcessContextMenu();
						e.Use();
					}
					break;
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

		private void Drag(Vector2 delta)
		{
			attachedNode.nodePosition += delta;
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
