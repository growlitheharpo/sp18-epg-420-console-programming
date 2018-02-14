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

        private void OnGUI()
        {
            DrawNodes();
            ProcessEvents(Event.current);

            if (GUI.changed)
                Repaint();
        }

        private void DrawNodes()
        {
        }

        private void ProcessEvents(Event current)
        {
        }
    }
}
