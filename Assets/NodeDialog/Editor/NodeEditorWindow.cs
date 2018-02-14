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
		private GUIStyle mNodeStyle;

		private void OnEnable()
		{
			mNodeStyle = new GUIStyle
			{
				border = new RectOffset(25, 25, 7, 7)
			};
			GUIContent content = EditorGUIUtility.IconContent("node0 hex");
			mNodeStyle.normal.background = content.image as Texture2D;
		}

		private void OnGUI()
        {
            DrawNodes();
            ProcessEvents(Event.current);

            if (GUI.changed)
                Repaint();
        }

        private void DrawNodes()
		{
			if (mNodes == null)
				return;

			foreach (Node node in mNodes)
				node.Draw();
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

			mNodes.Add(new Node(mousePosition, 200.0f, 50.0f, mNodeStyle));
		}
	}
}
