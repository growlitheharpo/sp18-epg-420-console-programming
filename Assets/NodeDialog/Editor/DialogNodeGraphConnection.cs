using System;
using NodeDialog;
using UnityEngine;

namespace UnityEditor
{
	public class DialogNodeGraphConnection
	{
		private const float SELECTION_RADIUS = 5.0f;
		private static Color selectedColor { get { return new Color(0.42f, 0.7f, 1.0f); } }
		private static Color standardColor { get { return Color.white; } }

		private readonly Action<DialogNodeGraphConnection> mOnDeleteConnection;
		private readonly Texture2D mTriangleTexture;

		private Vector2 lineStart { get { return associatedConnection.inNode.rect.center; } }
		private Vector2 lineEnd { get { return associatedConnection.outNode.rect.center; } }
		private bool isSelected { get { return Selection.activeObject == associatedConnection; } }

		public DialogNodeConnection associatedConnection { get; private set; }

		public DialogNodeGraphConnection(DialogNodeConnection connection, /*GUIStyle s, */Action<DialogNodeGraphConnection> onDeleteConnection)
		{
			associatedConnection = connection;
			mOnDeleteConnection = onDeleteConnection;
			if (mTriangleTexture != null)
				return;

			// Setup the triangle texture if no one has yet
			//mTriangleTexture = EditorGUIUtility.IconContent("AnimatorStateTransition Icon").image as Texture2D;
			mTriangleTexture = UnityEditor.Graphs.Styles.varPinIn.normal.background as Texture2D;

			/*var pixels = baseTexture.ReadPixels();
			for (int i = 0; i < pixels.Length; ++i)
				pixels[i] = new Color(1.0f, 1.0f, 1.0f, pixels[i].a);
			mTriangleTexture.SetPixels(pixels);*/
		}

		/// <summary>
		/// Draw this connection onto the window using the Handles utility.
		/// </summary>
		public void Draw()
		{
			Color c = isSelected ? selectedColor : standardColor;
			DrawLine(lineStart, lineEnd, c);
		}

		/// <summary>
		/// Handle any events relevant to this connection.
		/// </summary>
		public bool ProcessEvents(Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
					return HandleEventMouseDown(e);
				case EventType.KeyDown:
					if (e.keyCode == KeyCode.Delete && isSelected)
						DeleteConnection();
					break;
			}
			return false;
		}

		/// <summary>
		/// Handle the mouse being clicked. Select the connection if appropriate.
		/// </summary>
		private bool HandleEventMouseDown(Event e)
		{
			if (e.button == 0) // handle left click
			{
				// Select this object if the mouse is close enough
				if (LinePointDistance(lineStart, lineEnd, e.mousePosition) < SELECTION_RADIUS)
				{
					Selection.SetActiveObjectWithContext(associatedConnection, associatedConnection);
					return true;
				}
				else if (isSelected)
				{
					Selection.SetActiveObjectWithContext(null, null);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Handle the delete key being pressed while this connection is active.
		/// </summary>
		private void DeleteConnection()
		{
#if NET_4_6
			mOnDeleteConnection?.Invoke(this);
#else
			if (mOnDeleteConnection != null)
				mOnDeleteConnection.Invoke(this);
#endif
		}

		/// <summary>
		/// Return the distance between three 2D points. Requires a little bit of 3D math.
		/// </summary>
		private float LinePointDistance(Vector2 a, Vector2 b, Vector2 p)
		{
			//sqrt(((yb - ya) * (xc - xa) + (xb - xa) * (yc - ya)) ^ 2 / ((xb - xa) ^ 2 + (yb - ya) ^ 2))
			Vector3 toLine = p - a;
			Vector3 toB = b - a;

			Vector2 proj = Vector3.Project(toLine, toB);
			return Vector2.Distance(p, proj + a);
		}

		/// <summary>
		/// Draw the straight line for a connection.
		/// </summary>
		/// <param name="a">The start position of the line.</param>
		/// <param name="b">The end position of the line.</param>
		/// <param name="c">The color of the line.</param>
		/// <param name="width">The width of the line.</param>
		public static void DrawLine(Vector2 a, Vector2 b, Color? c = null, float width = 3.0f)
		{
			if (c == null)
				c = standardColor;

			Vector2 dir = b - a;
			Vector2 t1 = a - dir, t2 = b + dir;

			Handles.DrawBezier(a, b, t2, t1, (Color)c, Texture2D.whiteTexture, width);
		}
	}
}
