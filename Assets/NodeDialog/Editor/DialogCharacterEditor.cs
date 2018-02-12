using NodeDialog;
using UnityEngine;

namespace UnityEditor
{
    [CustomEditor(typeof(DialogCharacter))]
    public class DialogCharacterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();

            if (GUILayout.Button("Edit Dialog Tree"))
                NodeEditorWindow.CreateNewWindow(target as DialogCharacter);
        }
    }
}