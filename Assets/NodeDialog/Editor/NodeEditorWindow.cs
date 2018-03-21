using System.Collections.Generic;
using System.Linq;
using NodeDialog;
using UnityEngine;

namespace UnityEditor
{
	public class NodeEditorWindow : EditorWindow
	{
		private static DialogCharacter kEditingCharacter;

		public static void CreateNewWindow(DialogCharacter dialogCharacter)
		{
			kEditingCharacter = dialogCharacter;
			NodeEditorWindow window = GetWindow<NodeEditorWindow>();
			window.titleContent = new GUIContent("Node Dialog Editor");
		}

		private DialogCharacter mCachedCharacter;
		private List<BaseDialogGraphNode> mNodes;
		private Vector2 mDrag;
		private GUIStyle mNodeStyle, mInConnectionStyle, mOutConnectionStyle;

		/// <summary>
		/// Setup all of the styles that we're going to use.
		/// </summary>
		private void OnEnable()
		{
			mDrag = Vector2.zero;
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

			mNodes = new List<BaseDialogGraphNode>();
			InitializeFromCharacter();

			Undo.undoRedoPerformed += OnUndoRedoPerformed;
		}

		private void OnDisable()
		{
			Undo.undoRedoPerformed -= OnUndoRedoPerformed;
		}

		private void OnUndoRedoPerformed()
		{
			GUI.changed = true;
			InitializeFromCharacter();
			Repaint();
		}

		private void InitializeFromCharacter()
		{
			mNodes = new List<BaseDialogGraphNode>();

			mCachedCharacter = kEditingCharacter;
			if (mCachedCharacter == null)
				return;

			var nodes = mCachedCharacter.GetNodes_Editor();
			foreach (BaseDialogNode n in nodes)
				mNodes.Add(new BaseDialogGraphNode(n, mNodeStyle, OnRemoveNode));
		}

		/// <summary>
		/// Draw the background and the nodes and then process events.
		/// </summary>
		private void OnGUI()
		{
			DrawBackground();
			DrawGrid(12.0f, Color.white * 0.420f);
			DrawGrid(120.0f, Color.white * 0.29f);
			
			if (mCachedCharacter == null)
				return;

			if (mCachedCharacter != kEditingCharacter || mCachedCharacter.GetNodes_Editor().Count != mNodes.Count)
				InitializeFromCharacter();

			DrawNodes();
			DrawConnections();

			ProcessNodeEvents(Event.current);
			ProcessEvents(Event.current);

			if (GUI.changed)
				Repaint();
		}

		private void DrawBackground()
		{
			Color oldCol = GUI.color;
			GUI.color = new Color(0.451f, 0.451f, 0.451f, 1.0f);
			GUI.DrawTexture(new Rect(0.0f, 0.0f, position.width, position.height), EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);
			GUI.color = oldCol;
		}

		private void DrawGrid(float spacing, Color col)
		{
			int widthDivs = Mathf.CeilToInt(position.width / spacing);
			int heightDivs = Mathf.CeilToInt(position.height / spacing);

			Handles.BeginGUI();
			Handles.color = new Color(col.r, col.g, col.b, 1.0f);

			// Draw the vertical lines
			for (int i = 0; i < widthDivs; i++)
				Handles.DrawLine(new Vector3(spacing * i, 0.0f, 0), new Vector3(spacing * i, position.height, 0.0f));

			// Draw the horizontal lines
			for (int i = 0; i < heightDivs; i++)
				Handles.DrawLine(new Vector3(0.0f, spacing * i, 0), new Vector3(position.width, spacing * i, 0.0f));

			Handles.color = Color.white;
			Handles.EndGUI();
		}

		private void DrawConnections()
		{
			/*if (mConnections == null)
				return;

			for (int i = 0; i < mConnections.Count; i++)
			{
				mConnections[i].Draw();
			}*/
		}

		private void DrawNodes()
		{
			if (mNodes == null)
				return;

			foreach (BaseDialogGraphNode node in mNodes)
				node.Draw();
		}

		private void ProcessNodeEvents(Event current)
		{
			if (mNodes == null)
				return;

			for (int i = mNodes.Count - 1; i >= 0; --i)
				GUI.changed = mNodes[i].ProcessEvents(current) || GUI.changed;
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
			BaseDialogNode newRealNode = CreateInstance<BaseDialogNode>();
			
			Undo.RegisterCreatedObjectUndo(newRealNode, "Create New Node");
			Undo.RecordObject(mCachedCharacter, "Create New Node");
			
			mCachedCharacter.AddNode_Editor(newRealNode);
			newRealNode.nodePosition = mousePosition;

			mNodes.Add(new BaseDialogGraphNode(newRealNode, mNodeStyle, OnRemoveNode));
		}

		private void OnRemoveNode(BaseDialogGraphNode n)
		{
			Undo.RegisterCompleteObjectUndo(mCachedCharacter, "Delete Node");
			Undo.DestroyObjectImmediate(n.attachedNode);
			mCachedCharacter.RemoveNode_Editor(n.attachedNode);
			mNodes.Remove(n);
		}
	}
}
