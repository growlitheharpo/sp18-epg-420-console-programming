using System;
using UnityEngine;

namespace UnityEditor
{
	public class NodeConnection
	{
		private NodeConnectionPoint mInPoint, mOutPoint;
		private Action<NodeConnection> mOnClickRemoveConnectionDelegate;

		public NodeConnection(NodeConnectionPoint inPoint, NodeConnectionPoint outPoint, Action<NodeConnection> onRemove)
		{
			mInPoint = inPoint;
			mOutPoint = outPoint;
			mOnClickRemoveConnectionDelegate = onRemove;
		}

		public void Draw()
		{
			Handles.DrawBezier(
				mInPoint.rect.center,
				mOutPoint.rect.center,
				mInPoint.rect.center + Vector2.left * 50.0f,
				mOutPoint.rect.center - Vector2.left * 50.0f,
				Color.white,
				null,
				2.0f);

			if (Handles.Button((mInPoint.rect.center + mOutPoint.rect.center) * 0.5f, Quaternion.identity, 4.0f, 8.0f,
				Handles.RectangleHandleCap))
			{

#if NET_4_6
				mOnClickRemoveConnectionDelegate?.Invoke(this);
#else
				if (mOnClickRemoveConnectionDelegate != null)
					mOnClickRemoveConnectionDelegate.Invoke(this);
#endif
			}
		}
	}
}
