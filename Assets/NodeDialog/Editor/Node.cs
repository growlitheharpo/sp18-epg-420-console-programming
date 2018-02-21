﻿using System;
using UnityEngine;

namespace UnityEditor
{
    public class Node
    {
        private Rect mRect;
        private string mTitle;

        private GUIStyle mStyle;
		private bool mIsDragged, mIsSelected;
		private NodeConnectionPoint mInPoint, mOutPoint;
		private Action<Node> mOnRemoveNode;

	    public Rect rect { get { return mRect; } }
		public string title { get { return mTitle; } }
		public GUIStyle style { get { return mStyle; } }

		public Node(Vector2 pos, float width, float height, GUIStyle nodeStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<Node> onRemoveNode, Action<NodeConnectionPoint> onClickInPoint, Action<NodeConnectionPoint> onClickOutPoint)
		{
			mRect = new Rect(pos.x, pos.y, width, height);
			mStyle = nodeStyle;

			mOnRemoveNode = onRemoveNode;
			mInPoint = new NodeConnectionPoint(this, ConnectionPointType.In, inPointStyle, onClickInPoint);
			mOutPoint = new NodeConnectionPoint(this, ConnectionPointType.Out, outPointStyle, onClickOutPoint);
		}

		public void Drag(Vector2 delta)
		{
			mRect.position += delta;
		}

		public void Draw()
		{
			mInPoint.Draw();
			mOutPoint.Draw();

			if (mIsDragged || mIsSelected)
			{
				GUI.SetNextControlName("MyNodeName");
				GUI.FocusControl("MyNodeName");
			}
			else
				GUI.FocusControl("");

			GUI.Box(rect, title, style);
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

		private void ProcessContextMenu()
		{
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Remove Node"), false, OnClickRemoveNode);
			menu.ShowAsContext();
		}

		private void OnClickRemoveNode()
		{
#if NET_4_6
				mOnRemoveNode?.Invoke(this);
#else
			if (mOnRemoveNode != null)
				mOnRemoveNode.Invoke(this);
#endif
		}
	}
}
