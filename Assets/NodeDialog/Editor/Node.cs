using System;
using UnityEngine;

namespace UnityEditor
{
    public class Node
    {
        private Rect mRect;
        private string mTitle;

        private GUIStyle mStyle;
		private bool mIsDragged;
		private NodeConnectionPoint mInPoint, mOutPoint;

	    public Rect rect { get { return mRect; } }
		public string title { get { return mTitle; } }
		public GUIStyle style { get { return mStyle; } }

		public Node(Vector2 pos, float width, float height, GUIStyle nodeStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<NodeConnectionPoint> onClickInPoint, Action<NodeConnectionPoint> onClickOutPoint)
		{
			mRect = new Rect(pos.x, pos.y, width, height);
			mStyle = nodeStyle;

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
							GUI.changed = true;
						}
						else
							GUI.changed = false;
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
	}
}
