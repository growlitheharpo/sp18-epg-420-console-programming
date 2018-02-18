using System.Collections.Generic;
using NodeDialog;
using UnityEngine;

namespace UnityEditor
{
	public class NodeEditorWindow : EditorWindow
	{
		private static DialogCharacter mEditingCharacter;

		public static void CreateNewWindow(DialogCharacter dialogCharacter)
		{
			mEditingCharacter = dialogCharacter;
			NodeEditorWindow window = GetWindow<NodeEditorWindow>();
			window.titleContent = new GUIContent("Node Dialog Editor");
		}

		private List<Node> mNodes;
		private List<NodeConnection> mConnections;
		private GUIStyle mNodeStyle, mInConnectionStyle, mOutConnectionStyle;

		private NodeConnectionPoint mSelectedInPoint, mSelectedOutPoint;

		private void OnEnable()
		{
			mNodeStyle = new GUIStyle
			{
				border = new RectOffset(25, 25, 7, 7),
				normal = { background = EditorGUIUtility.IconContent("node0 hex").image as Texture2D },
				focused ={ background = EditorGUIUtility.IconContent("node0 hex on").image as Texture2D }
			};

			mInConnectionStyle = new GUIStyle
			{
				border = new RectOffset(4, 4, 12, 12),
				normal = { background = EditorGUIUtility.IconContent("btn left").image as Texture2D },
				active = { background = EditorGUIUtility.IconContent("btn left on").image as Texture2D }
			};

			mOutConnectionStyle = new GUIStyle
			{
				border = new RectOffset(4, 4, 12, 12),
				normal = { background = EditorGUIUtility.IconContent("btn right").image as Texture2D },
				active = { background = EditorGUIUtility.IconContent("btn right on").image as Texture2D },
			};
		}

		private void OnGUI()
		{
			DrawNodes();
			DrawConnections();

			ProcessNodeEvents(Event.current);
			ProcessEvents(Event.current);

			if (GUI.changed)
				Repaint();
		}

		private void DrawConnections()
		{
			if (mConnections == null)
				return;

			for (int i = 0; i < mConnections.Count; i++)
			{
				mConnections[i].Draw();
			}
		}

		private void DrawNodes()
		{
			if (mNodes == null)
				return;

			foreach (Node node in mNodes)
				node.Draw();
		}

		private void ProcessNodeEvents(Event current)
		{
			if (mNodes != null)
			{
				for (int i = mNodes.Count - 1; i >= 0; --i)
					GUI.changed = mNodes[i].ProcessEvents(current) || GUI.changed;
			}
		}

		private void ProcessEvents(Event current)
		{
			switch (current.type)
			{
				case EventType.MouseDown:
					if (current.button == 1)
						ProcessContextMenu(current.mousePosition);
					break;
			}
		}

		private void ProcessContextMenu(Vector2 mousePosition)
		{
			GenericMenu rightClickMenu = new GenericMenu();
			rightClickMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
			rightClickMenu.ShowAsContext();
		}

		private void OnClickAddNode(Vector2 mousePosition)
		{
			if (mNodes == null)
				mNodes = new List<Node>();

			mNodes.Add(new Node(mousePosition, 200.0f, 50.0f, mNodeStyle, mInConnectionStyle, mOutConnectionStyle, OnClickInConnection, OnClickOutConnection));
		}

		private void OnClickInConnection(NodeConnectionPoint point)
		{
			mSelectedInPoint = point;
			if (mSelectedOutPoint == null)
				return;

			if (mSelectedOutPoint.node != mSelectedInPoint.node)
				CreateConnection();

			ClearConnectionSelection();
		}

		private void OnClickOutConnection(NodeConnectionPoint point)
		{
			mSelectedOutPoint = point;
			if (mSelectedInPoint == null)
				return;

			if (mSelectedOutPoint.node != mSelectedInPoint.node)
				CreateConnection();

			ClearConnectionSelection();
		}

		private void CreateConnection()
		{
			if (mConnections == null)
				mConnections = new List<NodeConnection>();

			mConnections.Add(new NodeConnection(mSelectedInPoint, mSelectedOutPoint, OnClickRemoveConnection));
		}

		private void ClearConnectionSelection()
		{
			mSelectedInPoint = null;
			mSelectedOutPoint = null;
		}

		private void OnClickRemoveConnection(NodeConnection conn)
		{
			mConnections.Remove(conn);
		}
	}
}
