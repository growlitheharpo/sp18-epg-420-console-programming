using UnityEngine;

namespace UnityEditor
{
    public class Node
    {
        private Rect mRect;
        private string mTitle;

        private GUIStyle mStyle;

	    public Rect rect { get { return mRect; } }
		public string title { get { return mTitle; } }
		public GUIStyle style { get { return mStyle; } }

		public Node(Vector2 pos, float width, float height, GUIStyle nodeStyle)
		{
			mRect = new Rect(pos.x, pos.y, width, height);
			mStyle = nodeStyle;
		}

		public void Drag(Vector2 delta)
		{
			mRect.position += delta;
		}

		public void Draw()
		{
			//GUI.Box(rect, title, style);
			
			GUI.Box(rect, title, style);
		}

		public bool ProcessEvents(Event e)
		{
			return false;
		}
	}
}
