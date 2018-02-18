using System;
using UnityEngine;

namespace UnityEditor
{
	public enum ConnectionPointType
	{
		In,
		Out
	}

	public class NodeConnectionPoint
	{
		private Rect mRect;
		private ConnectionPointType mType;
		private Node mNode;
		private GUIStyle mStyle;
		private Action<NodeConnectionPoint> mOnClickDelegate;

		public Rect rect { get { return mRect; } }
		public Node node { get { return mNode; } }

		public NodeConnectionPoint(Node n, ConnectionPointType t, GUIStyle s, Action<NodeConnectionPoint> onClickConnectionPoint)
		{
			mNode = n;
			mStyle = s;
			mType = t;
			mOnClickDelegate = onClickConnectionPoint;
			mRect = new Rect(0.0f, 0.0f, 10.0f, 20.0f);
		}

		public void Draw()
		{
			mRect.y = mNode.rect.y + (mNode.rect.height * 0.5f) - mRect.height * 0.5f;

			switch (mType)
			{
				case ConnectionPointType.In:
					mRect.x = mNode.rect.x - mRect.width + 8.0f;
					break;
				case ConnectionPointType.Out:
					mRect.x = mNode.rect.x + mNode.rect.width - 8.0f;
					break;
			}

			if (GUI.Button(mRect, "", mStyle))
			{
#if NET_4_6
				mOnClickDelegate?.Invoke(this);
#else
				if (mOnClickDelegate != null)
					mOnClickDelegate.Invoke(this);
#endif
			}
		}
	}
}
